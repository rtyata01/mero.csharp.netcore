namespace NetCore.WorkItemService.Handler.Clients.OnPrem.Extensions
{
    /// <summary>
    /// OnPremWorkItemType class.
    /// </summary>
    internal class OnPremWorkItemType
    {
        private readonly string typeName;

        private OnPremWorkItemType(string typeName)
        {
            this.typeName = string.IsNullOrWhiteSpace(typeName) ? throw new ArgumentNullException(nameof(typeName)) : typeName;
        }

        /// <summary>
        /// Get the Type of the OnPremWorkItem.
        /// </summary>
        /// <returns>OnPremWorkItem Type.</returns>
        public override string ToString()
        {
            return this.typeName;
        }

        /// <summary>
        /// OnPremWorkItem Bug Type.
        /// </summary>
        public static OnPremWorkItemType Bug { get; } = new OnPremWorkItemType(OnPremConstants.TypeBug);

        /// <summary>
        /// OnPremWorkItem Release Ticket Type.
        /// </summary>
        public static OnPremWorkItemType ReleaseTicket { get; } = new OnPremWorkItemType(OnPremConstants.TypeReleaseTicket);


        /// <summary>
        /// OnPremWorkItem Release Ticket Type.
        /// </summary>
        public static OnPremWorkItemType ReleaseTicketProto { get; } = new OnPremWorkItemType(OnPremConstants.TypeReleaseTicketProto);
    }
}
