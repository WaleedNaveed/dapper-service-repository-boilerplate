using DapperSRP.Persistence.Models;
using DapperSRP.Repository.Interface;

namespace DapperSRP.Repository.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            string query = "SELECT * FROM Users WHERE Email = @Email";
            return await _unitOfWork.QuerySingleOrDefaultAsync<User>(query, new { Email = email });
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            string query = "SELECT * FROM Users WHERE RefreshToken = @RefreshToken";
            return await _unitOfWork.QuerySingleOrDefaultAsync<User>(query, new { RefreshToken = refreshToken });
        }

        public async Task<User?> GetUserByTokenAsync(string token)
        {
            string query = "SELECT * FROM Users WHERE PasswordResetToken = @Token";
            return await _unitOfWork.QuerySingleOrDefaultAsync<User>(query, new { Token = token });
        }

        public async Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<int> userIds)
        {
            if (!userIds.Any()) return new List<User>();

            var query = "SELECT Id, Name FROM Users WHERE Id IN @Ids";
            return await _unitOfWork.QueryAsync<User>(query, new { Ids = userIds });
        }

        public async Task<int> GetNewUsersCountAsync(DateTime date)
        {
            var query = "SELECT COUNT(*) FROM Users WHERE CreatedAt >= @Date";
            return await _unitOfWork.ExecuteScalarAsync<int>(query, new { Date = date });
        }
    }
}
