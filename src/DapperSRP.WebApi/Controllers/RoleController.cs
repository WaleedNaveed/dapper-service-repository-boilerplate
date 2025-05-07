using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DapperSRP.Common;
using DapperSRP.Service.Interface;
using Asp.Versioning;

namespace DapperSRP.WebApi.Controllers
{
    [ApiVersion("1")]
    public class RoleController : VersionApiController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.SuperAdmin}, {Roles.Admin}")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _roleService.GetAllRolesAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{Roles.SuperAdmin}, {Roles.Admin}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _roleService.GetRoleByIdAsync(id);
            return Ok(response);
        }
    }
}
