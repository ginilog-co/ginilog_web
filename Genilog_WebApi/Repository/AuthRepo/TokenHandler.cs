using Genilog_WebApi.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public class TokenHandler(IConfiguration configuration, Genilog_Data_Context maap_Context) : ITokenHandler
    {
        private readonly IConfiguration configuration = configuration;
        private readonly Genilog_Data_Context maap_Context = maap_Context;

        public Task<string> CreateTokenAsync(GeneralUsers sub)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            // create claims
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, sub.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.GivenName, sub.LastName!),
                new(ClaimTypes.Surname, sub.FirstName!),
                new(ClaimTypes.Email, sub.Email!)
            };
            //loop into roles of usres

            sub.Roles!.ForEach((role) =>
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            });
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
               issuer: configuration["Jwt:Issuer"],
               audience: configuration["Jwt:Audience"],
               claims: claims,
               expires: DateTime.UtcNow.AddHours(3),
               signingCredentials: credentials
                );
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<string> RefreshTokenAsync(string email)
        {
            var sub = await maap_Context.GeneralUsers!.FirstOrDefaultAsync(u => u.Email == email);
            if (sub == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            var newRefreshToken = GenerateRefreshToken();
            _ = int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            sub.RefreshToken = newRefreshToken;
            sub.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);
            await maap_Context.SaveChangesAsync();
            return sub.RefreshToken;
        }


        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
