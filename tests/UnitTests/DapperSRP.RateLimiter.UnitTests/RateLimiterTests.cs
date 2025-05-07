using Microsoft.Extensions.Options;
using Moq;
using DapperSRP.Common.Configuration;
using System.Threading.RateLimiting;

namespace DapperSRP.RateLimiter.UnitTests
{
    public class RateLimiterTests
    {
        private readonly Mock<IOptions<RateLimitingConfig>> _mockOptions;

        public RateLimiterTests()
        {
            _mockOptions = new Mock<IOptions<RateLimitingConfig>>();

            _mockOptions.Setup(opt => opt.Value).Returns(new RateLimitingConfig
            {
                PerUser = new PerUserRateLimit
                {
                    PolicyName = "PerUserRateLimit",
                    TokenLimit = 5,
                    QueueLimit = 5,
                    TokensPerMinute = 10,
                    ReplenishmentPeriodMinutes = 1
                }
            });
        }

        [Fact]
        public async Task Should_AllowRequest_When_UnderRateLimit()
        {
            var perUserInfo = _mockOptions.Object.Value.PerUser;

            var partition = new RateLimitPartition<string>(
                "test-user",
                _ => new TokenBucketRateLimiter(
                    new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = perUserInfo.TokenLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = perUserInfo.QueueLimit,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(perUserInfo.ReplenishmentPeriodMinutes),
                        TokensPerPeriod = perUserInfo.TokensPerMinute
                    }
                )
            );

            var limiter = partition.Factory("test-user");

            var result = await limiter.AcquireAsync(1);
            Assert.True(result.IsAcquired);  // Check if the request was allowed
        }

        [Fact]
        public async Task Should_BlockRequest_When_RateLimitExceeded()
        {
            var partition = new RateLimitPartition<string>(
                "test-user",
                _ => new TokenBucketRateLimiter(
                    new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 2,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(5),
                        TokensPerPeriod = 1
                    }
                )
            );

            var limiter = partition.Factory("test-user");

            var result1 = await limiter.AcquireAsync(1);
            Assert.True(result1.IsAcquired);

            var result2 = await limiter.AcquireAsync(1);
            Assert.True(result2.IsAcquired);

            var result3 = await limiter.AcquireAsync(1);
            Assert.False(result3.IsAcquired);
        }
    }
}
