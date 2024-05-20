namespace NetCore.WorkItemService.Dto.External
{
    using System.Collections.Generic;

    /// <summary>
    /// Details about a payload that is mentiond in a workitem.
    /// </summary>
    public class WorkItemPayload
    {
        /// <summary>
        /// The ID of the workitem from which the payload was read.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The binaries that are mentioned in the workitem.
        /// These are determined via a heuristic, and may not be accurate.
        /// </summary>
        public IEnumerable<WorkItemBinary> Binaries { get; set; }

        /// <summary>
        /// The components that are mentioned in the workitem.
        /// These are determined via a heuristic, and may not be accurate.
        /// </summary>
        public IEnumerable<WorkItemComponent> Components { get; set; }

        /// <summary>
        /// The payload items that are mentioned in the workitem.
        /// These may be either binaries or components.
        /// </summary>
        public IEnumerable<string> PayloadItems { get; set; }

        /// <summary>
        /// The release the workitem belongs to.
        /// </summary>
        public string Release { get; set; }
    }
}
