using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using SkillForge.Shared.Results;

namespace SkillForge.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResult = new
            {
                success = false,
                error = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An internal server error occurred. Please try again later."
            };

            if (_environment.IsDevelopment())
            {
                var devErrorDetails = new
                {
                    success = false,
                    error = exception.Message,
                    stackTrace = exception.StackTrace,
                    innerException = exception.InnerException?.Message
                };
                
                await response.WriteAsync(JsonSerializer.Serialize(devErrorDetails));
                return;
            }

            await response.WriteAsync(JsonSerializer.Serialize(errorResult));
        }
    }
}
