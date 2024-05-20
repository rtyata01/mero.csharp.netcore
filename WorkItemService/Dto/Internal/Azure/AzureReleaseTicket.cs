namespace NetCore.WorkItemService.Dto.Internal.Azure
{
    using System;

    /// <summary>
    /// AzureWorkItem class.
    /// </summary>
    public class AzureReleaseTicket
    {
        /// <summary>
        /// Azure WorkItem ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Product Family.
        /// </summary>
        public string ProductFamily { get; set; }

        /// <summary>
        /// The actual product of the workitem.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// The Hotpatch product for Hotpatch Package Type.
        /// </summary>
        public string HotpatchProduct { get; set; }

        /// <summary>
        /// The origin product of the workitem.
        /// Mainly relevant for H2 style products (like 19H1 and 19H2) and EKB products.
        /// Packaging will use this <see cref="OriginProduct"/> instead of <see cref="Product"/> as the product to create the package on.
        /// </summary>
        /// <remarks>
        /// For example, a workitem for WIR EKB will have <see cref="Product"/> set to "Windows Insider Refresh (EKB)" and
        /// <see cref="OriginProduct"/> set to "Windows Insider Refresh".
        /// That is because from a packaging sense, it is created just like a regular "Windows Insider Refresh" product package.
        /// For this reason the packaging system will use the <see cref="OriginProduct"/> to determine which product to use in packaging.
        /// </remarks>
        public string OriginProduct { get; set; }

        /// <summary>
        /// Update Type.
        /// </summary>
        public string UpdateType { get; set; }

        /// <summary>
        /// Release.
        /// </summary>
        public string Release { get; set; }

        /// <summary>
        /// Publishing Status.
        /// </summary>
        public string PublishingStatus { get; set; }

        /// <summary>
        /// MSRC Severity.
        /// </summary>
        public string MsrcSeverity { get; set; }

        /// <summary>
        /// KB Article.
        /// </summary>
        public string KbArticle { get; set; }

        /// <summary>
        /// Branch.
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// The Hotpatch Branch for Hotpatch Package Type.
        /// </summary>
        public string HotpatchBranch { get; set; }

        /// <summary>
        /// TTGL.
        /// </summary>
        public string Ttgl { get; set; }

        /// <summary>
        /// Created Date.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Modified Date.
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// Tags.
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Payload Query.
        /// </summary>
        public string PayloadQuery { get; set; }

        /// <summary>
        /// Is Pre RTM.
        /// </summary>
        public bool IsPreRtm { get; set; }

        /// <summary>
        /// Branch Type.
        /// </summary>
        public string BranchType { get; set; }

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
