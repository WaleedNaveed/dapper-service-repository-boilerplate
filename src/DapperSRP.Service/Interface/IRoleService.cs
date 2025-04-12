using DapperSRP.Dto.Role.Response;

namespace DapperSRP.Service.Interface
{
    public interface IRoleService
    {
        Task<List<GetRoleResponse>> GetAllRolesAsync();
        Task<GetRoleResponse> GetRoleByIdAsync(int Id);
    }
}
