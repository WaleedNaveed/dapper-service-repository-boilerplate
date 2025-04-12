CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(1000) NULL,
    RefreshToken NVARCHAR(255) NULL,
    RefreshTokenExpiry DATETIME NULL,
    IsEmailConfirmed BIT,
    PasswordResetToken NVARCHAR(255) NULL,
    PasswordResetExpiry DATETIME NULL,
    CreatedAt DATETIME
);
