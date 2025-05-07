using System.Threading.RateLimiting;
using DapperSRP.Common.Configuration;
using Microsoft.Extensions.Options;

namespace DapperSRP.WebApi.Extension
{
    public static class RateLimitingSetup
    {
        public static void AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                var perUserInfo = services.BuildServiceProvider().GetRequiredService<IOptions<RateLimitingConfig>>().Value.PerUser;

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    return RateLimitPartition.GetTokenBucketLimiter(
                        "global",
                        _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = perUserInfo.TokenLimit,
                            QueueLimit = perUserInfo.QueueLimit,
                            TokensPerPeriod = perUserInfo.TokensPerMinute,
                            ReplenishmentPeriod = TimeSpan.FromMinutes(perUserInfo.ReplenishmentPeriodMinutes),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                });

                // Custom response for 429
                options.OnRejected = (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    return new ValueTask();
                };
            });
        }
    }
}
