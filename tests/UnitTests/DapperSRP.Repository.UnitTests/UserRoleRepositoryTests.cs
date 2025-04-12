using Moq;
using DapperSRP.Persistence.Models;
using DapperSRP.Repository.Repository;
using DapperSRP.Repository.Interface;

namespace DapperSRP.Repository.UnitTests
{
    public class UserRoleRepositoryTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UserRoleRepository _userRoleRepository;

        public UserRoleRepositoryTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _userRoleRepository = new UserRoleRepository(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsUserRole_WhenUserIdExists()
        {
            // Arrange
            var userId = 1;
            var expectedUserRole = new UserRole { UserId = userId, RoleId = 2 };

            // Mocking the unit of work's QuerySingleOrDefaultAsync method
            _mockUnitOfWork
                .Setup(u => u.QuerySingleOrDefaultAsync<UserRole>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(expectedUserRole);

            // Act
            var result = await _userRoleRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUserRole.UserId, result.UserId);
            Assert.Equal(expectedUserRole.RoleId, result.RoleId);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsNull_WhenUserIdDoesNotExist()
        {
            // Arrange
            var userId = 1;

            // Mocking the unit of work's QuerySingleOrDefaultAsync method to return null
            _mockUnitOfWork
                .Setup(u => u.QuerySingleOrDefaultAsync<UserRole>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((UserRole)null);

            // Act
            var result = await _userRoleRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.Null(result);
        }
    }
}
