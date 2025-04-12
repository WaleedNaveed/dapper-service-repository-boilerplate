using System.Data.SqlClient;

namespace DapperSRP.Common.Helper
{
    public static class ConnectionStringHelper
    {
        public static string GetDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }
    }
}
