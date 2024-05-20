namespace NetCore.WorkItemService.Controllers
{
    using NetCore.WorkItemService.Dto;
    using NetCore.WorkItemService.Dto.External;
    using NetCore.WorkItemService.Handler.WorkItems;

    /// <summary>
    /// Controller class to handle WorkItem requests.
    /// </summary>
    [Route("api/v{version:apiVersion}/workitems")]
    [ApiVersion("1.0")]
    [ApiController]
    public class WorkItemServiceController : CoreActionApiController
    {
        private readonly IWorkItemHandler workItemHandler;
        private readonly ILogger<WorkItemServiceController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemServiceController"/> class.
        /// </summary>
        /// <param name="workItemHandler">The work item handler.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">ILogger.</param>
        public WorkItemServiceController(
            IWorkItemHandler workItemHandler,
            IServiceProvider serviceProvider,
            ILogger<WorkItemServiceController> logger)
            : base(serviceProvider, "workitems", "WorkItems", BaseStatelessService.StatelessContext)
        {
            this.workItemHandler = workItemHandler ?? throw new ArgumentNullException(nameof(workItemHandler));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>Get PackagingWorkItem</summary>
        /// <remarks>
        /// Returns PackagingWorkItem for specific work item id.
        /// </remarks>
        /// <group>WorkItem</group>
        /// <verb>GET</verb>
        /// <url>https://localhost/api/v1/workitems/{id}</url>
        /// <security type="http" name="http-bearer">
        ///     <description>AAD Role-Based Token. Reader Role required. See wiki.
        ///     <scheme>bearer</scheme>
        ///     <bearerFormat>JWT</bearerFormat>
        /// </security>
        /// <param name="id" in="path" cref="int">The work item id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200" cref="PackagingWorkItem">The PackagingWorkitem.</response>
        /// <response code="400">Error details for bad request.</response>
        /// <response code="404">WorkItem Id Not Found.</response>
        /// <returns>Returns the PackagingWorkItem.</returns>
        [HttpGet("{id}", Name = "GetWorkItem")]
        [Authorize(Policy = "Reader")]
        public async Task<ActionResult<PackagingWorkItem>> GetWorkItem([FromRoute] int id, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Querying for WorkItem: {Id}", id);
            // WorkItem Id is expected to be positive integer.
            if (id < 1)
            {
                return this.BadRequest("WorkItem Id must be valid!");
            }

            try
            {
                return await this.workItemHandler.GetWorkItem(id, cancellationToken)
                    .Match(error => this.ErrorResponse(error),
                           result => this.HandleResult(result, id))
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning("Error occurred! Error: {Exception}", ex);
                return this.StatusCode((int)HttpStatusCode.InternalServerError, $"Failed to query WorkItemId: {id}! Error: {ex.Message}");
            }
        }

        /// <summary>Get workitem payloads.</summary>
        /// <remarks>
        /// Get payloads specified in a workitem.
        /// If includeChildren is true, it will include payloads from all the child workitems associated with the same release.
        /// if includeBugList is true, it will include payloads from the workitem bug list associated with the given workitem.
        /// If both are true, then includeChildren takes precedence and includeBugList is ignored.
        /// </remarks>
        /// <group>WorkItem</group>
        /// <verb>GET</verb>
        /// <url>https://localhost/api/v1/workitems/{id}/payload</url>
        /// <security type="http" name="http-bearer">
        ///     <description>AAD Role-Based Token. Reader Role required. See wiki./wiki/Authentication_and_Authorization_for_Partner_Integration. </description>
        ///     <scheme>bearer</scheme>
        ///     <bearerFormat>JWT</bearerFormat>
        /// </security>
        /// <param name="id" in="path" cref="int">The work item id.</param>
        /// <param name="includeChildren" in="query" cref="bool">Include Children Payloads. By default it is set to false.</param>
        /// <param name="includeBugList" in="query" cref="bool">Include payloads from the workitem bug list associated with the given workitem. Only takes effect if includeChildren is false. By default includeBugList is set to false.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200" cref="WorkItemPayloads">The WorkItemPayloads.</response>
        /// <response code="400">Error details for bad request.</response>
        /// <response code="404">WorkItem Id Not Found.</response>
        /// <returns>Returns the WorkItemPayloads.</returns>
        [HttpGet("{id}/payload", Name = "GetWorkItemPayloads")]
        [Authorize(Policy = "Reader")]
        public async Task<ActionResult<WorkItemPayloads>> GetWorkItemPayloads(
            [FromRoute] int id,
            [FromQuery] bool includeChildren = false,
            [FromQuery] bool includeBugList = false,
            CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Querying payload for WorkItem: {Id} and IncludeChildren: {IncludeChildren}", id, includeChildren);
            if (id < 1)
            {
                return this.BadRequest($"WorkItem Id must an integer greater than 0 but was {id}.");
            }

            try
            {
                return await this.workItemHandler.GetWorkItemPayloads(id, includeChildren, includeBugList, cancellationToken)
                    .Match(error => this.ErrorResponse(error),
                           result => this.Ok(result))
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning("Error occurred! Error: {Exception}", ex);
                return this.StatusCode((int)HttpStatusCode.InternalServerError, $"Failed to query payload for WorkItemId: {id}! Error: {ex.Message}");
            }
        }

        /// <summary>Get the latest BaselineInfo.</summary>
        /// <remarks>
        /// Returns the latest BaselineInfo that contains the virtual RTM build and latest baseline lcu job id, declared before the input Release Ticket Id.
        /// </remarks>
        /// <group>WorkItem</group>
        /// <verb>GET</verb>
        /// <url>https://localhost/api/v1/workitems/{id}/baseline</url>
        /// <security type="http" name="http-bearer">
        ///     <description>AAD Role-Based Token. Reader Role required. See wiki./wiki/Authentication_and_Authorization_for_Partner_Integration. </description>
        ///     <scheme>bearer</scheme>
        ///     <bearerFormat>JWT</bearerFormat>
        /// </security>
        /// <param name="id" in="path" cref="int">The work item id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200" cref="BaselineInfo">The BaselineInfo.</response>
        /// <response code="400">Error details for bad request.</response>
        /// <response code="404">WorkItem Id Not Found.</response>
        /// <returns>Returns the latest BaselineInfo.</returns>
        [HttpGet("{id}/baseline", Name = "GetLatestBaselineInfo")]
        [Authorize(Policy = "Reader")]
        public async Task<ActionResult<BaselineInfo>> GetLatestBaselineInfo([FromRoute] int id, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Querying for WorkItem: {Id}", id);
            // WorkItem Id is expected to be positive integer.
            if (id < 1)
            {
                return this.BadRequest("WorkItem Id must be valid!");
            }

            try
            {
                return await this.workItemHandler.GetLatestBaselineInfo(id, cancellationToken)
                    .Match(error => this.ErrorResponse(error),
                           result => this.HandleResult(result, id))
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning("Error occurred! Error: {Exception}", ex);
                return this.StatusCode((int)HttpStatusCode.InternalServerError, $"Failed to query latest BaselineInfo for WorkItemId: {id}! Error: {ex.Message}");
            }
        }

        /// <summary>Get the Hotpatch Baseline.</summary>
        /// <remarks>
        /// Returns the Hotpatch Baseline for input Release Ticket Id.
        /// </remarks>
        /// <group>WorkItem</group>
        /// <verb>GET</verb>
        /// <url>https://localhost/api/v1/workitems/{id}/baseline/hotpatch</url>
        /// <security type="http" name="http-bearer">
        ///     <description>AAD Role-Based Token. Reader Role required. See wiki./wiki/Authentication_and_Authorization_for_Partner_Integration. </description>
        ///     <scheme>bearer</scheme>
        ///     <bearerFormat>JWT</bearerFormat>
        /// </security>
        /// <param name="id" in="path" cref="int">The work item id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200" cref="HotpatchBaseline">The HotpatchBaseline.</response>
        /// <response code="400">Error details for bad request.</response>
        /// <response code="404">WorkItem Id Not Found.</response>
        /// <returns>Returns the HotpatchBaseline.</returns>
        [HttpGet("{id}/baseline/hotpatch", Name = "GetHotpatchBaseline")]
        [Authorize(Policy = "Reader")]
        public async Task<ActionResult<HotpatchBaseline>> GetHotpatchBaseline([FromRoute] int id, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Querying for WorkItem: {Id}", id);
            // WorkItem Id is expected to be positive integer.
            if (id < 1)
            {
                return this.BadRequest("WorkItem Id must be valid!");
            }

            try
            {
                return await this.workItemHandler.GetHotpatchBaseline(id, cancellationToken)
                    .Match(error => this.ErrorResponse(error),
                           result => this.HandleResult(result, id))
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning("Error occurred! Error: {Exception}", ex);
                return this.StatusCode((int)HttpStatusCode.InternalServerError, $"Failed to query HotpatchBaseline for WorkItemId: {id}! Error: {ex.Message}");
            }
        }

        private ActionResult HandleResult<T>(T result, int workItemId)
        {
            if (result == null)
            {
                return this.StatusCode((int)HttpStatusCode.NotFound, $"{typeof(T)} not found for WorkItem Id: {workItemId}");
            }

            return this.Ok(result);
        }

        private ObjectResult ErrorResponse(StatusCodeError error)
        {
            if (error.StatusCode != HttpStatusCode.NotFound && error.StatusCode != HttpStatusCode.BadRequest)
            {
                this.logger.LogError("{StatusCode}: {ErrorMessage}", error.StatusCode, error.ErrorMessage);
            }

            return this.StatusCode((int)error.StatusCode, error.ErrorMessage);
        }

        private ActionResult HandleResult(PackagingWorkItems result, string releaseMonth, string product)
        {
            if (result == null)
            {
                return this.StatusCode((int)HttpStatusCode.NotFound, $"Unable to find release tickets for release month: {releaseMonth} and product {product}");
            }

            return this.Ok(result);
        }
    }
}
