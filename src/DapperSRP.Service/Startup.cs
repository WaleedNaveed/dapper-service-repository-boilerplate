using DapperSRP.Service.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DapperSRP.Service
{
    public static class Startup
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services, string redisConnectionString)
        {
            // Redis Connection Setup
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            // Register services
            services.AddScoped<IJwtBlacklistService, JwtBlacklistService>();

            return services;
        }
    }
}
