namespace NetCore.WorkItemService.Dto.Internal.OnPrem
{
    /// <summary>
    /// OnPremBugPayload class.
    /// </summary>
    public class OnPremBugPayload
    {
        /// <summary>
        /// OnPrem WorkItem ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// BinaryFiles.
        /// </summary>
        /// </summary>
        public string BinaryFiles { get; set; }

        /// <summary>
        /// ReproSteps.
        /// </summary>
        public string ReproSteps { get; set; }

        /// <summary>
        /// Release.
        /// </summary>
        public string Release { get; set; }
    }
}
