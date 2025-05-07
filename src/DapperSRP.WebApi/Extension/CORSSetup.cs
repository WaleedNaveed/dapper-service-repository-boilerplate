using DapperSRP.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class CORSSetup
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var corsConfig = serviceProvider.GetRequiredService<IOptions<CorsConfig>>().Value;
        var allowedOrigins = corsConfig.AllowedOrigins?.ToArray() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                if (allowedOrigins.Length > 0)
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                }
                else
                {
                    // If no origins are specified, deny all CORS requests
                    builder.SetIsOriginAllowed(_ => false);
                }
            });
        });

        return services;
    }
}