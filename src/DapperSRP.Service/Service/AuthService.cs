using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DapperSRP.Common.Configuration;
using DapperSRP.Persistence.Models;
using DapperSRP.Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DapperSRP.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly ICommonService _commonService;
        private readonly IOptions<JwtConfig> _jwt;

        public AuthService(ICommonService commonService, IOptions<JwtConfig> jwt)
        {
            _commonService = commonService;
            _jwt = jwt;
        }
        public async Task<string> GenerateJwtToken(User request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwt.Value.SecretKey);

            var role = await _commonService.GetUserRoleByUserId(request.Id);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, request.Id.ToString()),
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Name, request.Name),
                new Claim(ClaimTypes.Role, role)
            }),
                Expires = DateTime.UtcNow.AddMinutes(_jwt.Value.AccessTokenExpiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        //private byte[] GetValidJwtKey(string secretKey)
        //{
        //    if (string.IsNullOrEmpty(secretKey))
        //    {
        //        throw new InvalidOperationException("JWT secret key cannot be null or empty.");
        //    }

        //    var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        //    const int requiredKeySizeInBytes = 32; // 256 bits for HmacSha256

        //    if (keyBytes.Length < requiredKeySizeInBytes)
        //    {
        //        // If key is too short, hash it to get 32 bytes
        //        using var sha256 = SHA256.Create();
        //        return sha256.ComputeHash(keyBytes); // Always returns 32 bytes
        //    }

        //    // If key is longer, truncate to 32 bytes
        //    return keyBytes.Length > requiredKeySizeInBytes
        //        ? keyBytes.Take(requiredKeySizeInBytes).ToArray()
        //        : keyBytes;
        //}
    }
}