namespace NetCore.WorkItemService.Handler.Clients.OnPrem.Extensions
{
    using static NetCore.WorkItemService.Handler.Clients.OnPrem.Extensions.OnPremConstants;

    /// <summary>
    /// OnPremWorkItemExtensions class.
    /// </summary>
    internal static class OnPremWorkItemExtensions
    {
        private static readonly HashSet<string> LcuUpdateTypes = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "CUMULATIVE NON-SECURITY (CATALOG ONLY)",
            "CUMULATIVE NON-SECURITY (CRITICAL)",
            "CUMULATIVE NON-SECURITY (CRITICAL) DCAT",
            "CUMULATIVE NON-SECURITY (UPDATE)",
            "CUMULATIVE NON-SECURITY (UPDATE) DCAT"
        };

        #region Common Fields

        /// <summary>
        /// Get WorkItem Type.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>WorkItem Type.</returns>
        public static string GetWorkItemType(this WorkItem workItem)
        {
            return (string)workItem?.Fields[CommonFields.WorkItemType];
        }

        /// <summary>
        /// Get WorkItem Title.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>Title.</returns>
        public static string GetTitle(this WorkItem workItem)
        {
            return workItem.GetFieldValue<string>(CommonFields.Title);
        }

        /// <summary>
        /// Get WorkItem KB Number.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>KBNumber.</returns>
        public static int GetKBNumber(this WorkItem workItem)
        {
            if (workItem.TryGetFieldValue<string>(CommonFields.KBArticle, out string kbNumberValue)
                && !string.IsNullOrWhiteSpace(kbNumberValue)
                && int.TryParse(kbNumberValue, out int kbNumber))
            {
                return kbNumber;
            }

            return default;
        }

        /// <summary>
        /// Get WorkItem Product.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>Product.</returns>
        public static string GetProduct(this WorkItem workItem)
        {
            if (workItem.TryGetFieldValue<string>(CommonFields.Product, out string product) && !string.IsNullOrWhiteSpace(product))
            {
                return product;
            }

            return default;
        }

        /// <summary>
        /// Get WorkItem State.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>State.</returns>
        public static string GetState(this WorkItem workItem)
        {
            return workItem.GetFieldValue<string>(CommonFields.State);
        }

        /// <summary>
        /// Get WorkItem Keywords or Tags.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>Keywords.</returns>
        public static string GetKewords(this WorkItem workItem)
        {
            return workItem.GetFieldValue<string>(CommonFields.Tags);
        }

        /// <summary>
        /// Get WorkItem AssignedTo.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>AssignedTo.</returns>
        public static string GetAssignedTo(this WorkItem workItem)
        {
            if (workItem.Fields.ContainsKey(CommonFields.AssignedTo))
            {
                var assignedTo = (IdentityRef)workItem.Fields[CommonFields.AssignedTo];
                if (!string.IsNullOrWhiteSpace(assignedTo?.UniqueName))
                {
                    return assignedTo.UniqueName;
                }
            }

            return string.Empty;
        }

        #endregion Common Fields


        #region BugFields

        /// <summary>
        /// Get WorkItem IssueType.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>IssueType.</returns>
        public static string GetIssueType(this WorkItem workItem)
        {
            return workItem.GetFieldValue<string>(BugFields.HowFound);
        }

        /// <summary>
        /// Get WorkItem BinaryFiles.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>BinaryFiles.</returns>
        public static string GetBinaryFiles(this WorkItem workItem)
        {
            return workItem.GetFieldValue<string>(BugFields.BinaryFilename);
        }

        /// <summary>
        /// Get WorkItem Release.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>Release.</returns>
        public static string GetRelease(this WorkItem workItem)
        {
            return workItem.GetFieldValue<string>(BugFields.Release);
        }

        /// <summary>
        /// Get WorkItem ReproSteps.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>ReproSteps.</returns>
        public static string GetReproSteps(this WorkItem workItem)
        {
            return workItem.GetFieldValue<string>(BugFields.ReproSteps);
        }

