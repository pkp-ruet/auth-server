using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthServer.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Services
{
    public class TokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;

        public TokenGenerator(AuthenticationConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.AccessTokenSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            JwtSecurityToken token = new JwtSecurityToken(
                _configuration.Issuer, _configuration.Audience, 
                claims, 
                DateTime.UtcNow, 
                DateTime.UtcNow.AddMinutes(_configuration.AccessTokenExpirationTime),
                credentials);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }

        public string GenerateRefreshToken()
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.RefreshTokenSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                _configuration.Issuer, _configuration.Audience,
                null,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(_configuration.RefreshTokenExpirationTime),
                credentials);
            var refreshToken = new JwtSecurityTokenHandler().WriteToken(token);
            return refreshToken;
        }


    }
}
