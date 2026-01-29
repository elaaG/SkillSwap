using SkillSwap.API.Exceptions;
using System.Net;
using System.Text.Json;

namespace SkillSwap.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }
        
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred";
            string? details = null;
            
            switch (exception)
            {
                case SkillSwapException customEx:
                    statusCode = (HttpStatusCode)customEx.StatusCode;
                    message = customEx.Message;
                    break;
                    
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                    
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Unauthorized access";
                    break;
                    
                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                    
                default:
                    details = exception.Message;
                    break;
            }
            
            var response = new ErrorResponse
            {
                StatusCode = (int)statusCode,
                Message = message,
                Details = details,
                TraceId = context.TraceIdentifier
            };
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}