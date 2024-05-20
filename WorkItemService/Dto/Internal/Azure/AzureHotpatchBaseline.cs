namespace NetCore.WorkItemService.Dto.Internal.Azure
{
    /// <summary>
    /// Hotpatch baseline represents baseline information when baseline reset happens by releasing cold-patch to all hotpatch enrolled machines.
    /// </summary>
    public class AzureHotpatchBaseline
    {
        /// <summary>
        /// The hotpatch product name e.g. "Windows Server 2022 Hotpatch".
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// The build string of the coldpatch LCU package.
        /// </summary>
        public string BuildString { get; set; }

        /// <summary>
        /// The KB Article Number of the coldpatch LCU package.
        /// </summary>
        public int? LcuKbNumber { get; set; }

        /// <summary>
        /// The package job ID of the coldpatch LCU package.
        /// </summary>
        public int LcuPackageJobId { get; set; }

        /// <summary>
        /// The package <see cref="Version"/> of the coldpatch LCU package.
        /// </summary>
        public Version LcuPackageVersion { get; set; }

        /// <summary>
        /// Release this Baseline is a part of.
        /// </summary>
        public string Release { get; set; }

        /// <summary>
        /// This flag shows if the baseline reset release is Shipped (Live) or not (future release).
        /// </summary>
        public bool IsBaselineLive { get; set; }

        /// <summary>
        /// TTGL of the release.
        /// </summary>
        public DateTime Ttgl { get; set; }
    }
}
