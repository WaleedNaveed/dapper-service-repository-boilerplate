using DapperSRP.RateLimiter.IntegrationTests.Setup;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace DapperSRP.RateLimiter.IntegrationTests
{
    public class RateLimiterIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly int _allowedRequests;

        public RateLimiterIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateAuthenticatedClient();
            var configuration = factory.Services.GetRequiredService<IConfiguration>();
            _allowedRequests = configuration.GetValue<int>("RateLimiting:PerUser:TokenLimit");
        }

        [Fact]
        public async Task Should_Block_Request_When_RateLimit_Exceeded()
        {
            var url = "api/v1/Product";

            for (int i = 0; i < _allowedRequests; i++)
            {
                var response = await _client.GetAsync(url);
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            var blockedResponse = await _client.GetAsync(url);
            blockedResponse.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }
    }
}
