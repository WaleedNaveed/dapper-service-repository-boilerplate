using DapperSRP.Dto.User.CreateUser.Request;
using DapperSRP.Dto.User.ForgotPassword.Request;
using DapperSRP.Dto.User.Login.Request;
using DapperSRP.Dto.User.Login.Response;
using DapperSRP.Dto.User.RefreshToken.Request;
using DapperSRP.Dto.User.SetPassword.Request;

namespace DapperSRP.Service.Interface
{
    public interface IUserService
    {
        Task<ServiceResponse<bool>> CreateUserAsync(CreateUserRequest request);
        Task<ServiceResponse<bool>> SetPasswordAsync(SetPasswordRequest request);
        Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ServiceResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<ServiceResponse<bool>> LogoutAsync();
        Task<ServiceResponse<bool>> ForgotPassword(ForgotPasswordRequest request);
        Task<ServiceResponse<int>> GetNewUsersCountAsync(DateTime date);
    }
}
