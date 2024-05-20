namespace NetCore.WorkItemService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class WorkItemService : BaseWebApiStatelessService<Startup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemService"/> class.
        /// Constructor.
        /// </summary>
        /// <param name="context">StatelessService Context.</param>
        public WorkItemService(StatelessServiceContext context)
            : base(context, ServiceEventSource.Current)
        {
            this.WebHostBuilder
                .AddServiceFabricStartupConfig(context)
                .AddAzureAppConfigurationWithRefresh()
                .AddServiceFabricTelemetryEnrichers(context)
                .AddOpenTelemetry()
                .AddLegacyPrtLogging();
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateTheListeners();
        }
    }
}
