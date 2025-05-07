using DapperSRP.Repository.Interface;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DapperSRP.Repository.Repository;
using Microsoft.Extensions.Options;
using DapperSRP.Common.Configuration;

namespace DapperSRP.WebApi.Extension
{
    public static class ServiceExtensions
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtConfig>>().Value;
                var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}
