namespace DapperSRP.Common
{
    public static class Constants
    {
        public const string PasswordSetEmailSubject = "Set Your Password";
        public const string PasswordResetEmailSubject = "Reset Your Password";
        public const string DailyUserReportJobEmailSubject = "New Users Report";
        public const string DailyProductReportJobEmailSubject = "New Products Report";

        public const string SwaggerName = "DapperSRP API";
    }

    public static class PaginationConstants
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 10;
    }

    public static class DatabaseConstants
    {
        public const string MasterDatabaseName = "master";
        public const string MigrationsFolder = "Migrations";
    }

    public static class ConfigKeys
    {
        public const string SetPasswordLinkExpiryMinutes = "SetPasswordLinkExpiryMinutes";
        public const string AppUrl = "AppUrl";
        public const string SuperAdminCredentials = "SuperAdminCredentials";
        public const string Smtp = "Smtp";
        public const string Jwt = "Jwt";
        public const string RateLimiting = "RateLimiting";
        public const string Cors = "Cors";
    }

    public static class EmailTemplateConstants
    {
        public const string BaseFolder = "EmailTemplates";
        public const string PasswordSetEmail = "PasswordSetEmail.html";
        public const string PasswordResetEmail = "PasswordResetEmail.html";
        public const string DailyUserReportEmail = "DailyUserReportEmail.html";
        public const string DailyProductReportEmail = "DailyProductReportEmail.html";
    }

    public static class MessagesConstants
    {
        public const string InvalidUserIdInToken = "Invalid user ID in token";
        public const string RoleWithTheProvidedIdDoesNotExist = "Role with the provided ID does not exist";
        public const string InvalidOrExpiredToken = "Invalid or expired token";
        public const string InvalidCredentials = "Invalid credentials";
        public const string InvalidOrExpiredRefreshToken = "Invalid or expired refresh token";
        public const string UserDoesNotExist = "User does not exist";
        public const string UserNotFound = "User not found";
        public const string InvalidUsernameInToken = "Invalid username in token";
        public const string ProductCreationFailed = "Product creation failed";
        public const string ProductDoesNotExist = "Product does not exist";
        public const string ProductDeletionFailed = "Product deletion failed";
        public const string CouldNotUpdateProduct = "Could not update product";
        public const string InvalidRole = "Invalid role";
        public const string EmailAlreadyRegistered = "Email already registered";
        public const string UserCreationFailed = "User creation failed";
        public const string LinkHasBeenExpired = "Link has been expired. Request a new one";
    }
}
