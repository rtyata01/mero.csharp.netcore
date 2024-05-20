namespace NetCore.WorkItemService.Handler.WorkItems
{
    using NetCore.WorkItemService.Dto;
    using NetCore.WorkItemService.Dto.Converters;
    using NetCore.WorkItemService.Dto.External;
    using NetCore.WorkItemService.Dto.Internal.OnPrem;
    using NetCore.WorkItemService.Dto.Internal.Azure;
    using NetCore.WorkItemService.Handler.Clients.OnPrem;
    using NetCore.WorkItemService.Handler.Clients.Azure;

    /// <summary>
    /// WorkItemHandler class.
    /// </summary>
    public class WorkItemHandler : IWorkItemHandler
    {
        private readonly IAzureApiClient AzureClient;
        private readonly IOnPremApiClient OnPremClient;
        private readonly IWorkItemPayloadHandler workItemPayloadHandler;
        private readonly ProductConfig productConfig;
        private readonly ILogger<WorkItemHandler> logger;

        /// <summary>
        /// WorkItemHandler Constructor.
        /// </summary>
        /// <param name="AzureApiClient">Azure Api Client.</param>
        /// <param name="OnPremApiClient">OnPrem Api Client.</param>
        /// <param name="workItemPayloadHandler">WorkItem Payload Handler.</param>
        /// <param name="productConfig">The product app configuration.</param>
        /// <param name="logger">ILogger.</param>
        public WorkItemHandler(IAzureApiClient AzureApiClient, IOnPremApiClient OnPremApiClient, IWorkItemPayloadHandler workItemPayloadHandler, IOptionsSnapshot<ProductConfig> productConfig, ILogger<WorkItemHandler> logger)
        {
            this.AzureClient = AzureApiClient ?? throw new ArgumentNullException(nameof(AzureApiClient));
            this.OnPremClient = OnPremApiClient ?? throw new ArgumentNullException(nameof(OnPremApiClient));
            this.workItemPayloadHandler = workItemPayloadHandler ?? throw new ArgumentNullException(nameof(workItemPayloadHandler));
            this.productConfig = productConfig?.Value ?? throw new ArgumentNullException(nameof(productConfig));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, PackagingWorkItem>> GetWorkItem(int workItemId, CancellationToken cancellationToken)
        {
            return await this.AzureClient.GetReleaseTicket(workItemId, cancellationToken)
                .IfRight(AzureReleaseTicket => this.ShouldFallBackToBugWorkItem(AzureReleaseTicket, workItemId, cancellationToken))
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, PackagingWorkItems>> GetWorkItemForReleaseMonth(string releaseMonth, string product, CancellationToken cancellationToken)
        {
            return await this.AzureClient.GetAllActiveReleaseTickets(cancellationToken)
                .IfRight(AzureReleaseTickets => this.FilterLcuReleaseTickets(AzureReleaseTickets))
                .IfRight(lcuReleaseTickets => RetrieveWorkItemForReleaseMonth(lcuReleaseTickets, releaseMonth, product))
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Either<StatusCodeError, IEnumerable<AzureReleaseTicket>> FilterLcuReleaseTickets(IEnumerable<AzureReleaseTicket> AzureReleaseTickets)
        {
            if (AzureReleaseTickets != null && AzureReleaseTickets.Any())
            {
                return AzureReleaseTickets.Where(r => WorkItemExtensions.IsLcuUpdateType(r.UpdateType))?.ToList();
            }

            return new StatusCodeError
            {
                StatusCode = HttpStatusCode.NotFound,
                ErrorMessage = $"No release tickets provided."
            };
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, WorkItemPayloads>> GetWorkItemPayloads(int workItemId, bool includeChildren, bool includeBugList, CancellationToken cancellationToken = default)
        {
            return await this.AzureClient.GetReleaseTicket(workItemId, cancellationToken)
                .IfRight(AzureReleaseTicket => this.RetrieveWorkItemPayloads(AzureReleaseTicket, workItemId, includeChildren, includeBugList, cancellationToken))
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, BaselineInfo>> GetLatestBaselineInfo(int releaseTicketId, CancellationToken cancellationToken)
        {
            try
            {
                return await this.AzureClient.GetRebaseBaselines(releaseTicketId, cancellationToken)
                        .IfRight(rebaseBaselines => BaselineCreator.CreateBaselineInfo(releaseTicketId, rebaseBaselines, this.logger))
                        .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return CreateStatusCodeError(ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, HotpatchBaseline>> GetHotpatchBaseline(int releaseTicketId, CancellationToken cancellationToken)
        {
            try
            {
                return await this.AzureClient.GetHotpatchBaseline(releaseTicketId, cancellationToken)
                        .IfRight(AzureHotpatchBaseline => BaselineCreator.CreateHotpatchBaseline(releaseTicketId, AzureHotpatchBaseline, this.logger))
                        .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return CreateStatusCodeError(ex);
            }
        }

        #region private methods

        private static StatusCodeError CreateStatusCodeError(Exception ex)
        {
            if (ex is ArgumentException || ex is ArgumentNullException)
            {
                return new StatusCodeError()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessage = ex.Message,
                };
            }

            return new StatusCodeError()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMessage = ex.Message,
            };
        }

        private static Either<StatusCodeError, PackagingWorkItems> RetrieveWorkItemForReleaseMonth(IEnumerable<AzureReleaseTicket> lcuReleaseTickets, string releaseMonth, string product)
        {
            string errorMessage;

            if (lcuReleaseTickets != null && lcuReleaseTickets.Any())
            {
                var filteredLcuReleaseTickets = lcuReleaseTickets.Where(r => r.Release.Contains(releaseMonth, StringComparison.InvariantCultureIgnoreCase)
                    && r.Product.Equals(product, StringComparison.InvariantCultureIgnoreCase));

                var packagingWorkItemList = filteredLcuReleaseTickets.Select(w => w.ToPackagingWorkItem()).ToList();

                if (packagingWorkItemList.Count != 0)
                {
                    return new PackagingWorkItems
                    {
                        ReleaseMonth = releaseMonth,
                        Product = product,
                        PackagingWorkItemList = packagingWorkItemList
                    };
                }
                else
                {
                    errorMessage = $"Release tickets are not found for release month {releaseMonth} and product {product}";
                }
            }
            else
            {
                errorMessage = $"No release tickets provided";
            }

            return new StatusCodeError
            {
                StatusCode = HttpStatusCode.NotFound,
                ErrorMessage = errorMessage
            };
        }

        private async Task<Either<StatusCodeError, PackagingWorkItem>> ShouldFallBackToBugWorkItem(AzureReleaseTicket AzureReleaseTicket, int workItemId, CancellationToken cancellationToken)
        {
            if (AzureReleaseTicket != null)
            {
                return AzureReleaseTicket.ToPackagingWorkItem();
            }

            return await this.OnPremClient.GetWorkItem(workItemId, cancellationToken)
                    .IfRight(OnPremWorkItem => OnPremWorkItem?.ToPackagingWorkItem())
                    .ConfigureAwait(false);
        }

        private async Task<Either<StatusCodeError, WorkItemPayloads>> RetrieveWorkItemPayloads(AzureReleaseTicket AzureReleaseTicket, int workItemId, bool includeChildren, bool includeBuglist, CancellationToken cancellationToken)
        {
            if (AzureReleaseTicket != null)
            {
                return await this.RetrieveAzureReleaseTicketPayloads(AzureReleaseTicket, workItemId, includeChildren, includeBuglist, cancellationToken).ConfigureAwait(false);
            }

            // If Not ReleaseTicket WorkItem, then fallback to OnPrem Bug WorkItem.
            return await this.OnPremClient.GetWorkItem(workItemId, cancellationToken)
                    .IfRight(OnPremWorkItem => this.RetrieveBugWorkItemPayloads(OnPremWorkItem, workItemId, includeChildren, cancellationToken))
                    .ConfigureAwait(false);
        }

        private async Task<Either<StatusCodeError, WorkItemPayloads>> RetrieveAzureReleaseTicketPayloads(AzureReleaseTicket AzureReleaseTicket, int workItemId, bool includeChildren, bool includeBuglist, CancellationToken cancellationToken)
        {
            // By default ReleaseTicket itself has no payload and IncludeChildren is expected to be always true for LCU ReleaseTicket.
            if (includeChildren)
            {
                return await this.RetrieveAllBugPayloads(workItemId, AzureReleaseTicket.Product, AzureReleaseTicket.Release, cancellationToken).ConfigureAwait(false);
            }
            if (includeBuglist && this.TryParseQueryId(AzureReleaseTicket.PayloadQuery, out Guid queryId))
            {
                try
                {
                    var payloads = await this.OnPremClient.GetWorkitemPayloadsByQuery(queryId, cancellationToken).ConfigureAwait(false);
                    return this.ConstructWorkItemPayloads(workItemId, payloads);
                }
                catch (Exception ex)
                {
                    // log warning, but do not fail
                    this.logger.LogWarning("Could not get payloads from bug list. Error: {Exception}", ex);
                }
            }

            return CreateDefaultWorkItemPayloads(workItemId, AzureReleaseTicket.Release);
        }

        private bool TryParseQueryId(string payloadQuery, out Guid queryId)
        {
            queryId = Guid.Empty;
            if (string.IsNullOrEmpty(payloadQuery))
            {
                return false;
            }

            var match = Regex.Match(payloadQuery, @"\?id=([0-9a-f-]{36})");
            return match.Success && Guid.TryParse(match.Groups[1].Value, out queryId);
        }

        private static WorkItemPayloads CreateDefaultWorkItemPayloads(int workItemId, string release)
        {
            return new WorkItemPayloads
            {
                Id = workItemId,
                Payloads = new List<WorkItemPayload>
                {
                    new WorkItemPayload
                    {
                        Id = workItemId,
                        Binaries = new List<WorkItemBinary>(),
                        Components = new List<WorkItemComponent>(),
                        PayloadItems = new List<string>(),
                        Release = release,
                    }
                }
            };
        }

        private async Task<Either<StatusCodeError, WorkItemPayloads>> RetrieveBugWorkItemPayloads(OnPremWorkItem OnPremWorkItem, int workItemId, bool includeChildren, CancellationToken cancellationToken)
        {
            // If Bug WorkItem Not Found through fallback OnPremClient, return Not Found.
            if (OnPremWorkItem == null)
            {
                return new StatusCodeError
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessage = $"WorkItem Id: {workItemId} not found!"
                };
            }

            // If Bug WorkItem and IncludeChildren is true, then return all bug payloads including the parent.
            if (includeChildren)
            {
                return await this.RetrieveAllBugPayloads(workItemId, OnPremWorkItem.Product, OnPremWorkItem.Release, cancellationToken).ConfigureAwait(false);
            }

            // If Bug WorkItem and IncludeChildren is false, then just return the parent bug payload.
            return new WorkItemPayloads
            {
                Id = workItemId,
                Payloads = new List<WorkItemPayload> { this.workItemPayloadHandler.GetOnPremWorkItemPayload(OnPremWorkItem) }
            };
        }

        private async Task<Either<StatusCodeError, WorkItemPayloads>> RetrieveAllBugPayloads(int workItemId, string product, string release, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(release))
            {
                string errorMsg = $"Invalid WorkItem! Product and Release Field value cannot be empty.";
                this.logger.LogWarning("RetrieveAllBugPayloads: {ErrorMsg}", errorMsg);

                return new StatusCodeError
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessage = errorMsg
                };
            }

            List<string> queryProducts = this.GetAllProducts(product);
            IEnumerable<string> queryReleases = new List<string>() { release };

            var payloads = await this.OnPremClient.GetAllBugWorkItemsForProductsAndReleases(queryProducts, queryReleases, cancellationToken).ConfigureAwait(false);
            return this.ConstructWorkItemPayloads(workItemId, payloads);
        }

        private WorkItemPayloads ConstructWorkItemPayloads(int workItemId, IEnumerable<OnPremBugPayload> allBugPayloads)
        {
            var payloads = this.workItemPayloadHandler.GetOnPremBugPayloads(allBugPayloads)
                .OrderBy(payload => payload.Id);

            return new WorkItemPayloads
            {
                Id = workItemId,
                Payloads = payloads,
            };
        }

        private List<string> GetAllProducts(string product)
        {

            List<string> productMappings = new List<string>() { product };

            // Note: This mapping between 1607 Desktop and 1607 Server Product is required to unblock server releases. This is required as it use single codebase for servicing.
            // This is only required for RS1 and RS5 Products. RS3 is no longer serviced.
            // For HCIv1 and ASDB, the CFE TNT has automation in place to clone the RS5 bugs as HCIv1 and as ASDB.
            if (this.productConfig.ClientServerProductMapping != null)
            {
                foreach (var entry in this.productConfig.ClientServerProductMapping)
                {
                    if (product.Equals(entry.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        productMappings.Add(entry.Value);
                    }
                }
            }

            return productMappings;
        }

        #endregion private methods
    }
}
