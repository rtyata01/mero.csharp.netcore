namespace NetCore.WorkItemService.Settings
{
    /// <summary>
    /// SettingsModel for WorkItem Service.
    /// </summary>
    internal class SettingsModel : ServiceFabricWebApiConfig
    {
        /// <summary>
        /// Gets or sets the NetCore tenant Application Registration for authenticating/authorizing.
        /// </summary>
        [PipelineConfiguration("AzureWorkItemServiceClientId", true)]
        public string AzureWorkItemServiceClientId { get; set; }

        /// <summary>
        /// Gets or sets the NetCore Tenant Id.
        /// </summary>
        [PipelineConfiguration("AzureTenantId", true)]
        public string AzureTenantId { get; set; }

        /// <summary>
        /// Gets or sets the MS tenant authority for authenticating/authorizing.
        /// </summary>
        [PipelineConfiguration("AzureAuthority", true)]
        public string AzureAuthority { get; set; }

        /// <summary>
        /// Gets or sets the PME tenant Application Registration for authenticating/authorizing.
        /// </summary>
        [PipelineConfiguration("CloudPublicApisClientId", true)]
        public string CloudPublicApisClientId { get; set; }

        /// <summary>
        /// Gets or sets the PME Tenant Id.
        /// </summary>
        [PipelineConfiguration("CloudTenantId", true)]
        public string CloudTenantId { get; set; }

        /// <summary>
        /// Gets or sets the Pme tenant authority for authenticating/authorizing.
        /// </summary>
        [PipelineConfiguration("CloudAuthority", true)]
        public string CloudAuthority { get; set; }

        /// <summary>
        /// Gets or sets the custom Pme tenant audience to use in addition of the default audiences.
        /// This configures what audience is valid in the auth token.
        /// </summary>
        [PipelineConfiguration("CloudAudience", true)]
        public string CloudAudience { get; set; }
    }
}
