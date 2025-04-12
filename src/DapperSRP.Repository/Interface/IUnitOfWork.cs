using System.Data;

namespace DapperSRP.Repository.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();
        Task<int> ExecuteAsync(string sql, object? param = null);
        Task<T> ExecuteScalarAsync<T>(string sql, object? param = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
        Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null);
    }
}
