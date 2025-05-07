using DapperSRP.Persistence.Models;
using DapperSRP.Repository.Interface;

namespace DapperSRP.Repository.Repository
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserRoleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserRole> GetByUserIdAsync(int userId)
        {
            string query = "SELECT * FROM UserRoles WHERE UserId = @UserId";
            return await _unitOfWork.QuerySingleOrDefaultAsync<UserRole>(query, new { UserId = userId });
        }
    }
}
