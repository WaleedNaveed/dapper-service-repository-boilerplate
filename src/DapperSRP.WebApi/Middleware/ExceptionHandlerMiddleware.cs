using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Text;

namespace DapperSRP.WebApi.Middleware
{
    public class ExceptionHandlerMiddleware : IMiddleware
    {
        private readonly Service.Exceptions.IExceptionHandler _exceptionHandler;

        public ExceptionHandlerMiddleware(Service.Exceptions.IExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var exception = _exceptionHandler.HandleException(ex);

                var requestBody = await ReadRequestBody(context);

                // Log Exception with Request Body and Stack Trace
                Log.Error(ex, "Exception occurred! Request: {Method} {Path} | Body: {RequestBody} | StackTrace: {StackTrace}",
                    context.Request.Method, context.Request.Path, requestBody, ex.StackTrace);

                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = exception.ErrorCode;
                    var jsonSerializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(exception, jsonSerializerSettings));
                }
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
    }
}
