
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.AuthModel;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using System.Security.Cryptography;
using System.Text;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public class GeneralUserRepository(Genilog_Data_Context bmg_context) : IGeneralUserRepository
    {
        private readonly Genilog_Data_Context bmg_context = bmg_context;

        public async Task<GeneralUsers> AuthenticateAsync(string email, string password)
        {
            var user = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(x => x.Email == email
            ||x.PhoneNo==email || x.UserName==email);
            if (user == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            if (!VerifyPassword(password, user.PasswordHash!, user.PasswordSalt!))
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.

            //var userRoles = await bmg_context.User_Roles!.Where(x => x.GeneralUsersId == user.Id).ToListAsync();
            //if (userRoles.Count != 0)
            //{
            //    foreach (var userRole in userRoles)
            //    {
            //        var role = await bmg_context.Roles!.FirstOrDefaultAsync(x => x.Id == userRole.RoleId);
            //        if (role != null)
            //        {
            //            user.Roles.Add(role.Name!);
            //        }
            //    }
            //}
            // user.Password = null;
            return user!;
        }
        private static bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            var argon2 = new Konscious.Security.Cryptography.Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = passwordSalt,
                DegreeOfParallelism = 8,
                Iterations = 3,
                MemorySize = 1024 * 32
            };

            var computedHash = argon2.GetBytes(32);
            return CryptographicOperations.FixedTimeEquals(computedHash, passwordHash);
        }

        public async Task<GeneralUsers> AddAsync(GeneralUsers sub, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            sub.Id = Guid.NewGuid();
            sub.PasswordHash = passwordHash;
            sub.PasswordSalt = passwordSalt;
            await bmg_context.AddAsync(sub);
            await bmg_context.SaveChangesAsync();
            return sub;
        }

       private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            passwordSalt = RandomNumberGenerator.GetBytes(16);

            var argon2 = new Konscious.Security.Cryptography.Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = passwordSalt,
                DegreeOfParallelism = 8,
                Iterations = 3,
                MemorySize = 1024 * 32 // 32MB

            };

            passwordHash = argon2.GetBytes(32);
        }
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        public async Task<GeneralUsers?> DeleteAsync(Guid id)
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        {
            var sub = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(x => x.Id == id);
            if (sub == null)
            {
                return null;
            }
            else
            {
                // Delete Region
                bmg_context.GeneralUsers!.Remove(sub);
                await bmg_context.SaveChangesAsync();
                return sub;
            }

        }

        public async Task<IEnumerable<GeneralUsers>> GetAllAsync()
        {
            return await bmg_context.GeneralUsers!.AsNoTracking()
                 .Include(u => u.User_Roles!)
                 .ThenInclude(ur => ur.Roles)
                 .Include(u => u.UserPermissions!)
                 .ThenInclude(up => up.Permission)
                .ToListAsync();
        }

        public async Task<GeneralUsers> GetAsync(Guid id)
        {

#pragma warning disable CS8603 // Possible null reference return.
            return await bmg_context.GeneralUsers!.AsNoTracking()
                  .Include(u => u.User_Roles!)
                 .ThenInclude(ur => ur.Roles)
                 .Include(u => u.UserPermissions!)
                 .ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<GeneralUsers> GetByEmailAsync(string email)
        {

#pragma warning disable CS8603 // Possible null reference return.
            return await bmg_context.GeneralUsers!.AsNoTracking()
                  .Include(u => u.User_Roles!)
                 .ThenInclude(ur => ur.Roles)
                 .Include(u => u.UserPermissions!)
                 .ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(x => x.Email == email);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<List<GeneralUsers>> GetAllByIdsAsync(List<Guid> ids)
        {
            return await bmg_context.GeneralUsers!
                .Where(x => ids.Contains(x.Id))
                    .Include(u => u.User_Roles!)
                    .ThenInclude(ur => ur.Roles)
                    .Include(u => u.UserPermissions!)
                    .ThenInclude(up => up.Permission)
                .ToListAsync();
        }

#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        public async Task<GeneralUsers?> UpdateAsync(Guid id, GeneralUsers user)
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        {
            var existinguser = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(x => x.Id == id);

            if (existinguser == null)
            {
                return null;
            }
            else
            {
                existinguser.LastName = user.LastName;
                existinguser.FirstName = user.FirstName;
                existinguser.ImagePath = user.ImagePath;
                existinguser.PhoneNo = user.PhoneNo;
                existinguser.ActivateWallet = user.ActivateWallet;
                await bmg_context.SaveChangesAsync();
                return existinguser;
            }
        }

        public async Task<bool> UserExistAsync(string email)
        {
            var user = await bmg_context.GeneralUsers!.AnyAsync(x => x.Email == email);
            if (user)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return true;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> UserPhoneNoExistAsync(string phoneNo)
        {
            var user = await bmg_context.GeneralUsers!.AnyAsync(x => x.PhoneNo == phoneNo);
            if (user)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return true;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UserNameExistAsync(string userName)
        {
            var user = await bmg_context.GeneralUsers!.AnyAsync(x => x.UserName == userName);
            if (user)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return true;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                return false;
            }
        }

        public async Task<GeneralUsers> VerifyAsync(string token)
        {
            var user = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null || user.EmailTokenExpires <= DateTime.UtcNow)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            user.EmailConfirmed = true;
            user.VerifiedAt = DateTime.UtcNow;
            user.VerificationToken = "";
            user.EmailTokenExpires = DateTime.UtcNow;
            await bmg_context.SaveChangesAsync();
            return user;
        }
        public async Task<GeneralUsers> PhoneNoVerifyAsync(string otp)
        {
            var user = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(u => u.PhoneVerificationToken == otp);
            if (user == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            user.PhoneNoConfirmed = true;
            user.PhoneVerifiedAt = DateTime.UtcNow;
            user.PhoneVerificationToken = "";
            user.PhoneNoTokenExpires = DateTime.UtcNow;
            await bmg_context.SaveChangesAsync();
            return user;
        }

        public async Task<GeneralUsers> ForgetPasswordAsync(string email)
        {
            var user = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }

            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(10);
            await bmg_context.SaveChangesAsync();
            return user;
        }
        public async Task<GeneralUsers> RequestNewEmailTokenAsync(string email)
        {
            var user = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }

            user.VerificationToken = CreateRandomToken();
            user.EmailTokenExpires = DateTime.UtcNow.AddMinutes(10);
            await bmg_context.SaveChangesAsync();
            return user;
        }

        public async Task<GeneralUsers> RequestNewPhoneNoTokenAsync(string phoneNo)
        {
            var user = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(u => u.PhoneNo == phoneNo);
            if (user == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }

            user.PhoneVerificationToken = CreateRandomToken();
            user.PhoneNoTokenExpires = DateTime.UtcNow.AddMinutes(10);
            await bmg_context.SaveChangesAsync();
            return user;
        }

        public async Task<TwoFactorCodeModel> TwoFactorEnabledAsync(Guid id)
        {
            var user = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }

            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(secretKey);

            user.TwoFactorSecret = base32Secret;
            await bmg_context.SaveChangesAsync();
            var qrCodeUrl = $"otpauth://totp/Bizora+:{user.Email}?secret={base32Secret}&issuer=Bizora+";

            var data = new TwoFactorCodeModel 
            {
                Secret = base32Secret,
                QrCodeUrl = qrCodeUrl
            };

            return data;
        }

        public async Task<TwoFactorCodeModel> EnableTwoFactorAsync(Guid id, string code)
        {
            var user = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }

            var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecret!));

            if (!totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay))
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.


            var (rawCodes, hashedCodes) = GenerateRecoveryCodes();

            user.TwoFactorEnabled = true;
            user.TwoFactorRecoveryCodesHash = hashedCodes;
            await bmg_context.SaveChangesAsync();

            var data = new TwoFactorCodeModel
            {
                RawCodes= rawCodes ,// SHOW ONCE,
                Message = "Two-factor authentication enabled.",
                QrCodeUrl="",
                Secret=""
                

            };

            return data;
        }

        public async Task<GeneralUsers> CheckUserAsync(Guid id)
        {
            var user = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            return user;
        }
        private static string CreateRandomToken()
        {
            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new();
            for (int i = 0; i < 5; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos)!.ToString()!)) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
        }

        public async Task<GeneralUsers> PasswordResetAsync(string token, string password)
        {
            var user = await bmg_context.GeneralUsers!.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = "";
            user.ResetTokenExpires = DateTime.UtcNow;
            await bmg_context.SaveChangesAsync();
            return user;
        }

        // Device Token
        public async Task<IEnumerable<DeviceTokenModel>> GetAllDeviceTokenAsync()
        {
            return await bmg_context.DeviceTokenModels!.AsNoTracking().ToListAsync();
        }
        public async Task<DeviceTokenModel> AddDeviceTokenModelAsync(DeviceTokenModel subTable)
        {
            subTable.Id = Guid.NewGuid();
            await bmg_context.AddAsync(subTable);
            await bmg_context.SaveChangesAsync();
            return subTable;
        }
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        public async Task<DeviceTokenModel?> DeleteDeviceTokenModelAsync(string id)
        {
            var subTable = await bmg_context.DeviceTokenModels!.FirstOrDefaultAsync(x => x.Id.ToString() == id||x.DeviceTokenId==id);
            if (subTable == null)
            {
                return null;
            }
            else
            {
                // Delete Region
                bmg_context.DeviceTokenModels!.Remove(subTable);
                await bmg_context.SaveChangesAsync();
                return subTable;
            }
        }

        public async Task<DeviceTokenModel?> UpdateDeviceTokenModelAsync(Guid id, DeviceTokenModel subTable)
        {
            var existinguser = await bmg_context.DeviceTokenModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (existinguser == null)
            {
                return null;
            }
            else
            {
                existinguser.DeviceTokenId = subTable.DeviceTokenId;
                await bmg_context.SaveChangesAsync();
                return existinguser;
            }
        }

        public async Task<bool> DeviceTokenExistAsync(string token)
        {
            var user = await bmg_context.DeviceTokenModels!.AnyAsync(x => x.DeviceTokenId == token);
            if (user)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return true;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                return false;
            }
        }

        private static (string[] rawCodes, string hashedCodes) GenerateRecoveryCodes()
        {
            var rawCodes = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                rawCodes.Add(RandomNumberGenerator.GetInt32(100_000, 999_999).ToString());
            }

            // Hash codes before storage
            var hashedCodes = string.Join(";", rawCodes.Select(HashRecoveryCode));

            return (rawCodes.ToArray(), hashedCodes);
        }

        private static string HashRecoveryCode(string code)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(code));
            return Convert.ToBase64String(hash);
        }

        public async Task<bool> VerifyRecoveryCode(GeneralUsers user, string inputCode)
        {
            var inputHash = HashRecoveryCode(inputCode);

            var storedHashes = user.TwoFactorRecoveryCodesHash!
                .Split(';')
                .ToList();

            if (!storedHashes.Contains(inputHash))
                return false;

            // Remove used code
            storedHashes.Remove(inputHash);
            user.TwoFactorRecoveryCodesHash = string.Join(";", storedHashes);

            return true;
        }


    }
}
