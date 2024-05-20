namespace NetCore.WorkItemService.Handler.Clients.Azure
{
    using NetCore.WorkItemService.Dto;
    using NetCore.WorkItemService.Dto.Internal.Azure;

    /// <summary>
    /// IAzureApiClient interface.
    /// </summary>
    public interface IAzureApiClient
    {
        /// <summary>
        /// Get AzureReleaseTicket for specific ReleaseTicket Id.
        /// </summary>
        /// <param name="releaseTicketId">ReleaseTicket Id.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>AzureReleaseTicket.</returns>
        Task<Either<StatusCodeError, AzureReleaseTicket>> GetReleaseTicket(int releaseTicketId, CancellationToken cancellationToken);

        /// <summary>
        /// Get AzureReleaseTicket for specific Release and ProductName.
        /// </summary>
        /// <param name="release">Release.</param>
        /// <param name="product">Product Name.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>AzureReleaseTicket.</returns>
        Task<Either<StatusCodeError, AzureReleaseTicket>> GetLastBReleaseTicket(string release, string product, CancellationToken cancellationToken);

        /// <summary>
        /// Get collection of active AzureReleaseTicket.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Collection of AzureReleaseTicket.</returns>
        Task<Either<StatusCodeError, IEnumerable<AzureReleaseTicket>>> GetAllActiveReleaseTickets(CancellationToken cancellationToken);

        /// <summary>
        /// Get collection of AzureReleaseTicket for specific build name.
        /// </summary>
        /// <param name="buildName">Build Name (ex. 16257.40.rs3_release_svc_escrow_im.170908-1117).</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Collection of AzureReleaseTicket.</returns>
        Task<Either<StatusCodeError, IEnumerable<AzureReleaseTicket>>> GetReleaseTicketsByBuild(string buildName, CancellationToken cancellationToken);

        /// <summary>
        /// Get AzureBaselineInfos for specific ReleaseTicket Id.
        /// </summary>
        /// <param name="releaseTicketId">ReleaseTicket Id.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>AzureBaselineInfos.</returns>
        Task<Either<StatusCodeError, AzureRebaseBaselines>> GetRebaseBaselines(int releaseTicketId, CancellationToken cancellationToken);

        /// <summary>
        /// Get AzureHotpatchBaseline for specific ReleaseTicket Id.
        /// </summary>
        /// <param name="releaseTicketId">ReleaseTicket Id.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>AzureHotpatchBaseline.</returns>
        Task<Either<StatusCodeError, AzureHotpatchBaseline>> GetHotpatchBaseline(int releaseTicketId, CancellationToken cancellationToken);
    }
}