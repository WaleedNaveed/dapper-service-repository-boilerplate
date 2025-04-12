using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DapperSRP.Dto
{
    public static class FluentValidationExtensions
    {
        public static IServiceCollection AddFluentValidations(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddFluentValidationAutoValidation();

            return services;
        }
    }
}
