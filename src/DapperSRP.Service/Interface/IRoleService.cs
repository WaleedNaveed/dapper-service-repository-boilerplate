using DapperSRP.Dto.Role.Response;

namespace DapperSRP.Service.Interface
{
    public interface IRoleService
    {
        Task<ServiceResponse<List<GetRoleResponse>>> GetAllRolesAsync();
        Task<ServiceResponse<GetRoleResponse>> GetRoleByIdAsync(int Id);
    }
}
