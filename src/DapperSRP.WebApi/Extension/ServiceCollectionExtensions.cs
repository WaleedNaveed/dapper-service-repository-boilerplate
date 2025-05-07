using DapperSRP.Service.Exceptions;
using DapperSRP.Service.Interface;
using DapperSRP.Service.Mapper;
using DapperSRP.Service.Service;
using DapperSRP.Service;
using DapperSRP.Dto;
using DapperSRP.WebApi.Middleware;
using DapperSRP.Common;
using DapperSRP.Logging;

namespace DapperSRP.WebApi.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            string redisConnectionString = configuration.GetConnectionString("Redis");

            services.AddFluentValidations();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddRateLimiting();
            services.AddHttpContextAccessor();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddSingleton<IExceptionHandler, ExceptionHandler>();

            services.AddServiceLayer(redisConnectionString);
            services.AddConfig();
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<ExceptionHandlerMiddleware>();
            services.AddLoggingServices();

            return services;
        }
    }
}
