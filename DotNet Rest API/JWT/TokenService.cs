using DotNet_Rest_API.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotNet_Rest_API.JWT
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(AppUser user, IList<string> roles)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? 
                throw new InvalidOperationException("Jwt:Key is missing in configuration."));
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ??
                    throw new InvalidOperationException("User email is null.")),
                new(ClaimTypes.Name, user.UserName ??
                    throw new InvalidOperationException("Username is null.")),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                //issuer: _config["Jwt:Issuer"],
                //audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:AccessTokenLifetimeMinutes"] ?? 
                    throw new InvalidOperationException("Jwt:AccessTokenLifetimeMinutes is missing in configuration."))),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public DateTime GetRefreshTokenExpiry()
        {
            return DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:RefreshTokenLifetimeMinutes"] ?? 
                throw new InvalidOperationException("Jwt:RefreshTokenLifetimeMinutes is missing in configuration.")));
        }
    }
}
