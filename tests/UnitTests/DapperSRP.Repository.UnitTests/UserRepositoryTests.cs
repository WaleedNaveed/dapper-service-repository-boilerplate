using Moq;
using DapperSRP.Persistence.Models;
using DapperSRP.Repository.Repository;
using DapperSRP.Repository.Interface;

namespace DapperSRP.Repository.UnitTests
{
    public class UserRepositoryTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _userRepository = new UserRepository(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsUser_WhenEmailExists()
        {
            // Arrange
            var email = "test@example.com";
            var expectedUser = new User { Email = email, Id = 1, Name = "Test User" };

            _mockUnitOfWork
                .Setup(u => u.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userRepository.GetByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Email, result.Email);
            Assert.Equal(expectedUser.Id, result.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNull_WhenEmailDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";

            _mockUnitOfWork
                .Setup(u => u.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userRepository.GetByEmailAsync(email);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ReturnsUser_WhenTokenExists()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var expectedUser = new User { RefreshToken = refreshToken, Id = 1, Name = "Test User" };

            _mockUnitOfWork
                .Setup(u => u.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.RefreshToken, result.RefreshToken);
            Assert.Equal(expectedUser.Id, result.Id);
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ReturnsNull_WhenTokenDoesNotExist()
        {
            // Arrange
            var refreshToken = "invalid-refresh-token";

            _mockUnitOfWork
                .Setup(u => u.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByTokenAsync_ReturnsUser_WhenTokenExists()
        {
            // Arrange
            var token = "valid-password-reset-token";
            var expectedUser = new User { PasswordResetToken = token, Id = 1, Name = "Test User" };

            _mockUnitOfWork
                .Setup(u => u.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userRepository.GetUserByTokenAsync(token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.PasswordResetToken, result.PasswordResetToken);
            Assert.Equal(expectedUser.Id, result.Id);
        }

        [Fact]
        public async Task GetUserByTokenAsync_ReturnsNull_WhenTokenDoesNotExist()
        {
            // Arrange
            var token = "invalid-password-reset-token";

            _mockUnitOfWork
                .Setup(u => u.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userRepository.GetUserByTokenAsync(token);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUsersByIdsAsync_ReturnsUsers_WhenIdsExist()
        {
            // Arrange
            var userIds = new List<int> { 1, 2, 3 };
            var expectedUsers = new List<User>
            {
                new User { Id = 1, Name = "User 1" },
                new User { Id = 2, Name = "User 2" },
                new User { Id = 3, Name = "User 3" }
            };

            _mockUnitOfWork
                .Setup(u => u.QueryAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(expectedUsers);

            // Act
            var result = await _userRepository.GetUsersByIdsAsync(userIds);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUsers.Count, result.Count());
            Assert.Equal(expectedUsers[0].Id, result.ElementAt(0).Id);
            Assert.Equal(expectedUsers[1].Id, result.ElementAt(1).Id);
            Assert.Equal(expectedUsers[2].Id, result.ElementAt(2).Id);
        }

        [Fact]
        public async Task GetUsersByIdsAsync_ReturnsEmpty_WhenNoIds()
        {
            // Arrange
            var userIds = new List<int>();

            // Act
            var result = await _userRepository.GetUsersByIdsAsync(userIds);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesUserAndReturnsAffectedRows()
        {
            // Arrange
            var user = new User { Id = 1, Name = "Updated User", Email = "updated@example.com" };
            var affectedRows = 1;

            _mockUnitOfWork
                .Setup(u => u.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(affectedRows);

            // Act
            var result = await _userRepository.UpdateAsync(user);

            // Assert
            Assert.Equal(affectedRows, result);
        }
    }
}
