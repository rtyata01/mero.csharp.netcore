namespace NetCore.WorkItemService.Handler.Clients.OnPrem
{
    using NetCore.WorkItemService.Dto;
    using NetCore.WorkItemService.Dto.Converters;
    using NetCore.WorkItemService.Dto.Internal.OnPrem;
    using static NetCore.WorkItemService.Handler.Clients.OnPrem.Extensions.OnPremConstants;

    /// <summary>
    /// OnPremApiClient.
    /// </summary>
    public class OnPremApiClient : IOnPremApiClient
    {
        private readonly string[] fields = new[] { CommonFields.Id, BugFields.BinaryFilename, BugFields.ReproSteps, BugFields.Release };
        private readonly IVssConnectionFactory vssConnectionFactory;
        private readonly ILogger<OnPremApiClient> logger;

        /// <summary>
        /// Initialize a new instance of <see cref="OnPremApiClient"/>.
        /// </summary>
        /// <param name="vssConnectionFactory">The factory to create VssConnections.</param>
        /// <param name="logger">ILogger.</param>
        public OnPremApiClient(IVssConnectionFactory vssConnectionFactory, ILogger<OnPremApiClient> logger)
        {
            this.vssConnectionFactory = vssConnectionFactory ?? throw new ArgumentNullException(nameof(vssConnectionFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, OnPremWorkItem>> GetWorkItem(int workItemId, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = await this.vssConnectionFactory.GetVssConnection(cancellationToken).ConfigureAwait(false);
                WorkItem workItem = await connection.Client.GetWorkItemAsync(workItemId, expand: WorkItemExpand.Relations, cancellationToken: cancellationToken).ConfigureAwait(false);

                return workItem.ToOnPremWorkItem();
            }
            catch (VssServiceResponseException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return default(OnPremWorkItem);
            }
            catch (VssServiceException)
            {
                return new StatusCodeError
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessage = $"WorkItem: {workItemId} is a protected workItem. PkgCloud does not have permissions to the WorkItem.",
                };
            }
            catch (Exception ex)
            {
                return new StatusCodeError
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"Unable to extract fields for WorkItem: {workItemId}, Error: {ex.Message}",
                };
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OnPremBugPayload>> GetWorkitemPayloadsByQuery(Guid queryId, CancellationToken cancellationToken)
        {
            if (queryId == default)
            {
                return Enumerable.Empty<OnPremBugPayload>();
            }

            Stopwatch stopWatch = Stopwatch.StartNew();
            try
            {
                using var connection = await this.vssConnectionFactory.GetVssConnection(cancellationToken).ConfigureAwait(false);
                WorkItemQueryResult workItemQueryResult = await connection.Client.QueryByIdAsync(new TeamContext("OS"), queryId, cancellationToken: cancellationToken).ConfigureAwait(false);

                return await this.GetPayloadsFromQueryResult(connection.Client, workItemQueryResult, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get workitem query {queryId} results from OnPrem.", ex);
            }
            finally
            {
                this.logger.LogInformation("Processing completed for GetWorkitemPayloadsByQuery. ElapsedTime: {Elapsed}", stopWatch.Elapsed);
                stopWatch.Stop();
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OnPremBugPayload>> GetAllBugWorkItemsForProductsAndReleases(IEnumerable<string> products, IEnumerable<string> releases, CancellationToken cancellationToken)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            try
            {
                string formattedProducts = string.Join(",", products.Select(p => $"'{p}'"));
                string formattedReleases = string.Join(",", releases.Select(r => $"'{r}'"));
                this.logger.LogInformation("Querying all bug workitems for Products: {FormattedProducts}, Releases: {FormattedReleases}", formattedProducts, formattedReleases);

                Wiql wiql = new Wiql()
                {
                    Query = string.Format(GetByProductAndReleaseQueryFormat, formattedProducts, formattedReleases),
                };

                using var connection = await this.vssConnectionFactory.GetVssConnection(cancellationToken).ConfigureAwait(false);
                WorkItemQueryResult workItemQueryResult = await connection.Client.QueryByWiqlAsync(wiql, timePrecision: null, top: null, userState: null, cancellationToken: cancellationToken).ConfigureAwait(false);
                return await this.GetPayloadsFromQueryResult(connection.Client, workItemQueryResult, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                this.logger.LogInformation("Processing completed for GetAllBugWorkItemsForProductsAndReleases. ElapsedTime: {Elapsed}", stopWatch.Elapsed);
                stopWatch.Stop();
            }
        }

        private async Task<IEnumerable<OnPremBugPayload>> GetPayloadsFromQueryResult(WorkItemTrackingHttpClient client, WorkItemQueryResult workItemQueryResult, CancellationToken cancellationToken)
        {
            List<WorkItem> workitems = new List<WorkItem>();
            if (workItemQueryResult.WorkItems != null && workItemQueryResult.WorkItems.Any())
            {
                foreach (IEnumerable<WorkItemReference> batch in workItemQueryResult.WorkItems.Batch(100))
                {
                    var workItemIds = batch.Select(p => p.Id).ToArray();
                    var workItems = await client.GetWorkItemsAsync(workItemIds, this.fields, asOf: null, expand: null, errorPolicy: null, userState: null, cancellationToken: cancellationToken).ConfigureAwait(false);

                    workitems.AddRange(workItems);
                }
            }

            if (workitems.Count != 0)
            {
                return workitems.Select(item => item.ToOnPremBugPayload()).ToList();
            }

            return new List<OnPremBugPayload>();
        }
    }
}
