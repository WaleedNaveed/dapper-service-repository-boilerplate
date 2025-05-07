using DapperSRP.Common;
using DapperSRP.Common.Configuration;
using DapperSRP.Dto.User.ForgotPassword.Request;
using DapperSRP.Dto.User.Login.Request;
using DapperSRP.Dto.User.RefreshToken.Request;
using DapperSRP.Dto.User.SetPassword.Request;
using DapperSRP.Logging;
using DapperSRP.Persistence.Models;
using DapperSRP.Repository.Interface;
using DapperSRP.Service.Exceptions;
using DapperSRP.Service.Interface;
using DapperSRP.Service.Redis;
using DapperSRP.Service.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace DapperSRP.Service.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;
        private readonly Mock<IRepository<Role>> _mockRoleRepository;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IEmailTemplateService> _mockEmailTemplateService;
        private readonly Mock<ICommonService> _mockCommonService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IOptions<JwtConfig>> _mockJwt;
        private readonly Mock<IJwtBlacklistService> _mockJwtBlacklistService;
        private readonly Mock<ILoggerService<UserService>> _mockLogger;

        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserRoleRepository = new Mock<IUserRoleRepository>();
            _mockRoleRepository = new Mock<IRepository<Role>>();
            _mockAuthService = new Mock<IAuthService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockEmailTemplateService = new Mock<IEmailTemplateService>();
            _mockCommonService = new Mock<ICommonService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockJwt = new Mock<IOptions<JwtConfig>>();
            _mockJwtBlacklistService = new Mock<IJwtBlacklistService>();
            _mockLogger = new Mock<ILoggerService<UserService>>();

            _userService = new UserService(
                _mockUserRepository.Object,
                _mockUserRoleRepository.Object,
                _mockRoleRepository.Object,
                _mockAuthService.Object,
                _mockEmailService.Object,
                _mockEmailTemplateService.Object,
                _mockCommonService.Object,
                _mockUnitOfWork.Object,
                _mockConfiguration.Object,
                _mockJwt.Object,
                _mockJwtBlacklistService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task SetPasswordAsync_ShouldThrowBadRequestException_WhenTokenIsInvalidOrExpired()
        {
            // Arrange
            var request = new SetPasswordRequest { Token = "invalidToken", Password = "newPassword" };

            _mockUserRepository.Setup(r => r.GetUserByTokenAsync(request.Token)).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _userService.SetPasswordAsync(request));
            Assert.Equal(MessagesConstants.InvalidOrExpiredToken, exception.Message);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowBadRequestException_WhenCredentialsAreInvalid()
        {
            // Arrange
            var request = new LoginRequest { Email = "john@example.com", Password = "wrongPassword" };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(new User { Password = "hashedPassword" });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _userService.LoginAsync(request));
            Assert.Equal(MessagesConstants.InvalidCredentials, exception.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnAuthorizedException_WhenRefreshTokenIsExpired()
        {
            // Arrange
            var request = new RefreshTokenRequest { RefreshToken = "expiredToken" };

            _mockUserRepository.Setup(r => r.GetByRefreshTokenAsync(request.RefreshToken)).ReturnsAsync(new User { RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1) });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnAuthorizedException>(() => _userService.RefreshTokenAsync(request));
            Assert.Equal(MessagesConstants.InvalidOrExpiredRefreshToken, exception.Message);
        }

        [Fact]
        public async Task ForgotPassword_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new ForgotPasswordRequest { Email = "nonexistent@example.com" };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.ForgotPassword(request));
            Assert.Equal(MessagesConstants.UserDoesNotExist, exception.Message);
        }

        [Fact]
        public async Task LogoutAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            _mockCommonService.Setup(c => c.GetLoggedInUserId()).Returns(1);
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.LogoutAsync());
            Assert.Equal(MessagesConstants.UserNotFound, exception.Message);
        }
    }
}
