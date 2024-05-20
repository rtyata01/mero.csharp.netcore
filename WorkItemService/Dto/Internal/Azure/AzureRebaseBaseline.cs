namespace NetCore.WorkItemService.Dto.Internal.Azure
{
    /// <summary>
    /// AzureRebaseBaseline contains the information about the virtual RTM build and latest baseline packages declared before the input Release Ticket Id.
    /// </summary>
    public class AzureRebaseBaseline
    {
        /// <summary>
        /// Release this Baseline is a part of.
        /// e.g. "2021.09 B".
        /// </summary>
        public string Release { get; set; }

        /// <summary>
        /// List of packages which are part of baseline.
        /// </summary>
        public IEnumerable<AzureRebasedBaselinePackage> BaselinePackageJobInfos { get; set; }

        /// <summary>
        /// This flag shows if the baseline release is Shipped (Live) or not (In-Progress).
        /// </summary>
        public bool IsBaselineLive { get; set; }

        /// <summary>
        /// TTGL of the release.
        /// </summary>
        public DateTime Ttgl { get; set; }

        /// <summary>
        /// Virtual Build string with format {Major}.{Qfe}.{BranchName}.{Timestamp(i.e. yyMMdd-HHmm)}.
        /// </summary>
        public string VirtualBuildString { get; set; }
    }
}
