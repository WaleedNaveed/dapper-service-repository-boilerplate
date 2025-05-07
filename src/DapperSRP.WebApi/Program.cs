using DapperSRP.Persistence;
using DapperSRP.Repository;
using DapperSRP.WebApi.Extension;
using DapperSRP.Logging;
using Serilog;
using DapperSRP.Quartz;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .RegisterServices(builder.Configuration)
    .RegisterRepositories(connectionString)
    .AddApiConfiguration()
    .AddCustomCors();

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

app.UseCors("CorsPolicy");

app.MapControllers();
app.Run();

public partial class Program { }