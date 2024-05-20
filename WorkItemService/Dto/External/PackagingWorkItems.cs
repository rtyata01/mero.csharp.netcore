namespace NetCore.WorkItemService.Dto.External
{
    using System.Collections.Generic;

    /// <summary>
    /// PackagingWorkItem.
    /// </summary>
    public class PackagingWorkItems
    {
        /// <summary>
        ///  Gets or sets ReleaseMonth.
        /// </summary>
        public string ReleaseMonth { get; set; }

        /// <summary>
        ///  Gets or sets Product.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets PackagingWorkItems.
        /// </summary>
        public IEnumerable<PackagingWorkItem> PackagingWorkItemList { get; set; }
    }
}
