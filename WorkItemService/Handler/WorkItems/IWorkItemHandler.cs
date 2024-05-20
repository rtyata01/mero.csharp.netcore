namespace NetCore.WorkItemService.Handler.WorkItems
{
    using NetCore.WorkItemService.Dto;
    using NetCore.WorkItemService.Dto.External;
    using NetCore.WorkItemService.Dto.Internal.Azure;

    /// <summary>
    /// IWorkItemHandler interface.
    /// </summary>
    public interface IWorkItemHandler
    {
        /// <summary>
        /// Get PackagingWorkItem for given WorkItem Id.
        /// </summary>
        /// <param name="workItemId">WorkItem Id.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>PackagingWorkItem.</returns>
        Task<Either<StatusCodeError, PackagingWorkItem>> GetWorkItem(int workItemId, CancellationToken cancellationToken);

        /// <summary>
        /// Get WorkItemPayloads for given WorkItem Id.
        /// </summary>
        /// <param name="workItemId">WorkItem Id.</param>
        /// <param name="includeChildren">Include Children.</param>
        /// <param name="includeBugList">Include payloads from bug list. Will be ignored if includeChildren is true.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>WorkItemPayloads.</returns>
        Task<Either<StatusCodeError, WorkItemPayloads>> GetWorkItemPayloads(int workItemId, bool includeChildren, bool includeBugList, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get PackagingWorkItems for given releaseMonth and product.
        /// </summary>
        /// <param name="releaseMonth">Release month.</param>
        /// <param name="product">Product.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>PackagingWorkItems.</returns>
        Task<Either<StatusCodeError, PackagingWorkItems>> GetWorkItemForReleaseMonth(string releaseMonth, string product, CancellationToken cancellationToken);

        /// <summary>
        /// Get release tickets where update type is LCU.
        /// </summary>
        /// <param name="AzureReleaseTickets">List of Azure release tickets.</param>
        /// <returns>List of filtered release tickets.</returns>
        Either<StatusCodeError, IEnumerable<AzureReleaseTicket>> FilterLcuReleaseTickets(IEnumerable<AzureReleaseTicket> AzureReleaseTickets);

        /// <summary>
        /// Get the latest BaselineInfo for specific ReleaseTicket Id.
        /// </summary>
        /// <param name="releaseTicketId">ReleaseTicket Id.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>BaselineInfo.</returns>
        Task<Either<StatusCodeError, BaselineInfo>> GetLatestBaselineInfo(int releaseTicketId, CancellationToken cancellationToken);

        /// <summary>
        /// Get HotpatchBaseline for specific ReleaseTicket Id.
        /// </summary>
        /// <param name="releaseTicketId">ReleaseTicket Id.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>HotpatchBaseline.</returns>
        Task<Either<StatusCodeError, HotpatchBaseline>> GetHotpatchBaseline(int releaseTicketId, CancellationToken cancellationToken);
    }
}
