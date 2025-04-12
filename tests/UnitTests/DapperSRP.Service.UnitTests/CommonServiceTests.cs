using DapperSRP.Common;
using DapperSRP.Persistence.Models;
using DapperSRP.Repository.Interface;
using DapperSRP.Service.Exceptions;
using DapperSRP.Service.Interface;
using DapperSRP.Service.Service;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace DapperSRP.Service.UnitTests
{
    public class CommonServiceTests
    {
        private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;
        private readonly Mock<IRepository<Role>> _mockRoleRepository;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly ICommonService _commonService;

        public CommonServiceTests()
        {
            _mockUserRoleRepository = new Mock<IUserRoleRepository>();
            _mockRoleRepository = new Mock<IRepository<Role>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _commonService = new CommonService(
                _mockUserRoleRepository.Object,
                _mockRoleRepository.Object,
                _mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task GetUserRoleByUserId_ShouldReturnRole_WhenValidUserIdIsProvided()
        {
            // Arrange
            var userId = 1;
            var roleName = "Admin";

            // Setup the mock for GetByUserIdAsync to return a role ID
            _mockUserRoleRepository.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(new UserRole { RoleId = 1 });

            // Setup the mock for GetByIdAsync to return a role name
            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(new Role { Name = roleName });

            // Act
            var role = await _commonService.GetUserRoleByUserId(userId);

            // Assert
            Assert.Equal(roleName, role);
        }

        [Fact]
        public void GetLoggedInUserId_ShouldReturnUserId_WhenValidClaimExists()
        {
            // Arrange
            var userId = 1;
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            // Act
            var result = _commonService.GetLoggedInUserId();

            // Assert
            Assert.Equal(userId, result);
        }

        [Fact]
        public void GetLoggedInUserId_ShouldThrowUnAuthorizedException_WhenInvalidClaim()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            // Act & Assert
            var exception = Assert.Throws<UnAuthorizedException>(() => _commonService.GetLoggedInUserId());
            Assert.Equal(MessagesConstants.InvalidUserIdInToken, exception.Message);
        }
    }
}
