namespace NetCore.WorkItemService.Dto.Internal.Azure
{
    /// <summary>
    /// AzureRebasedBaselinePackage that contains the information about the baseline package declared before the input Release Ticket Id.
    /// </summary>
    public class AzureRebasedBaselinePackage
    {
        /// <summary>
        /// Azure package type.
        /// </summary>
        public string PackageType { get; set; }

        /// <summary>
        ///  Packaging job id.
        /// </summary>
        public int PackageJobId { get; set; }

        /// <summary>
        /// Package version (e.g. 1.2).
        /// </summary>
        public Version PackageVersion { get; set; }

        /// <summary>
        /// OS build version from which the package was built (e.g. 10.0.123.4567).
        /// </summary>
        public Version PackageOsVersion { get; set; }
    }
}
