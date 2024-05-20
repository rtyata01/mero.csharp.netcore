namespace NetCore.WorkItemService.Dto.Internal.OnPrem
{
    using System;

    /// <summary>
    /// OnPremWorkItem class.
    /// </summary>
    public class OnPremWorkItem
    {
        /// <summary>
        /// OnPrem WorkItem ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///  Gets or sets Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// IsBug.
        /// </summary>
        public bool IsBug { get; set; }

        /// <summary>
        /// IsReleaseTicket.
        /// </summary>
        public bool IsReleaseTicket { get; set; }

        /// <summary>
        /// Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// KB Article.
        /// </summary>
        public int KbArticleNumber { get; set; }

        /// <summary>
        /// Product.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Release Type.
        /// </summary>
        public string ReleaseType { get; set; }

        /// <summary>
        /// Update Type.
        /// </summary>
        public string UpdateType { get; set; }

        /// <summary>
        /// Release.
        /// </summary>
        public string Release { get; set; }

        /// <summary>
        /// Branch.
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// Build.
        /// </summary>
        public string Build { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// AssignedTo.
        /// </summary>
        public string AssignedTo { get; set; }

        /// <summary>
        ///  ModifiedDate.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// TTGL.
        /// </summary>
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Triage.
        /// </summary>
        public string Triage { get; set; }

        /// <summary>
        /// IssueType.
        /// </summary>
        public string IssueType { get; set; }

        /// <summary>
        /// Keywords.
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// BinaryFiles.
        /// </summary>
        public string BinaryFiles { get; set; }

        /// <summary>
        /// ReproSteps.
        /// </summary>
        public string ReproSteps { get; set; }
    }
}
