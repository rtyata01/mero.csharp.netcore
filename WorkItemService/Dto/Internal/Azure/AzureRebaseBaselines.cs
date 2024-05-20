namespace NetCore.WorkItemService.Dto.Internal.Azure
{
    /// <summary>
    /// AzureRebaseBaselines contains collection of past Baselines for specific Release Ticket Id.
    /// </summary>
    public class AzureRebaseBaselines
    {
        /// <summary>
        /// Product name.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// List of past Baselines for specific release ticket.
        /// </summary>
        public IEnumerable<AzureRebaseBaseline> Baselines { get; set; }
    }
}
