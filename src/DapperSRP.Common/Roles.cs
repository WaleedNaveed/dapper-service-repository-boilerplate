using System.Collections.ObjectModel;

namespace DapperSRP.Common
{
    public static class Roles
    {
        public const string SuperAdmin = "Super Admin";
        public const string Admin = "Admin";
        public const string User = "User";

        public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
        {
            SuperAdmin,
            Admin,
            User
        });
    }
}
