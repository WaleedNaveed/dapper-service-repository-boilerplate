using DapperSRP.Persistence.Models;

namespace DapperSRP.Repository.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task<User?> GetUserByTokenAsync(string token);
        Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<int> userIds);
        Task<int> GetNewUsersCountAsync(DateTime date);
    }
}
