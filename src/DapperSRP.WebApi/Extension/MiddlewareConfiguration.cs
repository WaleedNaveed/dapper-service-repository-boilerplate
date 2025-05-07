using DapperSRP.WebApi.Middleware;

namespace DapperSRP.WebApi.Extension
{
    public static class MiddlewareConfiguration
    {
        public static void ConfigureAppMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMiddleware<JwtBlacklistMiddleware>();
            app.UseRateLimiter();
            app.UseAuthorization();
        }
    }
}
