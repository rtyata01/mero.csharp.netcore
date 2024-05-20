namespace NetCore.WorkItemService.Handler.Clients.OnPrem
{
    using NetCore.WorkItemService.Dto;
    using NetCore.WorkItemService.Dto.Internal.OnPrem;

    /// <summary>
    /// IOnPremApiClient interface.
    /// </summary>
    public interface IOnPremApiClient
    {
        /// <summary>
        /// Get OnPremWorkItem for specific WorkItem Id.
        /// </summary>
        /// <param name="workItemId">WorkItem Id.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>OnPremWorkItem.</returns>
        Task<Either<StatusCodeError, OnPremWorkItem>> GetWorkItem(int workItemId, CancellationToken cancellationToken);

        /// <summary>
        /// Get a collection of OnPremBugPayload for a specified shared OnPrem query.
        /// </summary>
        /// <param name="queryId">The ID of the OnPrem query which returns the list of workitems whose payload should be loaded.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>OnPrem workitem payloads.</returns>
        Task<IEnumerable<OnPremBugPayload>> GetWorkitemPayloadsByQuery(Guid queryId, CancellationToken cancellationToken);

        /// <summary>
        /// Get Collection of OnPremBugPayload for specific collection of products and releases.
        /// </summary>
        /// <param name="products">Products.</param>
        /// <param name="releases">Releases.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>OnPremWorkItem.</returns>
        Task<IEnumerable<OnPremBugPayload>> GetAllBugWorkItemsForProductsAndReleases(IEnumerable<string> products, IEnumerable<string> releases, CancellationToken cancellationToken);
    }
}
