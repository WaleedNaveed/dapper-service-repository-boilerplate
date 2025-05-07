using DapperSRP.Service.Redis;

namespace DapperSRP.WebApi.Middleware
{
    public class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public JwtBlacklistMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var jwtBlacklistService = scope.ServiceProvider.GetRequiredService<IJwtBlacklistService>();

                    if (await jwtBlacklistService.IsTokenBlacklistedAsync(token))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("You are logged out, please log in again.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
