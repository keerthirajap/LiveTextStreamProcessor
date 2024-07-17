namespace LiveTextStreamProcessorWebApp.ActionFilter
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class LogExceptionAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<LogExceptionAttribute> _logger;

        public LogExceptionAttribute(ILogger<LogExceptionAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            // Log the exception details
            _logger.LogError(context.Exception, $"An exception occurred while processing request for {context.HttpContext.Request.Path}");

            // Optionally handle the exception or provide a different response
            context.Result = new StatusCodeResult(500); // Example: Return a 500 Internal Server Error response

            base.OnException(context);
        }
    }
}