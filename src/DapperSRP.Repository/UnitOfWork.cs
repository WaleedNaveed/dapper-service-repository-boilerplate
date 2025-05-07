using Dapper;
using DapperSRP.Repository.Interface;
using System.Data;

namespace DapperSRP.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction? _transaction;
        private bool _disposed;

        public UnitOfWork(IDbConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        public IDbConnection Connection => _connection;
        public IDbTransaction? Transaction => _transaction;

        public void BeginTransaction()
        {
            if (_transaction == null)
            {
                _transaction = _connection.BeginTransaction();
            }
        }

        public void Commit()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            return await _connection.ExecuteAsync(sql, param, _transaction);
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null)
        {
            return await _connection.ExecuteScalarAsync<T>(sql, param, _transaction);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            return await _connection.QueryAsync<T>(sql, param);
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
        {
            return await _connection.QuerySingleOrDefaultAsync<T>(sql, param, _transaction);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _transaction?.Dispose();
            _connection?.Dispose();

            _disposed = true;
        }
    }
}
