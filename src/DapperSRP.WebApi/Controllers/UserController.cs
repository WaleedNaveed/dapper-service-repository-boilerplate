using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DapperSRP.Common;
using DapperSRP.Service.Interface;
using Asp.Versioning;
using DapperSRP.Dto.User.SetPassword.Request;
using DapperSRP.Dto.User.CreateUser.Request;
using DapperSRP.Dto.User.ForgotPassword.Request;
using DapperSRP.Dto.User.Login.Request;
using DapperSRP.Dto.User.RefreshToken.Request;

namespace DapperSRP.WebApi.Controllers
{
    [ApiVersion("1")]
    public class UserController : VersionApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin}, {Roles.Admin}")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var response = await _userService.CreateUserAsync(request);
            return Ok(response);
        }

        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
        {
            var response = await _userService.SetPasswordAsync(request);
            return Ok(response);
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _userService.LoginAsync(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _userService.RefreshTokenAsync(request);
            return Ok(response);
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var response = await _userService.LogoutAsync();
            return Ok(response);
        }

        [HttpPost]
        [Route("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var response = await _userService.ForgotPassword(request);
            return Ok(response);
        }
    }
}
