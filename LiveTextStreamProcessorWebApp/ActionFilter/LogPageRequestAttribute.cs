namespace LiveTextStreamProcessorWebApp.ActionFilter
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using NLog;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class LogPageRequestAttribute : ActionFilterAttribute
    {
        private readonly ILogger<LogPageRequestAttribute> _logger;

        public LogPageRequestAttribute(ILogger<LogPageRequestAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Log the request URL
            _logger.LogInformation($"Page requested: {context.HttpContext.Request.Path}");

            base.OnActionExecuting(context);
        }
    }
}