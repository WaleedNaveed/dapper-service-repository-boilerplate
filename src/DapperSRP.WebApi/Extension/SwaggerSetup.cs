using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using Asp.Versioning.ApiExplorer;
using DapperSRP.Common;

namespace DapperSRP.WebApi.Extension
{
    public static class SwaggerSetup
    {
        public static IServiceCollection AddSwaggerSetup(this IServiceCollection services)
        {
            services.AddApiVersioning().AddApiExplorer(options =>
            {

                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName,
                        new OpenApiInfo
                        {
                            Title = Constants.SwaggerName,
                            Version = description.ApiVersion.ToString(),
                            Description = Constants.SwaggerName,
                            Contact = new OpenApiContact
                            {
                                Name = Constants.SwaggerName,
                            },
                        });
                }

                options.DescribeAllParametersInCamelCase();
                options.OrderActionsBy(x => x.RelativePath);

                var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlfile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // JWT Authentication in Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your Bearer token"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerSetup(this IApplicationBuilder app)
        {
            app.UseSwagger()
               .UseSwaggerUI(options =>
               {
                   var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

                   foreach (var description in provider.ApiVersionDescriptions)
                   {
                       options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                               $"{Constants.SwaggerName} {description.ApiVersion}");
                   }

                   //options.RoutePrefix = "api-docs"; // Change this as needed
                   options.DocExpansion(DocExpansion.List);
                   options.DisplayRequestDuration();
               });

            return app;
        }
    }
}
