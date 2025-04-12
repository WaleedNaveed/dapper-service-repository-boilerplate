using DapperSRP.Common.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DapperSRP.Service.Redis
{
    public class JwtBlacklistService : IJwtBlacklistService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IOptions<JwtConfig> _jwt;

        public JwtBlacklistService(IConnectionMultiplexer redis, IOptions<JwtConfig> jwt)
        {
            _redis = redis;
            _jwt = jwt;
        }
        public async Task BlacklistTokenAsync(string token)
        {
            var db = _redis.GetDatabase();
            var expiration = TimeSpan.FromMinutes(_jwt.Value.AccessTokenExpiryMinutes);
            await db.StringSetAsync(token, "blacklisted", expiration);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync(token);
        }
    }
}
