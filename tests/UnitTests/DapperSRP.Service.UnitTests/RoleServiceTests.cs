using AutoMapper;
using DapperSRP.Common;
using DapperSRP.Dto.Role.Response;
using DapperSRP.Repository.Interface;
using DapperSRP.Service.Exceptions;
using DapperSRP.Service.Service;
using Moq;

namespace DapperSRP.Service.UnitTests
{
    public class RoleServiceTests
    {
        private readonly Mock<IRepository<DapperSRP.Persistence.Models.Role>> _mockRoleRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly RoleService _roleService;

        public RoleServiceTests()
        {
            _mockRoleRepository = new Mock<IRepository<DapperSRP.Persistence.Models.Role>>();
            _mockMapper = new Mock<IMapper>();
            _roleService = new RoleService(_mockRoleRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllRolesAsync_ShouldReturnRolesExcludingSuperAdmin()
        {
            // Arrange
            var roles = new List<DapperSRP.Persistence.Models.Role>
            {
                new DapperSRP.Persistence.Models.Role { Id = 1, Name = "Admin" },
                new DapperSRP.Persistence.Models.Role { Id = 2, Name = "User" },
                new DapperSRP.Persistence.Models.Role { Id = 3, Name = "SuperAdmin" }
            };

            _mockRoleRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(roles);
            _mockMapper.Setup(mapper => mapper.Map<List<GetRoleResponse>>(It.IsAny<IEnumerable<DapperSRP.Persistence.Models.Role>>()))
                       .Returns(new List<GetRoleResponse> { new GetRoleResponse { Id = 1, Name = "Admin" }, new GetRoleResponse { Id = 2, Name = "User" } });

            // Act
            var result = await _roleService.GetAllRolesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Result.Count);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ShouldReturnRole_WhenRoleExists()
        {
            // Arrange
            var role = new DapperSRP.Persistence.Models.Role { Id = 1, Name = "Admin" };
            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(role);
            _mockMapper.Setup(mapper => mapper.Map<GetRoleResponse>(It.IsAny<DapperSRP.Persistence.Models.Role>()))
                       .Returns(new GetRoleResponse { Id = 1, Name = "Admin" });

            // Act
            var result = await _roleService.GetRoleByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Result.Id);
            Assert.Equal("Admin", result.Result.Name);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ShouldThrowNotFoundException_WhenRoleDoesNotExist()
        {
            // Arrange
            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((DapperSRP.Persistence.Models.Role)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _roleService.GetRoleByIdAsync(99));
            Assert.Equal(MessagesConstants.RoleWithTheProvidedIdDoesNotExist, exception.Message);
        }
    }
}
