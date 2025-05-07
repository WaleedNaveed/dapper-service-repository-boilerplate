using Microsoft.Extensions.Options;
using DapperSRP.Common;
using DapperSRP.Common.Configuration;
using DapperSRP.Repository.Interface;

namespace DapperSRP.Repository.Initialization
{
    public class DbSeeder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<SuperAdminCredentialsConfig> _superAdminCredentials;

        public DbSeeder(IUnitOfWork unitOfWork, IOptions<SuperAdminCredentialsConfig> superAdminCredentials)
        {
            _unitOfWork = unitOfWork;
            _superAdminCredentials = superAdminCredentials;
        }

        public async Task SeedDatabaseAsync()
        {
            await SeedRolesAsync();
            await SeedSuperAdminUserAsync();
        }

        private async Task SeedRolesAsync()
        {
            foreach (string roleName in Roles.DefaultRoles)
            {
                string query = "SELECT COUNT(*) FROM Roles WHERE Name = @Role";
                int count = await _unitOfWork.ExecuteScalarAsync<int>(query, new { Role = roleName });

                if (count == 0)
                {
                    string insertQuery = "INSERT INTO Roles (Name) VALUES (@Role)";
                    await _unitOfWork.ExecuteAsync(insertQuery, new { Role = roleName });
                }
            }
        }

        private async Task SeedSuperAdminUserAsync()
        {
            string checkSuperAdminQuery = @"
                SELECT u.Id FROM Users u
                JOIN UserRoles ur ON u.Id = ur.UserId
                JOIN Roles r ON ur.RoleId = r.Id
                WHERE r.Name = @Role";

            int? superAdminId = await _unitOfWork.ExecuteScalarAsync<int?>(checkSuperAdminQuery,
                                new { Role = Roles.SuperAdmin });

            if (superAdminId == null)
            {
                string getRoleIdQuery = "SELECT Id FROM Roles WHERE Name = @Role";
                int superAdminRoleId = await _unitOfWork.ExecuteScalarAsync<int>(getRoleIdQuery,
                                        new { Role = Roles.SuperAdmin });

                string insertSuperAdminUserQuery = @"
                    INSERT INTO Users (Name, Email, Password, CreatedAt, IsEmailConfirmed) 
                    VALUES (@Name, @Email, @Password, @CreatedAt, @IsEmailConfirmed);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                superAdminId = await _unitOfWork.ExecuteScalarAsync<int>(insertSuperAdminUserQuery, new
                {
                    Name = _superAdminCredentials.Value.Name,
                    Email = _superAdminCredentials.Value.Email,
                    Password = PasswordHasher.HashPassword(_superAdminCredentials.Value.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsEmailConfirmed = true
                });

                string insertSuperAdminUserRoleQuery = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
                await _unitOfWork.ExecuteAsync(insertSuperAdminUserRoleQuery, new
                {
                    UserId = superAdminId,
                    RoleId = superAdminRoleId
                });
            }
        }
    }
}
