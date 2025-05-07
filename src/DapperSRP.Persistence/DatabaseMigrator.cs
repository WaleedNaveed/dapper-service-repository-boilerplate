using DbUp;
using Microsoft.Data.SqlClient;
using DapperSRP.Common;
using DapperSRP.Common.Helper;

namespace DapperSRP.Persistence
{
    public class DatabaseMigrator
    {
        public static async Task RunMigrationsAsync(string connectionString)
        {
            var databaseName = ConnectionStringHelper.GetDatabaseName(connectionString);

            string masterConnectionString = connectionString.Replace(databaseName, DatabaseConstants.MasterDatabaseName);

            // Ensure Database Exists
            await EnsureDatabaseExistsAsync(masterConnectionString, databaseName);

            string migrationsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DatabaseConstants.MigrationsFolder);

            // Apply Migrations
            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(migrationsPath)
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw result.Error;
            }

            Console.WriteLine("Database Migrated Successfully!");
        }

        private static async Task EnsureDatabaseExistsAsync(string masterConnectionString, string databaseName)
        {
            await using var connection = new SqlConnection(masterConnectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                $"IF DB_ID('{databaseName}') IS NULL CREATE DATABASE {databaseName}", connection);

            await command.ExecuteNonQueryAsync();
        }
    }
}
