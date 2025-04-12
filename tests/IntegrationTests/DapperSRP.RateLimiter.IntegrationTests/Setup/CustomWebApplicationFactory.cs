using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;

namespace DapperSRP.RateLimiter.IntegrationTests.Setup
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var testProjectPath = Path.GetDirectoryName(typeof(RateLimiterIntegrationTests).Assembly.Location);
                var configPath = Path.Combine(testProjectPath, "appsettings.Test.json");
                config.AddJsonFile(configPath, optional: false, reloadOnChange: true);
            })
            .ConfigureServices(services =>
            {
                services.RemoveAll<IAuthenticationSchemeProvider>();
                services.RemoveAll<IAuthenticationHandlerProvider>();
                services.RemoveAll<AuthenticationSchemeOptions>();

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Constants.TestAuth;
                    options.DefaultChallengeScheme = Constants.TestAuth;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(Constants.TestAuth, options => { });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Constants.TestPolicy, policy => policy.RequireAuthenticatedUser());
                });
            })
            .UseEnvironment("Test");
        }

        public HttpClient CreateAuthenticatedClient()
        {
            return CreateClient();
        }
    }
}
