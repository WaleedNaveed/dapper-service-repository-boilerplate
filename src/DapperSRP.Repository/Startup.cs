using Microsoft.Extensions.DependencyInjection;
using DapperSRP.Repository.Initialization;
using Microsoft.Data.SqlClient;
using DapperSRP.Repository.Interface;
using System.Data;

namespace DapperSRP.Repository
{
    public static class Startup
    {
        public static IServiceCollection AddRepository(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DbSeeder>();

            return services;
        }

        public static async Task DatabaseSeedingAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();

            await scope.ServiceProvider.GetRequiredService<DbSeeder>()
                .SeedDatabaseAsync();
        }
    }
}
