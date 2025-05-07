using Microsoft.AspNetCore.Mvc;
using Moq;
using DapperSRP.WebApi.Controllers;
using DapperSRP.Service.Interface;
using DapperSRP.Service;
using DapperSRP.Dto.User.SetPassword.Request;
using DapperSRP.Dto.User.CreateUser.Request;
using DapperSRP.Dto.User.ForgotPassword.Request;
using DapperSRP.Dto.User.Login.Request;
using DapperSRP.Dto.User.Login.Response;
using DapperSRP.Dto.User.RefreshToken.Request;

namespace DapperSRP.Api.UnitTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
        }

        [Fact]
        public async Task CreateUser_ReturnsOkResult_WithValidRequest()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Name = "johndoe",
                Email = "johndoe@example.com",
                Role = 1
            };

            var mockResponse = new ServiceResponse<bool>
            {
                HasError = false,
                Result = true
            };

            _mockUserService.Setup(service => service.CreateUserAsync(It.IsAny<CreateUserRequest>()))
                            .ReturnsAsync(mockResponse);

            var controller = new UserController(_mockUserService.Object);

            // Act
            var result = await controller.CreateUser(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<bool>>(okResult.Value);
            Assert.False(response.HasError);
            Assert.True(response.Result);
        }


        [Fact]
        public async Task SetPassword_ReturnsOkResult_WithValidRequest()
        {
            // Arrange
            var request = new SetPasswordRequest
            {
                Password = "NewPassword123!",
                Token = "NewPassword123!"
            };

            var mockResponse = new ServiceResponse<bool>
            {
                HasError = false,
                Result = true
            };

            _mockUserService.Setup(service => service.SetPasswordAsync(It.IsAny<SetPasswordRequest>()))
                            .ReturnsAsync(mockResponse);

            var controller = new UserController(_mockUserService.Object);

            // Act
            var result = await controller.SetPassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<bool>>(okResult.Value);
            Assert.False(response.HasError);
            Assert.True(response.Result);
        }


        [Fact]
        public async Task Login_ReturnsOkResult_WithValidRequest()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            var mockResponse = new ServiceResponse<LoginResponse>
            {
                HasError = false,
                Result = new LoginResponse
                {
                    AccessToken = "mockAccessToken",
                    RefreshToken = "mockRefreshToken",
                    Role = "Admin"
                }
            };

            _mockUserService.Setup(service => service.LoginAsync(It.IsAny<LoginRequest>()))
                           .ReturnsAsync(mockResponse);

            var controller = new UserController(_mockUserService.Object);

            // Act
            var result = await controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<LoginResponse>>(okResult.Value);
            Assert.False(response.HasError);
            Assert.NotNull(response.Result);
            Assert.Equal(mockResponse.Result.AccessToken, response.Result.AccessToken);
            Assert.Equal(mockResponse.Result.RefreshToken, response.Result.RefreshToken);
            Assert.Equal(mockResponse.Result.Role, response.Result.Role);
        }


        [Fact]
        public async Task RefreshToken_ReturnsOkResult_WithValidRequest()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                RefreshToken = "oldRefreshToken"
            };

            var mockResponse = new ServiceResponse<LoginResponse>
            {
                HasError = false,
                Result = new LoginResponse
                {
                    AccessToken = "newAccessToken",
                    RefreshToken = "newRefreshToken",
                    Role = "Admin"
                }
            };

            _mockUserService.Setup(service => service.RefreshTokenAsync(It.IsAny<RefreshTokenRequest>()))
                           .ReturnsAsync(mockResponse);

            var controller = new UserController(_mockUserService.Object);

            // Act
            var result = await controller.RefreshToken(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<LoginResponse>>(okResult.Value);
            Assert.False(response.HasError);
            Assert.NotNull(response.Result);
            Assert.Equal(mockResponse.Result.AccessToken, response.Result.AccessToken);
            Assert.Equal(mockResponse.Result.RefreshToken, response.Result.RefreshToken);
            Assert.Equal(mockResponse.Result.Role, response.Result.Role);
        }

        [Fact]
        public async Task ForgotPassword_ReturnsOkResult_WithValidRequest()
        {
            // Arrange
            var request = new ForgotPasswordRequest
            {
                Email = "test@example.com"
            };

            var mockResponse = new ServiceResponse<bool>
            {
                HasError = false,
                Result = true
            };

            _mockUserService.Setup(service => service.ForgotPassword(It.IsAny<ForgotPasswordRequest>()))
                            .ReturnsAsync(mockResponse);

            var controller = new UserController(_mockUserService.Object);

            // Act
            var result = await controller.ForgotPassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<bool>>(okResult.Value);
            Assert.False(response.HasError);
            Assert.True(response.Result);
        }
    }
}
