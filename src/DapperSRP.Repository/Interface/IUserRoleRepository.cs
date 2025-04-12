using DapperSRP.Persistence.Models;

namespace DapperSRP.Repository.Interface
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task<UserRole> GetByUserIdAsync(int userId);
    }
}
