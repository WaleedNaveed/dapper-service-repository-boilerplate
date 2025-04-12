using Microsoft.AspNetCore.Mvc;
using Moq;
using DapperSRP.Service.Interface;
using DapperSRP.WebApi.Controllers;
using DapperSRP.Dto.Role.Response;

namespace DapperSRP.Api.UnitTests
{
    public class RoleControllerTests
    {
        private readonly Mock<IRoleService> _mockRoleService;
        private readonly RoleController _controller;

        public RoleControllerTests()
        {
            _mockRoleService = new Mock<IRoleService>();
            _controller = new RoleController(_mockRoleService.Object);
        }

        #region GetAllRoles

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithRoles()
        {
            // Arrange
            var mockRoles = new List<GetRoleResponse>
            {
                new GetRoleResponse { Id = 1, Name = "Admin" },
                new GetRoleResponse { Id = 2, Name = "User" }
            };

            _mockRoleService.Setup(service => service.GetAllRolesAsync())
                            .ReturnsAsync(mockRoles);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<GetRoleResponse>>(okResult.Value);
            Assert.Equal(mockRoles.Count, response.Count);
        }


        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenNoRolesExist()
        {
            // Arrange
            var emptyList = new List<GetRoleResponse>();
            _mockRoleService.Setup(service => service.GetAllRolesAsync())
                           .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<GetRoleResponse>>(okResult.Value);
            Assert.Empty(response);
        }

        #endregion

        #region GetRoleById

        [Fact]
        public async Task GetById_ReturnsOkResult_WithValidRole()
        {
            // Arrange
            var roleId = 1;
            var mockRole = new GetRoleResponse
            {
                Id = roleId,
                Name = "Admin"
            };

            _mockRoleService.Setup(service => service.GetRoleByIdAsync(roleId))
                            .ReturnsAsync(mockRole);

            // Act
            var result = await _controller.GetById(roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GetRoleResponse>(okResult.Value);
            Assert.Equal(mockRole.Id, response.Id);
        }

        #endregion
    }
}
