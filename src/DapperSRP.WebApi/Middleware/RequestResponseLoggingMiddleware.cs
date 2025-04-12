using Serilog;
using System.Text;

namespace DapperSRP.WebApi.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Read and log request body
            var requestBody = await ReadRequestBody(context);
            Log.Information("Request: {Method} {Path} | Body: {RequestBody}", context.Request.Method, context.Request.Path, requestBody);

            // Capture response
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                await _next(context);
                var responseBodyText = await ReadResponseBody(context);
                Log.Information("Response: {StatusCode} | Body: {ResponseBody}", context.Response.StatusCode, responseBodyText);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> ReadRequestBody(HttpContext context)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            return body;
        }

        private async Task<string> ReadResponseBody(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }
}
