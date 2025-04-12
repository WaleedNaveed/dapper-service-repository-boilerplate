namespace DapperSRP.Repository.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<bool> AddAsync(T entity);
        Task<int> AddAndReturnIdAsync(T entity);
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> UpdateAsync(T entity);
        Task<int> DeleteAsync(int id);
    }
}
