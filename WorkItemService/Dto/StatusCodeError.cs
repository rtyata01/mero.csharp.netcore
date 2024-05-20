namespace NetCore.WorkItemService.Dto
{
    using System.Net;

    /// <summary>
    /// StatusCodeError class.
    /// </summary>
    public class StatusCodeError
    {
        /// <summary>
        /// The HTTP status code of the error.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
