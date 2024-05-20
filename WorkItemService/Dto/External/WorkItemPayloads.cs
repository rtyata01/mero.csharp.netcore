namespace NetCore.WorkItemService.Dto.External
{
    using System.Collections.Generic;

    /// <summary>
    /// WorkItemPayloads class.
    /// </summary>
    public class WorkItemPayloads
    {
        /// <summary>
        ///  Gets or sets WorkItem Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Payloads.
        /// </summary>
        public IEnumerable<WorkItemPayload> Payloads { get; set; }
    }
}
