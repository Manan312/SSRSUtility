using Core.Entities;
using Microsoft.AspNetCore.Http;
using Infrastructure.Attributes;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Middleware
{
    public class ResponseMiddleware
    {
        private readonly RequestDelegate _next;
        public ResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<SkipResponseMiddlewareAttribute>() != null)
            {
                await _next(context);
                return;
            }
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                if (context.Response.HasStarted)
                    return;

                // Skip non-JSON or empty responses
                if (context.Response.StatusCode >= 400)
                    return;

                responseBody.Seek(0, SeekOrigin.Begin);
                var bodyText = await new StreamReader(responseBody).ReadToEndAsync();
                object? data;

                // ✅ Only deserialize if valid JSON
                if (IsJson(context.Response.ContentType, bodyText))
                {
                    data = JsonSerializer.Deserialize<object>(bodyText);
                }
                else
                {
                    // Plain text or other formats
                    data = bodyText;
                }
                var wrappedResponse = new APIResponse<object>
                {
                    StatusCode = context.Response.StatusCode,
                    Data = string.IsNullOrWhiteSpace(bodyText)
                        ? null
                        : data,
                    TraceId = context.TraceIdentifier
                };

                var json = JsonSerializer.Serialize(wrappedResponse);

                context.Response.ContentType = "application/json";
                context.Response.ContentLength = Encoding.UTF8.GetByteCount(json);

                context.Response.Body = originalBodyStream;
                await context.Response.WriteAsync(json);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
        private static bool IsJson(string? contentType, string body)
        {
            if (!string.IsNullOrWhiteSpace(contentType) &&
                contentType.Contains("application/json"))
                return true;

            body = body.Trim();
            return body.StartsWith("{") || body.StartsWith("[");
        }
    }
}
