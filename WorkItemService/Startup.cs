namespace NetCore.WorkItemService
{
    using NetCore.WorkItemService.Handler.Clients.OnPrem;
    using NetCore.WorkItemService.Handler.Clients.Azure;
    using NetCore.WorkItemService.Handler.WorkItems;
    using NetCore.WorkItemService.Settings;

    /// <summary>
    /// Startup Class.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly SettingsModel settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// Constructor.
        /// </summary>
        /// <param name="serviceContext">Context of the service.</param>
        /// <param name="configuration">Configuration of the service.</param>
        public Startup(StatelessServiceContext serviceContext, IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.settings = this.CreateSettingsModel(serviceContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service that is being run.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // bind app configuration sections to refreshable dependency injected config classes
            services.Configure<ProductConfig>(this.configuration.GetSection(ProductConfig.RootNamespace));

            services.AddRouting(options => options.LowercaseUrls = true);

            // Add API versioning services.
            services.AddApiVersioning(options => options.ReportApiVersions = true);
            services.AddVersionedApiExplorer(
                options =>
                {
                    // format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true; // make help page show ~/api/v1/{controller}/ instead of ~/api/{version}/{controller}
                });

            // Setting needed to turn on CorePRT's LinkToResourceTransformer to auto populate hrefs in links section
            services.AddControllers(options =>
                options.Filters.Add<LinkToResourceTransformer>())
                //Using newtonsoft because System.Text doesn't support Dictionary serialization nicely
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            this.ConfigureAuth(services);
            this.AddHttpClients(services);

            services.Configure<ClientAppConfig>(this.configuration.GetSection(ClientAppConfig.ConfigNamespaces.PkgIntClientApp));

            services.AddOnPremApiClient(this.configuration);
            services.AddScoped<IOnPremApiClient, OnPremApiClient>();
            services.AddScoped<IWorkItemHandler, WorkItemHandler>();
            services.AddSingleton<IWorkItemPayloadHandler, WorkItemPayloadHandler>();

            services.AddAzureAppConfiguration();
        }

        private void ConfigureAuth(IServiceCollection services)
        {
            string[] authSchemes = services.ConfigureAuthentication(
                msClientId: this.settings.AzureWorkItemServiceClientId,
                AzureAuthority: this.settings.AzureAuthority,
                pmeClientId: this.settings.CloudPublicApisClientId,
                CloudAuthority: this.settings.CloudAuthority,
                customCloudAudience: this.settings.CloudAudience);

            this.ConfigureAuthorization(services, authSchemes);
        }

        private void ConfigureAuthorization(IServiceCollection services, string[] authSchemes)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Reader", policy => policy
                    .AddAuthenticationSchemes(authSchemes)
                    .RequireRole("Reader"));
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Object to build the application.</param>
        /// <param name="env">Environment hosting the application.</param>
        /// <param name="provider">Object that's dependency injected by swagger to provide metadata about the API.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseUrlLogging();
            app.UseAzureAppConfiguration();

            app.UseRouting();
            app.UseAuthentication();
            app.UseCallerLogging();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                    options.RoutePrefix = string.Empty; // show the Swagger UI as the app root page
                });
        }

        private void AddHttpClients(IServiceCollection services)
        {
            var pkgIntApp = this.configuration.GetConfiguration<ClientAppConfig>(ClientAppConfig.ConfigNamespaces.PkgIntClientApp);
            var AzureConfig = this.configuration.GetConfiguration<ApiClientConfig>(ApiClientConfig.ConfigNamespaces.Azure);

            var retryPolicy = new PipelineHttpPollyRetryPolicy(new RetryPolicyConfiguration(
                AzureConfig.RetryPolicy.NumberOfTries,
                AzureConfig.RetryPolicy.IncrementTime));

            AzureAuthentication workItemServiceApiAuthProvider = new AzureAuthentication(
                pkgIntApp.ClientId,
                pkgIntApp.CreateCert(),
                AzureConfig.ResourceId,
                AzureConfig.Authority);

            services.AddHttpClient<IAzureApiClient, AzureApiClient>()
                .ConfigureForWebService(
                    retryPolicy: retryPolicy,
                    baseUrl: AzureConfig.BaseUrl,
                    authenticationProvider: workItemServiceApiAuthProvider);
        }

        private async Task<SettingsModel> CreateSettingsModel(StatelessServiceContext serviceContext)
        {
            SettingsModel settings = await this.GetSettingsModel<SettingsModel>(serviceContext, "Config").ConfigureAwait(false);
            return settings;
        }

        private async Task<T> GetSettingsModel<T>(StatelessServiceContext serviceContext, string configName)
            where T : new()
        {
            IConfigurationData configData = new FabricConfigurationData(serviceContext.CodePackageActivationContext.GetConfigurationPackageObject(configName));
            ConfigurationRepository configRepo = new ConfigurationRepository(configData);

            T settings = new T();
            using (CancellationTokenSource cTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
            {
                await configRepo.LoadConfigurationModel(settings, cTokenSource.Token).ConfigureAwait(false);
            }

            return settings;
        }
    }
}
