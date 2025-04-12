using DapperSRP.Persistence;
using DapperSRP.Repository;
using DapperSRP.Service.Exceptions;
using DapperSRP.Service.Interface;
using DapperSRP.Service.Mapper;
using DapperSRP.Service.Service;
using DapperSRP.WebApi.Extension;
using DapperSRP.WebApi.Middleware;
using DapperSRP.Common;
using DapperSRP.Logging;
using Serilog;
using DapperSRP.Service;
using Asp.Versioning;
using DapperSRP.Quartz;
using DapperSRP.Dto;

//var builder = WebApplication.CreateBuilder(args);
//string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//string redisConnectionString = builder.Configuration.GetConnectionString("Redis");

//builder.Services.AddFluentValidations();
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//builder.Services.AddRateLimiting();

//builder.Services.AddRepositoryServices();
//builder.Services.AddServiceLayer(redisConnectionString);
//builder.Services.AddJwtAuthentication(builder.Configuration);

//builder.Services.AddQuartzJobs();


//builder.Services.AddHttpContextAccessor();

//builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IRoleService, RoleService>();
//builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
//builder.Services.AddScoped<ICommonService, CommonService>();
//builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddSingleton<IExceptionHandler, ExceptionHandler>();

//builder.Services.AddConfig();
//builder.Services.AddRepository(connectionString);
//builder.Services.AddQuartzJobs();
////builder.Services.AddPersistence(builder.Configuration);

//builder.Services.AddAutoMapper(typeof(MappingProfile));


//// Middleware
//builder.Services.AddScoped<ExceptionHandlerMiddleware>();

//// Logger
//builder.Host.ConfigureLogging();
//builder.Services.AddLoggingServices();

//builder.Services.AddApiVersioning(options =>
//{
//    options.ReportApiVersions = true;
//    options.AssumeDefaultVersionWhenUnspecified = true;
//    options.DefaultApiVersion = new ApiVersion(1, 0);
//});

//builder.Services.AddSwaggerSetup();

//var app = builder.Build();

//app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

//await DatabaseMigrator.RunMigrationsAsync(connectionString);
//await app.Services.DatabaseSeedingAsync();

//await app.Services.UseQuartzSchedulerAsync();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwaggerSetup();
//}

//app.UseMiddleware<RequestResponseLoggingMiddleware>();
//app.UseMiddleware(typeof(ExceptionHandlerMiddleware));



//app.UseHttpsRedirection();

//app.UseAuthentication();

//app.UseMiddleware<JwtBlacklistMiddleware>();

//app.UseRateLimiter();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();

//public partial class Program { }



var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .RegisterServices(builder.Configuration)
    .RegisterRepositories(connectionString)
    .AddApiConfiguration();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddQuartzJobs();

builder.Host.ConfigureLogging();

var app = builder.Build();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

await DatabaseMigrator.RunMigrationsAsync(connectionString);
await app.Services.DatabaseSeedingAsync();
await app.Services.UseQuartzSchedulerAsync();

if (app.Environment.IsDevelopment())
    app.UseSwaggerSetup();

app.ConfigureAppMiddleware();

app.MapControllers();
app.Run();

public partial class Program { }

