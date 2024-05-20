namespace NetCore.WorkItemService.Dto.Converters
{
    using NetCore.WorkItemService.Dto.Internal.OnPrem;
    using NetCore.WorkItemService.Handler.Clients.OnPrem.Extensions;

    /// <summary>
    /// OnPremWorkItemConverter class.
    /// </summary>
    public static class OnPremWorkItemConverter
    {
        /// <summary>
        /// Convert to OnPremBugPayload.
        /// </summary>
        /// <param name="workItem">Workitem Object.</param>
        /// <returns>OnPremBugPayload.</returns>
        public static OnPremBugPayload ToOnPremBugPayload(this WorkItem workItem)
        {
            return new OnPremBugPayload()
            {
                Id = workItem.Id ?? default,
                BinaryFiles = workItem.GetBinaryFiles(),
                ReproSteps = workItem.GetReproSteps(),
                Release = workItem.GetRelease(),
            };
        }

        /// <summary>
        /// Convert to OnPremWorkItem.
        /// </summary>
        /// <param name="workItem">WorkItem Object.</param>
        /// <returns>OnPremWorkItem.</returns>
        public static OnPremWorkItem ToOnPremWorkItem(this WorkItem workItem)
        {
            OnPremWorkItem OnPremWorkItem = new OnPremWorkItem()
            {
                Id = workItem.Id ?? default,
                Type = workItem.GetWorkItemType(),
                IsBug = workItem.IsBug(),
                IsReleaseTicket = workItem.IsReleaseTicket(),
                Product = workItem.GetProduct(),
                KbArticleNumber = workItem.GetKBNumber(),
                Title = workItem.GetTitle(),
            };

            if (OnPremWorkItem.IsBug)
            {
                OnPremWorkItem.IssueType = workItem.GetIssueType();
                OnPremWorkItem.BinaryFiles = workItem.GetBinaryFiles();
                OnPremWorkItem.ReproSteps = workItem.GetReproSteps();
            }

            if (OnPremWorkItem.IsReleaseTicket)
            {
                OnPremWorkItem.UpdateType = workItem.GetUpdateType();
                OnPremWorkItem.Branch = workItem.GetBranch();
                OnPremWorkItem.Build = workItem.GetBuild();
            }

            OnPremWorkItem.State = workItem.GetState();
            OnPremWorkItem.Keywords = workItem.GetKewords();
            OnPremWorkItem.AssignedTo = workItem.GetAssignedTo();
            OnPremWorkItem.ModifiedDate = workItem.GetModifiedDate();
            OnPremWorkItem.Triage = workItem.GetTriage();
            OnPremWorkItem.Release = workItem.GetRelease(OnPremWorkItem.IsReleaseTicket);
            OnPremWorkItem.ReleaseType = workItem.GetReleaseType(OnPremWorkItem.IsReleaseTicket, OnPremWorkItem.IsBug);
            OnPremWorkItem.TargetDate = workItem.GetTtglDateTime(OnPremWorkItem.IsReleaseTicket);

            return OnPremWorkItem;
        }
    }
}
