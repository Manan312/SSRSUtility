using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using NLog;
using System.Text.Json;

namespace Infrastructure.Middleware
{
    public class ExceptionMiddleware
    {
        private static readonly ILogger _errorLogger =
            LogManager.GetLogger("ErrorLog");

        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
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

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            int statusCode;
            string message;

            if (exception is AppException appException)
            {
                statusCode = appException.StatusCode;
                message = appException.Message;
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                message = "An unexpected error occurred.";
            }

            // 🔴 Log only server errors
            if (statusCode >= 500)
            {
                _errorLogger.Error(exception,
                    "Unhandled exception | Path: {0} | Method: {1} | TraceId: {2}",
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier);
            }

            var response = new 
            {
                Success = false,
                Message = message,
                TraceId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
