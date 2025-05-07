using DapperSRP.Common.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DapperSRP.Common
{
    public static class Startup
    {
        public static IServiceCollection AddConfig(this IServiceCollection services)
        {
            services.AddOptions<SuperAdminCredentialsConfig>()
                .BindConfiguration(ConfigKeys.SuperAdminCredentials);

            services.AddOptions<SmtpConfig>()
                .BindConfiguration(ConfigKeys.Smtp);

            services.AddOptions<JwtConfig>()
                .BindConfiguration(ConfigKeys.Jwt);

            services.AddOptions<RateLimitingConfig>()
                .BindConfiguration(ConfigKeys.RateLimiting);

            services.AddOptions<CorsConfig>()
                .BindConfiguration(ConfigKeys.Cors);

            return services;
        }
    }
}
