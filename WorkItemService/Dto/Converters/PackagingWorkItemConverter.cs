namespace NetCore.WorkItemService.Dto.Converters
{
    using NetCore.WorkItemService.Dto.External;
    using NetCore.WorkItemService.Dto.Internal.OnPrem;
    using NetCore.WorkItemService.Dto.Internal.Azure;
    using NetCore.WorkItemService.Handler.Clients.OnPrem.Extensions;
    using System;

    /// <summary>
    /// AzureWorkItemConverter.
    /// </summary>
    public static class PackagingWorkItemConverter
    {
        /// <summary>
        /// Convert to PackagingWorkItem.
        /// </summary>
        /// <param name="releaseTicket">Azure ReleaseTicket.</param>
        /// <returns>PackagingWorkItem.</returns>
        public static PackagingWorkItem ToPackagingWorkItem(this AzureReleaseTicket releaseTicket)
        {
            return new PackagingWorkItem
            {
                Id = int.Parse(releaseTicket.Id),
                IsBug = false,
                IsReleaseTicket = true,
                Title = releaseTicket.Title,
                KBArticleNumber = int.Parse(releaseTicket.KbArticle),
                Product = releaseTicket.OriginProduct,
                HotpatchProduct = releaseTicket.HotpatchProduct,
                UpdateType = releaseTicket.UpdateType,
                Release = releaseTicket.Release,
                Branch = releaseTicket.Branch,
                HotpatchBranch = releaseTicket.HotpatchBranch,
                TargetDate = GetTargetDate(releaseTicket.Ttgl),
                ModifiedDate = releaseTicket.ModifiedOn,
                KeyWords = string.Join(";", releaseTicket.Tags ?? new string[] { string.Empty }),
                State = releaseTicket.PublishingStatus,
                ReleaseType = releaseTicket.UpdateType,
                Triage = OnPremConstants.ReleaseTicketTriageAsApproved,
                IssueType = OnPremConstants.ReleaseTicketIssueTypeAsCodeDefect,
                AssignedTo = string.Empty,
                BinaryFiles = string.Empty,
                Build = string.Empty,
                Type = OnPremConstants.TypeReleaseTicket,
                PayloadQuery = releaseTicket.PayloadQuery,
                IsBaseline = releaseTicket.IsBaseline,
                ProductVersion = releaseTicket.ProductVersion,
            };
        }


        /// <summary>
        /// Convert to PackagingWorkItem.
        /// </summary>
        /// <param name="workItem">OnPrem workitem.</param>
        /// <returns>PackagingWorkItem.</returns>
        public static PackagingWorkItem ToPackagingWorkItem(this OnPremWorkItem workItem)
        {
            return new PackagingWorkItem
            {
                Id = workItem.Id,
                Type = workItem.Type,
                IsBug = workItem.IsBug,
                IsReleaseTicket = workItem.IsReleaseTicket,
                Title = workItem.Title,
                KBArticleNumber = workItem.KbArticleNumber,
                Product = workItem.Product,
                HotpatchProduct = null,
                ReleaseType = workItem.ReleaseType,
                UpdateType = workItem.UpdateType,
                Release = workItem.Release,
                Branch = workItem.Branch,
                HotpatchBranch = null,
                BinaryFiles = workItem.BinaryFiles,
                IssueType = workItem.IssueType,
                KeyWords = workItem.Keywords,
                Triage = workItem.Triage,
                Build = workItem.Build,
                TargetDate = workItem.TargetDate,
                ModifiedDate = workItem.ModifiedDate,
                State = workItem.State,
                AssignedTo = workItem.AssignedTo,
                IsBaseline = false, // Setting false, as this field is only present for Azure Release Ticket.
                ProductVersion = null, // Setting to null, as this field is only present for Azure Release Ticket.
            };
        }

        private static DateTime GetTargetDate(string ttgl)
        {
            if (!string.IsNullOrWhiteSpace(ttgl))
            {
                if (DateTime.TryParse(ttgl, out DateTime targetDate))
                {
                    return targetDate;
                }
            }

            return DateTime.MinValue;
        }
    }
}
