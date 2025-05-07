using DapperSRP.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DapperSRP.RateLimiter.IntegrationTests.Setup
{
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, Constants.TestUser),
                new Claim(ClaimTypes.NameIdentifier, Constants.TestUserId),
                new Claim(ClaimTypes.Role, Roles.SuperAdmin)
            };

            var identity = new ClaimsIdentity(claims, Constants.TestAuth);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Constants.TestAuth);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
