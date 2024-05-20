namespace NetCore.WorkItemService.Dto.External
{
    using System;

    /// <summary>
    /// Details of a workitem.
    /// </summary>
    public class PackagingWorkItem
    {
        /// <summary>
        /// The workitem Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The workitem Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// True if the workitem is a bug, false otherwise.
        /// </summary>
        public bool IsBug { get; set; }

        /// <summary>
        /// True if the workitem is a release ticket, false otherwise.
        /// </summary>
        public bool IsReleaseTicket { get; set; }

        /// <summary>
        /// The title of the workitem.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The KB number of the workitem.
        /// </summary>
        public int KBArticleNumber { get; set; }

        /// <summary>
        /// The product of the workitem.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// The hotpatch product of the workitem.
        /// </summary>
        public string HotpatchProduct { get; set; }

        /// <summary>
        /// The release type.
        /// </summary>
        public string ReleaseType { get; set; }

        /// <summary>
        /// The update type which the workitem should be packaged in.
        /// </summary>
        public string UpdateType { get; set; }

        /// <summary>
        /// The release when the workitem should get serviced.
        /// </summary>
        public string Release { get; set; }

        /// <summary>
        /// The branch for which the workitem should get packaged.
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// The hotpatch branch for which the workitem should get a hotpatch packaged.
        /// </summary>
        public string HotpatchBranch { get; set; }

        /// <summary>
        /// The build for which the workitem should get packaged.
        /// </summary>
        public string Build { get; set; }

        /// <summary>
        /// The state of the workitem.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Who the workitem is assigned to.
        /// </summary>
        public string AssignedTo { get; set; }

        /// <summary>
        /// The time the workitem was last modified.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// The target date when the workitem should get serviced.
        /// </summary>
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Tag keywords set on the workitem.
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// The triage status of the workitem.
        /// </summary>
        public string Triage { get; set; }

        /// <summary>
        /// The workitem issue type.
        /// </summary>
        public string IssueType { get; set; }

        /// <summary>
        /// The binary files mentioned in the workitem.
        /// </summary>
        public string BinaryFiles { get; set; }

        /// <summary>
        /// An OnPrem query for workitems which specify additional payload for the package.
        /// May be null if no such workitems exist.
        /// </summary>
        public string PayloadQuery { get; set; }

        /// <summary>
        /// True if the workitem is a baseline release, false otherwise.
        /// </summary>
        public bool IsBaseline { get; set; }

        /// <summary>
        /// The version that the OS should have after the update is installed.
        /// </summary>
        public string ProductVersion { get; set; }
    }
}
