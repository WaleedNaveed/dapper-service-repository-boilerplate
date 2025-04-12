using DapperSRP.Persistence.Models;

namespace DapperSRP.Service.Interface
{
    public interface IAuthService
    {
        Task<string> GenerateJwtToken(User request);
        string GenerateRefreshToken();
    }
}
