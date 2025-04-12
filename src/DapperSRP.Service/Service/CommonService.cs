using Microsoft.AspNetCore.Http;
using DapperSRP.Persistence.Models;
using DapperSRP.Repository.Interface;
using DapperSRP.Service.Exceptions;
using DapperSRP.Service.Interface;
using System.Security.Claims;
using DapperSRP.Common;

namespace DapperSRP.Service.Service
{
    public class CommonService : ICommonService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommonService(IUserRoleRepository userRoleRepository,
            IRepository<Role> roleRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string?> GetUserRoleByUserId(int userId)
        {
            var roleId = (await _userRoleRepository.GetByUserIdAsync(userId)).RoleId;
            var role = await _roleRepository.GetByIdAsync(roleId);

            return role?.Name;
        }

        public int GetLoggedInUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnAuthorizedException(MessagesConstants.InvalidUserIdInToken);
            }

            return userId;
        }

        public string GetLoggedInUsername()
        {
            var usernameClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(usernameClaim))
            {
                throw new UnAuthorizedException(MessagesConstants.InvalidUsernameInToken);
            }

            return usernameClaim;
        }

        public string GetJwtLoggedInUser()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return token;
        }

    }
}
