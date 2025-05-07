using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DapperSRP.Common;
using DapperSRP.Common.Configuration;
using DapperSRP.Persistence.Models;
using DapperSRP.Repository.Interface;
using DapperSRP.Service.EmailTemplates.Models;
using DapperSRP.Service.Exceptions;
using DapperSRP.Service.Interface;
using DapperSRP.Logging;
using DapperSRP.Service.Redis;
using DapperSRP.Dto.User.SetPassword.Request;
using DapperSRP.Dto.User.CreateUser.Request;
using DapperSRP.Dto.User.ForgotPassword.Request;
using DapperSRP.Dto.User.Login.Request;
using DapperSRP.Dto.User.Login.Response;
using DapperSRP.Dto.User.RefreshToken.Request;

namespace DapperSRP.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IAuthService _authService;
        private readonly ICommonService _commonService;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IOptions<JwtConfig> _jwt;
        private readonly IJwtBlacklistService _jwtBlacklistService;

        public UserService(IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IRepository<Role> roleRepository,
            IAuthService authService,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            ICommonService commonService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IOptions<JwtConfig> jwt,
            IJwtBlacklistService jwtBlacklistService,
            ILoggerService<UserService> logger)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _authService = authService;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _commonService = commonService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _jwt = jwt;
            _jwtBlacklistService = jwtBlacklistService;
        }

        public async Task<ServiceResponse<bool>> CreateUserAsync(CreateUserRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(request.Role);
            if (role == null || role.Name == Roles.SuperAdmin)
            {
                throw new BadRequestException(MessagesConstants.InvalidRole);
            }

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new BadRequestException(MessagesConstants.EmailAlreadyRegistered);
            }

            var passwordSetToken = GeneratePasswordSetToken();
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                IsEmailConfirmed = false,
                PasswordResetToken = passwordSetToken,
                PasswordResetExpiry = GetPasswordSetLinkExpiry(),
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.BeginTransaction();

            try
            {
                int userId = await _userRepository.AddAndReturnIdAsync(user);
                if (userId <= 0)
                {
                    throw new BadRequestException(MessagesConstants.UserCreationFailed);
                }

                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = request.Role
                };
                await _userRoleRepository.AddAsync(userRole);

                string passwordSetLink = GeneratePasswordSetLink(passwordSetToken);
                var emailModel = new PasswordSetEmailModel
                {
                    Name = user.Name,
                    Link = passwordSetLink,
                };
                var emailBody = _emailTemplateService.RenderTemplate<PasswordSetEmailModel>(EmailTemplateConstants.PasswordSetEmail, emailModel);

                await _emailService.SendEmailAsync(request.Email, Constants.PasswordSetEmailSubject, emailBody);

                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }

            return new ServiceResponse<bool> { HasError = false, Result = true };
        }

        public async Task<ServiceResponse<bool>> SetPasswordAsync(SetPasswordRequest request)
        {
            // Get user by token
            var user = await _userRepository.GetUserByTokenAsync(request.Token);
            if (user == null)
            {
                throw new BadRequestException(MessagesConstants.InvalidOrExpiredToken);
            }

            if (user.PasswordResetExpiry == null || user.PasswordResetExpiry < DateTime.UtcNow)
            {
                throw new BadRequestException(MessagesConstants.LinkHasBeenExpired);
            }

            // Hash new password
            string hashedPassword = PasswordHasher.HashPassword(request.Password);

            _unitOfWork.BeginTransaction();

            try
            {
                user.Password = hashedPassword;
                user.PasswordResetToken = null;
                user.PasswordResetExpiry = null;
                if (!user.IsEmailConfirmed)
                    user.IsEmailConfirmed = true;

                await _userRepository.UpdateAsync(user);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }

            return new ServiceResponse<bool> { HasError = false, Result = true };
        }

        public async Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var enteredHashPassword = PasswordHasher.HashPassword(request.Password);

            var u = await _userRepository.GetByEmailAsync(request.Email);

            if (u == null || u.Password != enteredHashPassword)
            {
                throw new BadRequestException(MessagesConstants.InvalidCredentials);
            }
            var accessToken = await _authService.GenerateJwtToken(u);
            var refreshToken = _authService.GenerateRefreshToken();

            u.RefreshToken = refreshToken;
            u.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(_jwt.Value.RefreshTokenExpiryMinutes);

            await _userRepository.UpdateAsync(u);

            var role = await _commonService.GetUserRoleByUserId(u.Id);

            return new ServiceResponse<LoginResponse>()
            {
                HasError = false,
                Result = new LoginResponse()
                { AccessToken = accessToken, RefreshToken = refreshToken, Role = role }
            };
        }

        public async Task<ServiceResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                throw new UnAuthorizedException(MessagesConstants.InvalidOrExpiredRefreshToken);
            }

            var newAccessToken = await _authService.GenerateJwtToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(_jwt.Value.RefreshTokenExpiryMinutes);

            await _userRepository.UpdateAsync(user);

            var role = await _commonService.GetUserRoleByUserId(user.Id);

            return new ServiceResponse<LoginResponse>()
            {
                HasError = false,
                Result = new LoginResponse()
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Role = role
                }
            };
        }

        public async Task<ServiceResponse<bool>> LogoutAsync()
        {
            var loggedInUserId = _commonService.GetLoggedInUserId();

            var user = await _userRepository.GetByIdAsync(loggedInUserId);

            if (user == null)
            {
                throw new NotFoundException(MessagesConstants.UserNotFound);
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userRepository.UpdateAsync(user);

            var token = _commonService.GetJwtLoggedInUser();
            await _jwtBlacklistService.BlacklistTokenAsync(token);

            return new ServiceResponse<bool>() { HasError = false, Result = true };
        }

        public async Task<ServiceResponse<bool>> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new NotFoundException(MessagesConstants.UserDoesNotExist);
            }

            var passwordResetToken = GeneratePasswordSetToken();
            user.PasswordResetToken = passwordResetToken;
            user.PasswordResetExpiry = GetPasswordSetLinkExpiry();

            await _userRepository.UpdateAsync(user);

            string passwordResetLink = GeneratePasswordSetLink(passwordResetToken);
            var emailModel = new PasswordResetEmailModel
            {
                Name = user.Name,
                Link = passwordResetLink,
            };
            var emailBody = _emailTemplateService.RenderTemplate<PasswordResetEmailModel>(EmailTemplateConstants.PasswordResetEmail, emailModel);
            await _emailService.SendEmailAsync(request.Email, Constants.PasswordResetEmailSubject, emailBody);

            return new ServiceResponse<bool>() { HasError = false, Result = true };

        }

        public async Task<ServiceResponse<int>> GetNewUsersCountAsync(DateTime date)
        {
            var count = await _userRepository.GetNewUsersCountAsync(date);
            return new ServiceResponse<int>() { HasError = false, Result = count };
        }

        private string GeneratePasswordSetToken()
        {
            return Guid.NewGuid().ToString();
        }

        private string GeneratePasswordSetLink(string token)
        {
            var appUrl = _configuration[ConfigKeys.AppUrl];
            var link = $"{appUrl}set-password?token={token}";

            return link;
        }

        private DateTime GetPasswordSetLinkExpiry()
        {
            if (!int.TryParse(_configuration[ConfigKeys.SetPasswordLinkExpiryMinutes], out int expiryMinutes))
            {
                expiryMinutes = 60; // Default value if parsing fails
            }
            DateTime passwordSetLinkExpiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

            return passwordSetLinkExpiry;
        }
    }
}
