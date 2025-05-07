namespace DapperSRP.Service.Interface
{
    public interface ICommonService
    {
        Task<string?> GetUserRoleByUserId(int userId);
        int GetLoggedInUserId();
        string GetLoggedInUsername();
        string GetJwtLoggedInUser();
    }
}
