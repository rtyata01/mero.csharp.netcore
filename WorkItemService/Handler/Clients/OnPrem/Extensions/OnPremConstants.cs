namespace NetCore.WorkItemService.Handler.Clients.OnPrem.Extensions
{
    internal static class OnPremConstants
    {
        public const string TypeBug = "Bug";
        public const string TypeReleaseTicket = "Release Ticket";
        public const string TypeReleaseTicketProto = "Release Ticket-Proto";
        public const string PatternSecurity = "security";
        public const string MobileOnlyWithSpace = "Mobile Only";
        public const string MobileOnlyWithoutSpace = "MobileOnly";
        public const string SeeReproSteps = "seereprosteps";
        public const string ReleaseTicketTriageAsApproved = "Approved";
        public const string ReleaseTicketIssueTypeAsCodeDefect = "Code Defect";

        /// <summary>
        /// VSO query for retrieving bugs associated with a particular product/release.
        /// </summary>
        public const string GetByProductAndReleaseQueryFormat =
           @"SELECT [NetCore.Id] FROM [WorkItems]
               WHERE [Work Item Type] = 'WorkItem'
                    AND [Team Project] = 'Project'
                    AND [Issue Type] <> 'Test Defect'
                    AND ([State] = 'Active' OR [Resolved Reason] = 'Fixed')
                    AND [Triage] CONTAINS 'Approved'
                    AND [Product] IN ({0}) 
                    AND [Release] IN ({1})";

        public static class ReleaseTypes
        {
            public const string CumulativeNonSecurity = "Cumulative Non-Security";
            public const string CumulativeSecurity = "Cumulative Security";
            public const string ServicingGdrNonSecurity = "Servicing GDR Non-Security";
            public const string ServicingGdrSecurity = "Servicing GDR Security";
        }

        public static class SDLSeverities
        {
            public const string NotASecurityBug = "Not A Security Bug";
            public const string Low = "Low";
            public const string Moderate = "Moderate";
            public const string Important = "Important";
            public const string Critical = "Critical";
        }

        public static class CommonFields
        {
            public const string Id = "NetCore.Id";
            public const string Title = "NetCore.Title";
            public const string WorkItemType = "NetCore.WorkItemType";
            public const string State = "NetCore.State";
            public const string AssignedTo = "NetCore.AssignedTo";
            public const string Tags = "NetCore.Tags";
            public const string ProductFamily = "NetCore.ProductFamily";
            public const string Product = "NetCore.Product";
            public const string KBArticle = "NetCore.KBArticleNumber";
        }

        public static class BugFields
        {
            public const string IssueType = "NetCore.IssueType";
            public const string BinaryFilename = "NetCore.BinaryFilename";
            public const string SDLSeverity = "NetCore.SDLSeverity";
            public const string DevOwner = "NetCore.DevOwner";
            public const string PMOwner = "NetCore.PMOwner";
            public const string QualityOwner = "NetCore.QualityOwner";
            public const string HowFound = "NetCore.VSTS.CMMI.HowFound";
            public const string Release = "NetCore.VSTS.Common.Release";
            public const string Triage = "NetCore.VSTS.Common.Triage";
            public const string ReproSteps = "NetCore.VSTS.TCM.ReproSteps";
            public const string Keywords = "NetCore.VSTS.Common.Keywords";
            public const string ResolvedBy = "NetCore.VSTS.Common.ResolvedBy";
            public const string ChangedDate = "NetCore.ChangedDate";
        }

        public static class ReleaseTicketFields
        {
            public const string ReleaseCycle = "NetCore.ReleaseCycle";
            public const string ReleaseMonth = "NetCore.ReleaseMonth";
            public const string UpdateType = "NetCore.UpdateType";
            public const string DeploymentDate = "NetCore.Scheduling.DeploymentDate";
            public const string ServicingBranch = "NetCore.VSTS.Branch.Servicing";
            public const string TargetDate = "NetCore.VSTS.Scheduling.TargetDate";
            public const string BuildCustomString05 = "NetCore.VSTS.Common.CustomString05";
        }
    }
}
