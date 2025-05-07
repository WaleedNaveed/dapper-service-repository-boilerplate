using AutoMapper;
using DapperSRP.Common;
using DapperSRP.Dto.Role.Response;
using DapperSRP.Persistence.Models;
using DapperSRP.Repository.Interface;
using DapperSRP.Service.Exceptions;
using DapperSRP.Service.Interface;

namespace DapperSRP.Service.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _repository;
        private readonly IMapper _mapper;

        public RoleService(IRepository<Role> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<GetRoleResponse>>> GetAllRolesAsync()
        {
            var roles = (await _repository.GetAllAsync())
                        .Where(x => x.Name != Roles.SuperAdmin);

            var response = _mapper.Map<List<GetRoleResponse>>(roles);

            return new ServiceResponse<List<GetRoleResponse>>()
            {
                HasError = false,
                Result = response
            };
        }

        public async Task<ServiceResponse<GetRoleResponse>> GetRoleByIdAsync(int Id)
        {
            var role = await _repository.GetByIdAsync(Id);

            if (role == null)
            {
                throw new NotFoundException(MessagesConstants.RoleWithTheProvidedIdDoesNotExist);
            }

            var response = _mapper.Map<GetRoleResponse>(role);

            return new ServiceResponse<GetRoleResponse>()
            {
                HasError = false,
                Result = response
            };
        }
    }
}
