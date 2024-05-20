namespace NetCore.WorkItemService.Handler.WorkItems
{
    using NetCore.WorkItemService.Dto.External;
    using NetCore.WorkItemService.Dto.Internal.Azure;

    /// <summary>
    /// BaselineCreator class for validating and creating the BaselineInfo and HotpatchBaseline.
    /// </summary>
    public static class BaselineCreator
    {
        /// <summary>
        /// Create BaselineInfo, that contains the virtual RTM build and latest baseline lcu job id, declared before the input Release Ticket Id.
        /// </summary>
        /// <param name="releaseTicketId">Input ReleaseTicketId.</param>
        /// <param name="AzureRebaseBaselines">Azure RebaseBaselines.</param>
        /// <param name="logger">ILogger.</param>
        /// <returns>BaselineInfo.</returns>sss
        public static BaselineInfo CreateBaselineInfo(int releaseTicketId, AzureRebaseBaselines AzureRebaseBaselines, ILogger logger)
        {
            BaselineInfo baseline = null;
            if (AzureRebaseBaselines?.Baselines == null || !AzureRebaseBaselines.Baselines.Any())
            {
                logger.LogInformation("No AzureRebaseBaselines found for WorkItem Id: {ReleaseTicketId}!", releaseTicketId);
                return baseline;
            }

            // Order by TTGL and then pick the first for finding the latest Baseline.
            AzureRebaseBaseline latestBaseline = AzureRebaseBaselines.Baselines.OrderByDescending(x => x.Ttgl).First();
            logger.LogInformation("Found Latest AzureRebaseBaseline for WorkItem Id: {ReleaseTicketId}!", releaseTicketId);
            logger.LogInformation("CreateBaselineInfo: {LatestBaseline}", WriteAs.Json(latestBaseline));

            string virtualRtmBuild = string.Empty;
            if (!string.IsNullOrWhiteSpace(latestBaseline.VirtualBuildString))
            {
                virtualRtmBuild = Build.CreateBuildFromBuildName(latestBaseline.VirtualBuildString).GetBuildName();
            }

            baseline = new BaselineInfo()
            {
                ReleaseTicketId = releaseTicketId,
                LcuJobId = GetBaselineLcuJobId(latestBaseline, logger),
                VirtualBuildName = virtualRtmBuild,
            };

            return baseline;
        }

        /// <summary>
        /// Create HotpatchBaseline with required details.
        /// </summary>
        /// <param name="releaseTicketId">Input ReleaseTicketId.</param>
        /// <param name="AzureHotpatchBaseline">Azure HotpatchBaseline.</param>
        /// <param name="logger">ILogger.</param>
        /// <returns>HotpatchBaseline.</returns>
        public static HotpatchBaseline CreateHotpatchBaseline(int releaseTicketId, AzureHotpatchBaseline AzureHotpatchBaseline, ILogger logger)
        {
            if (AzureHotpatchBaseline == null)
            {
                logger.LogInformation("No AzureHotpatchBaseline found for WorkItem Id: {ReleaseTicketId}!", releaseTicketId);
                return null;
            }

            logger.LogInformation("Found AzureHotpatchBaseline for WorkItem Id: {ReleaseTicketId}!", releaseTicketId);
            logger.LogInformation("CreateHotpatchBaseline: {AzureHotpatchBaseline}", WriteAs.Json(AzureHotpatchBaseline));

            if (AzureHotpatchBaseline.LcuPackageJobId == default)
            {
                logger.LogInformation("Found invalid AzureHotpatchBaseline.LcuPackageJobId value i.e. zero");
                return null;
            }

            return new HotpatchBaseline()
            {
                ReleaseTicketId = releaseTicketId,
                LcuJobId = AzureHotpatchBaseline.LcuPackageJobId,
            };
        }

        private static int GetBaselineLcuJobId(AzureRebaseBaseline AzureRebaseBaseline, ILogger logger)
        {
            AzureRebasedBaselinePackage lcuPackageJobInfo = AzureRebaseBaseline.BaselinePackageJobInfos?.Where(x => "LatestCumulativeUpdate".Equals(x.PackageType, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (lcuPackageJobInfo == null)
            {
                logger.LogInformation("Found AzureBaselineInfo with missing or invalid lcu baseline package!");
                return default;
            }

            return lcuPackageJobInfo.PackageJobId;
        }
    }
}
