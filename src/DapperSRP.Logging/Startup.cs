using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DapperSRP.Logging
{
    public static class Startup
    {
        public static void ConfigureLogging(this IHostBuilder hostBuilder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .CreateLogger();

            hostBuilder.UseSerilog();
        }

        public static void AddLoggingServices(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));
        }
    }
}
