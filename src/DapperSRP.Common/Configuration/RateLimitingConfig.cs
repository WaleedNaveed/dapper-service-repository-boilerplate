namespace DapperSRP.Common.Configuration
{
    public class RateLimitingConfig
    {
        public PerUserRateLimit PerUser { get; set; }
    }

    public class PerUserRateLimit
    {
        public string PolicyName { get; set; }
        public int TokenLimit { get; set; }
        public int QueueLimit { get; set; }
        public int TokensPerMinute { get; set; }
        public int ReplenishmentPeriodMinutes { get; set; }
    }
}