        /// <summary>
        /// Get WorkItem ModifiedDate.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>ModifiedDate.</returns>
        public static DateTime GetModifiedDate(this WorkItem workItem)
        {
            if (workItem.Fields.ContainsKey(BugFields.ChangedDate))
            {
                var modifiedDate = workItem?.Fields[BugFields.ChangedDate];
                if (modifiedDate != null)
                {
                    return (DateTime)modifiedDate;
                }
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Get WorkItem Triage.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>Triage.</returns>
        public static string GetTriage(this WorkItem workItem)
        {
            if (workItem.TryGetFieldValue<string>(BugFields.Triage, out string triage) && !string.IsNullOrWhiteSpace(triage))
            {
                return triage;
            }

            return ReleaseTicketTriageAsApproved;
        }

        #endregion BugFields


        #region ReleaseTicket Fields

        /// <summary>
        /// Get WorkItem ReleaseMonth.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>ReleaseMonth.</returns>
        public static string GetReleaseMonth(this WorkItem workItem)
        {
            if (workItem.TryGetFieldValue<string>(ReleaseTicketFields.ReleaseMonth, out string releaseMonth))
            {
                return releaseMonth;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get WorkItem ReleaseCycle.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>ReleaseCycle.</returns>
        public static string GetReleaseCycle(this WorkItem workItem)
        {
            if (workItem.TryGetFieldValue<string>(ReleaseTicketFields.ReleaseCycle, out string releaseCycle))
            {
                return releaseCycle;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get WorkItem Update Type.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>Update Type.</returns>
        public static string GetUpdateType(this WorkItem workItem)
        {
            return workItem.GetFieldValue<string>(ReleaseTicketFields.UpdateType);
        }

        /// <summary>
        /// Get WorkItem Update Type.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <param name="updateType">Output: The UpdateType if it exists.</param>
        /// <returns>True if an Update Type exists; false otherwise.</returns>
        public static bool TryGetUpdateType(this WorkItem workItem, out string updateType)
        {
            return workItem.TryGetFieldValue<string>(ReleaseTicketFields.UpdateType, out updateType) && !string.IsNullOrWhiteSpace(updateType);
        }

        /// <summary>
        /// Get WorkItem TTGL DateTime.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <param name="isReleaseTicket">IsReleaseTicket.</param>
        /// <returns>TTGL DateTime.</returns>
        public static DateTime GetTtglDateTime(this WorkItem workItem, bool isReleaseTicket)
        {
            if (isReleaseTicket && workItem.Fields.ContainsKey(ReleaseTicketFields.TargetDate))
            {
                var ttglDateTime = workItem?.Fields[ReleaseTicketFields.TargetDate];
                if (ttglDateTime != null)
                {
                    return (DateTime)ttglDateTime;
                }
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Get WorkItem Branch.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>Branch.</returns>
        public static string GetBranch(this WorkItem workItem)
        {
            return workItem.GetFieldValue<string>(ReleaseTicketFields.ServicingBranch);
        }

        /// <summary>
        /// Get WorkItem Build.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>Build.</returns>
        public static string GetBuild(this WorkItem workItem)
        {
            return workItem.GetFieldValue<string>(ReleaseTicketFields.BuildCustomString05);
        }

        #endregion ReleaseTicket Fields


        #region Others

        /// <summary>
        /// Get WorkItem Release.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <param name="isReleaseTicket">IsReleaseTicket.</param>
        /// <returns>Release.</returns>
        public static string GetRelease(this WorkItem workItem, bool isReleaseTicket)
        {
            if (workItem.TryGetFieldValue<string>(BugFields.Release, out string release) && !string.IsNullOrWhiteSpace(release))
            {
                return release;
            }

            if (string.IsNullOrWhiteSpace(release) && isReleaseTicket)
            {
                release = $"{workItem.GetReleaseMonth()} {workItem.GetReleaseCycle()}";
            }

            return release;
        }

        /// <summary>
        /// Get WorkItem ReleaseType.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <param name="isReleaseTicket">IsReleaseTicket.</param>
        /// <param name="isBug">IsBug.</param>
        /// <returns>ReleaseType.</returns>
        public static string GetReleaseType(this WorkItem workItem, bool isReleaseTicket, bool isBug)
        {
            if (isReleaseTicket && workItem.TryGetUpdateType(out string updateType))
            {
                return updateType;
            }

            if (!isBug)
            {
                return default;
            }

            if (workItem.TryGetFieldValue<string>(BugFields.SDLSeverity, out string sdlSeverity) && !string.IsNullOrWhiteSpace(sdlSeverity))
            {
                // if SDLSeverity is NOT "Not a security bug" then it is a security bug
                if (!sdlSeverity.Equals(SDLSeverities.NotASecurityBug, StringComparison.OrdinalIgnoreCase))
                {
                    return ReleaseTypes.ServicingGdrSecurity;
                }
            }

            if (workItem.TryGetFieldValue<string>(BugFields.HowFound, out string howFound))
            {
                // if SDLSeverity is "Not a security bug" then it is a security bug if HowFound contains "security"
                if (howFound.Contains(PatternSecurity, StringComparison.OrdinalIgnoreCase))
                {
                    return ReleaseTypes.ServicingGdrSecurity;
                }
            }

            // must be a non security bug
            return ReleaseTypes.ServicingGdrNonSecurity;
        }

        /// <summary>
        /// Check if WorkItem is a Bug Type.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>True if workItemId corresponds to a Bug.</returns>
        public static bool IsBug(this WorkItem workItem)
        {
            if (workItem is null)
            {
                return false;
            }

            return OnPremWorkItemType.Bug.ToString().Equals(GetWorkItemType(workItem), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if WorkItem is a Release Ticket Type.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>True if workItemId corresponds to a ReleaseTicket.</returns>
        public static bool IsReleaseTicket(this WorkItem workItem)
        {
            if (workItem is null)
            {
                return false;
            }

            return OnPremWorkItemType.ReleaseTicket.ToString().Equals(GetWorkItemType(workItem), StringComparison.OrdinalIgnoreCase)
                || OnPremWorkItemType.ReleaseTicketProto.ToString().Equals(GetWorkItemType(workItem), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if WorkItem is a Container Update Type.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>True if workItem corresponds to a Container Based Update type.</returns>
        public static bool IsContainerUpdateType(this WorkItem workItem)
        {
            if (workItem.TryGetUpdateType(out string updateType))
            {
                switch (updateType.Trim().ToUpperInvariant())
                {
                    case "UNIFIED CUMULATIVE NON-SECURITY":
                    case "UNIFIED CUMULATIVE SECURITY":
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the workitem is LCU update type.
        /// </summary>
        /// <param name="workItem">OnPrem WorkItem.</param>
        /// <returns>True if workItem corresponds to a LCU Update type.</returns>
        public static bool IsLcuUpdateType(this WorkItem workItem)
        {
            if (workItem.TryGetUpdateType(out string updateType))
            {
                return LcuUpdateTypes.Contains(updateType.Trim().ToUpperInvariant());
            }

            return false;
        }

        #endregion Others

        private static T GetFieldValue<T>(this WorkItem workItem, string fieldName)
            where T : class
        {
            if (workItem != null && workItem.Fields.TryGetValue(fieldName, out object fieldValue))
            {
                return (T)fieldValue;
            }

            return default;
        }

        private static bool TryGetFieldValue<T>(this WorkItem workItem, string fieldName, out T value)
            where T : class
        {
            value = null;
            if (workItem != null && workItem.Fields.TryGetValue(fieldName, out object fieldValue))
            {
                value = (T)fieldValue;
            }

            return value != null;
        }
    }
}