using DapperSRP.Repository.Interface;
using System.Data;

namespace DapperSRP.Repository.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;

        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddAsync(T entity)
        {
            var query = $"INSERT INTO {typeof(T).Name}s ({GetColumns()}) VALUES ({GetValues()})";
            var rowsAffected = await _unitOfWork.ExecuteAsync(query, entity);
            return rowsAffected > 0;
        }

        public async Task<int> AddAndReturnIdAsync(T entity)
        {
            var query = $"INSERT INTO {typeof(T).Name}s ({GetColumns()}) VALUES ({GetValues()}); SELECT CAST(SCOPE_IDENTITY() as int);";
            var newId = await _unitOfWork.QuerySingleOrDefaultAsync<int>(query, entity);
            entity.GetType().GetProperty("Id")?.SetValue(entity, newId); // Set ID in entity
            return newId;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var query = $"SELECT * FROM {typeof(T).Name}s WHERE Id = @Id";
            return await _unitOfWork.QuerySingleOrDefaultAsync<T>(query, new { Id = id });
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = $"SELECT * FROM {typeof(T).Name}s";
            return await _unitOfWork.QueryAsync<T>(query);
        }

        public async Task<int> UpdateAsync(T entity)
        {
            var query = $"UPDATE {typeof(T).Name}s SET {GetUpdateSet()} WHERE Id = @Id";
            return await _unitOfWork.ExecuteAsync(query, entity);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query = $"DELETE FROM {typeof(T).Name}s WHERE Id = @Id";
            return await _unitOfWork.ExecuteAsync(query, new { Id = id });
        }

        private string GetColumns()
            => string.Join(", ", typeof(T).GetProperties()
                .Where(p => !IsIdentityColumn(p.Name))
                .Select(p => p.Name));

        private string GetValues()
            => string.Join(", ", typeof(T).GetProperties()
                .Where(p => !IsIdentityColumn(p.Name))
                .Select(p => "@" + p.Name));

        private string GetUpdateSet() => string.Join(", ", typeof(T).GetProperties().Where(p => p.Name != "Id").Select(p => $"{p.Name} = @{p.Name}"));

        private bool IsIdentityColumn(string propertyName)
        {
            return propertyName.Equals("ID", StringComparison.OrdinalIgnoreCase);
        }
    }
}
