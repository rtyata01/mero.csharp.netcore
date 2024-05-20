namespace NetCore.WorkItemService.Dto.External
{
    /// <summary>
    /// BaselineInfo contains the virtual RTM build and latest baseline lcu job id, declared before the input Release Ticket Id.
    /// </summary>
    public class BaselineInfo
    {
        /// <summary>
        /// Release Ticket Id.
        /// </summary>
        public int ReleaseTicketId { get; set; }

        /// <summary>
        /// Lcu Job Id, which indicates latest baseline package.
        /// </summary>
        public int LcuJobId { get; set; }

        /// <summary>
        /// Virtual RTM build name, which will be used for rebased LCUs. Expected format {Major}.{Qfe}.{BranchName}.{Timestamp(i.e. yyMMdd-HHmm)}.
        /// </summary>
        public string VirtualBuildName { get; set; }
    }
}
