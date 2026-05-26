using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.AuthModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public class TokenHandler(IConfiguration configuration, Genilog_Data_Context maap_Context) : ITokenHandler
    {
        private readonly IConfiguration configuration = configuration;
        private readonly Genilog_Data_Context maap_Context = maap_Context;

        public async Task<string> CreateTokenAsync(GeneralUsers user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // ----------------------------------------------------
            // 1. Load User Roles (ONLY for identity, not permissions)
            // ----------------------------------------------------
            var roles = await maap_Context.User_Roles!
                .Where(ur => ur.GeneralUsersId == user.Id)
                .Select(ur => ur.Roles!.Name!)
                .ToListAsync();

            // ----------------------------------------------------
            // 2. Load ONLY user-assigned permissions
            // ----------------------------------------------------
            var userPermissions = await maap_Context.UserPermissionUsages!
                .Where(up => up.GeneralUsersId == user.Id)
                .Select(up => up.Permission!.Name!)
                .ToListAsync();

            // ----------------------------------------------------
            // 3. Create Claims
            // ----------------------------------------------------
            var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.GivenName, user.FirstName!),
        new(ClaimTypes.Surname, user.LastName!),
        new(ClaimTypes.Email, user.Email!)
    };

            // ----------------------------------------------------
            // 4. Add Roles (for identity only, not permission access)
            // ----------------------------------------------------
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // ----------------------------------------------------
            // 5. Add ONLY assigned permissions
            // ----------------------------------------------------
            foreach (var permission in userPermissions)
                claims.Add(new Claim("permission", permission));

            // ----------------------------------------------------
            // 6. Build Token
            // ----------------------------------------------------
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
            sub.RefreshToken = newRefreshToken;
            sub.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await maap_Context.SaveChangesAsync();
            return newRefreshToken;
        }


        public async Task<string?> RotateRefreshTokenAsync(string email, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var user = await maap_Context.GeneralUsers!.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            if (user.RefreshToken != refreshToken)
                return null;

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await maap_Context.SaveChangesAsync();
            return newRefreshToken;
        }

        public async Task<bool> LogoutAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var user = await maap_Context.GeneralUsers!.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return false;

            // REMOVE REFRESH TOKEN
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow;
            await maap_Context.SaveChangesAsync();
            return true;
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
