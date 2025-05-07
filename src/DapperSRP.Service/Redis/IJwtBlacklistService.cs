namespace DapperSRP.Service.Redis
{
    public interface IJwtBlacklistService
    {
        Task BlacklistTokenAsync(string token);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}
