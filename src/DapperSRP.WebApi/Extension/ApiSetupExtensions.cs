using Asp.Versioning;

namespace DapperSRP.WebApi.Extension
{
    public static class ApiSetupExtensions
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddSwaggerSetup();
            return services;
        }
    }
}
