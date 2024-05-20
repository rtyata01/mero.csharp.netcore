namespace NetCore.WorkItemService.Handler.Clients.Azure
{
    using NetCore.WorkItemService.Dto;
    using NetCore.WorkItemService.Dto.Internal.Azure;

    /// <summary>
    /// Azure Service.
    /// </summary>
    public class AzureApiClient : IAzureApiClient
    {
        private const string AzureAllActiveReleaseTickets = "/api/v1/releasetickets/active";
        private static readonly CompositeFormat AzureReleaseTicketUrl = CompositeFormat.Parse("api/v1/releasetickets/{0}");
        private static readonly CompositeFormat AzureLastBReleaseTicketUrl = CompositeFormat.Parse("api/v1/releasetickets/delta/{0}/{1}");
        private static readonly CompositeFormat AzureRebaseBaselinesUrl = CompositeFormat.Parse("api/v1/baseline/rebase/{0}");
        private static readonly CompositeFormat AzureHotpatchBaselineUrl = CompositeFormat.Parse("api/v1/baseline/hotpatch/{0}");

        private readonly HttpClient httpClient;

        /// <summary>
        /// AzureClient Contructor.
        /// </summary>
        /// <param name="httpClient">The http client for talking to the Azure APIs.</param>
        public AzureApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, AzureReleaseTicket>> GetReleaseTicket(int releaseTicketId, CancellationToken cancellationToken)
        {
            string requestedRoute = string.Format(null, AzureReleaseTicketUrl, releaseTicketId);
            return await this.GetDataFromAzureApi<AzureReleaseTicket>(requestedRoute: requestedRoute, allowNoContent: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, AzureReleaseTicket>> GetLastBReleaseTicket(string release, string product, CancellationToken cancellationToken)
        {
            string requestedRoute = string.Format(null, AzureLastBReleaseTicketUrl, release, product);
            return await this.GetDataFromAzureApi<AzureReleaseTicket>(requestedRoute: requestedRoute, allowNoContent: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, IEnumerable<AzureReleaseTicket>>> GetAllActiveReleaseTickets(CancellationToken cancellationToken)
        {
            return await this.GetDataFromAzureApi<IEnumerable<AzureReleaseTicket>>(requestedRoute: AzureAllActiveReleaseTickets, allowNoContent: false, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, IEnumerable<AzureReleaseTicket>>> GetReleaseTicketsByBuild(string buildName, CancellationToken cancellationToken)
        {
            string requestedRoute = string.Format(null, AzureReleaseTicketUrl, buildName);
            return await this.GetDataFromAzureApi<IEnumerable<AzureReleaseTicket>>(requestedRoute: requestedRoute, allowNoContent: false, cancellationToken: cancellationToken)
                .IfRight(releaseTickets => this.FilterReleaseTickets(releaseTickets))
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, AzureRebaseBaselines>> GetRebaseBaselines(int releaseTicketId, CancellationToken cancellationToken)
        {
            string requestedRoute = string.Format(null, AzureRebaseBaselinesUrl, releaseTicketId);
            return await this.GetDataFromAzureApi<AzureRebaseBaselines>(requestedRoute: requestedRoute, allowNoContent: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Either<StatusCodeError, AzureHotpatchBaseline>> GetHotpatchBaseline(int releaseTicketId, CancellationToken cancellationToken)
        {
            string requestedRoute = string.Format(null, AzureHotpatchBaselineUrl, releaseTicketId);
            return await this.GetDataFromAzureApi<AzureHotpatchBaseline>(requestedRoute: requestedRoute, allowNoContent: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }


        private IEnumerable<AzureReleaseTicket> FilterReleaseTickets(IEnumerable<AzureReleaseTicket> releaseTickets)
        {
            return releaseTickets.Where(rt => rt.ProductFamily.Contains("Windows Servicing", StringComparison.InvariantCultureIgnoreCase)
                    && (rt.UpdateType.StartsWith("Cumulative", StringComparison.InvariantCultureIgnoreCase)
                        && !rt.UpdateType.Contains("Delta", StringComparison.InvariantCultureIgnoreCase))
                    );
        }

        private async Task<Either<StatusCodeError, T>> GetDataFromAzureApi<T>(string requestedRoute, bool allowNoContent, CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                var response = await this.httpClient.GetAsync(requestedRoute, cancellationToken).ConfigureAwait(false);
                string content = response.Content != null ? await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false) : string.Empty;

                if (response.IsSuccessStatusCode)
                {
                    if (allowNoContent && response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default(T);
                    }

                    return JsonConvert.DeserializeObject<T>(content);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return default(T);
                }
                else
                {
                    string errorMessage = $"An error occurred while calling the Azure Rest Api: {requestedRoute}! Received error code: {response.StatusCode}, Content: {content}";

                    return new StatusCodeError
                    {
                        StatusCode = response.StatusCode,
                        ErrorMessage = errorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                return new StatusCodeError
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"An error occurred while calling the Azure Rest Api: {requestedRoute}! Error: {ex.Message}",
                };
            }
        }
    }
}
