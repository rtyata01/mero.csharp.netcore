namespace NetCore.WorkItemService.Dto.External
{
    /// <summary>
    /// Hotpatch baseline, which represents baseline information when baseline reset happens by releasing cold-patch to all hotpatch enrolled machines.
    /// </summary>
    public class HotpatchBaseline
    {
        /// <summary>
        /// Release Ticket Id.
        /// </summary>
        public int ReleaseTicketId { get; set; }

        /// <summary>
        /// The package job ID of the coldpatch LCU package.
        /// </summary>
        public int LcuJobId { get; set; }
    }
}
