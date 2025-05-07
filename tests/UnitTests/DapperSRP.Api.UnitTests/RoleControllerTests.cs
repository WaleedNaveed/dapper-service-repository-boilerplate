using Microsoft.AspNetCore.Mvc;
using Moq;
using DapperSRP.Service.Interface;
using DapperSRP.WebApi.Controllers;
using DapperSRP.Dto.Role.Response;
using DapperSRP.Service;

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
                            .ReturnsAsync(new ServiceResponse<List<GetRoleResponse>>
                            {
                                HasError = false,
                                Result = mockRoles
                            });

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var serviceResponse = Assert.IsType<ServiceResponse<List<GetRoleResponse>>>(okResult.Value);
            Assert.False(serviceResponse.HasError);
            var roles = Assert.IsType<List<GetRoleResponse>>(serviceResponse.Result);
            Assert.Equal(mockRoles.Count, roles.Count);
        }


        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenNoRolesExist()
        {
            // Arrange
            var emptyList = new List<GetRoleResponse>();
            _mockRoleService.Setup(service => service.GetAllRolesAsync())
                           .ReturnsAsync(new ServiceResponse<List<GetRoleResponse>>
                           {
                               HasError = false,
                               Result = emptyList
                           });

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var serviceResponse = Assert.IsType<ServiceResponse<List<GetRoleResponse>>>(okResult.Value);
            Assert.False(serviceResponse.HasError);
            var roles = Assert.IsType<List<GetRoleResponse>>(serviceResponse.Result);
            Assert.Empty(roles);
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
                            .ReturnsAsync(new ServiceResponse<GetRoleResponse>
                            {
                                HasError = false,
                                Result = mockRole
                            });

            // Act
            var result = await _controller.GetById(roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var serviceResponse = Assert.IsType<ServiceResponse<GetRoleResponse>>(okResult.Value);
            Assert.False(serviceResponse.HasError);
            var role = Assert.IsType<GetRoleResponse>(serviceResponse.Result);
            Assert.Equal(mockRole.Id, role.Id);
        }

        #endregion
    }
}
