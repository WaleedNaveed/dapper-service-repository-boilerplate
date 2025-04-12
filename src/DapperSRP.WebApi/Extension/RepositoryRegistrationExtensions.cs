using DapperSRP.Repository;

namespace DapperSRP.WebApi.Extension
{
    public static class RepositoryRegistrationExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services, string connectionString)
        {
            services.AddRepositoryServices();
            services.AddRepository(connectionString);
            return services;
        }
    }
}
