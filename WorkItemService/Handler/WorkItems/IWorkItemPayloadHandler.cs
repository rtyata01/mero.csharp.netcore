namespace NetCore.WorkItemService.Handler.WorkItems
{
    using NetCore.WorkItemService.Dto.External;
    using NetCore.WorkItemService.Dto.Internal.OnPrem;

    /// <summary>
    /// IWorkItemPayloadHandler.
    /// </summary>
    public interface IWorkItemPayloadHandler
    {
        /// <summary>
        /// Get WorkItemPayload for specific OnPremWorkItem.
        /// </summary>
        /// <param name="OnPremWorkItem">OnPremWorkItem.</param>
        /// <returns>WorkItemPayload.</returns>
        WorkItemPayload GetOnPremWorkItemPayload(OnPremWorkItem OnPremWorkItem);

        /// <summary>
        /// Get Collection of WorkItemPayload for collection of OnPremBugPayload.
        /// </summary>
        /// <param name="OnPremBugPayloads">Collection of OnPremBugPayload.</param>
        /// <returns>WorkItemPayloads.</returns>
        IEnumerable<WorkItemPayload> GetOnPremBugPayloads(IEnumerable<OnPremBugPayload> OnPremBugPayloads);

        /// <summary>
        /// Get WorkItemPayload for specific OnPremBugPayload.
        /// </summary>
        /// <param name="OnPremBugPayload">OnPremBugPayload.</param>
        /// <returns>WorkItemPayload.</returns>
        WorkItemPayload GetOnPremBugPayload(OnPremBugPayload OnPremBugPayload);
    }
}
