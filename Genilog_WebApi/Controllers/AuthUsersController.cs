using AutoMapper;
using Genilog_WebApi.EmailSender;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.AuthModel;
using Genilog_WebApi.Model.UsersDataModel;
using Genilog_WebApi.Repository.AuthRepo;
using Genilog_WebApi.Repository.AuthRepo.PolicyBased;
using Genilog_WebApi.Repository.UploadRepo;
using Genilog_WebApi.Repository.UserRepo;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Genilog_WebApi.Controllers
{
    [Route("api/auth-users")]
    [ApiController]
    public class AuthUsersController(IHostEnvironment _env, IGeneralUserRepository userRepository, ITokenHandler tokenHandler, IMapper mapper, IRolesRepository rolesRepository, IUser_RoleRepository user_RoleRepository, IUserRepository newUsersRepository,
        IUploadRepository uploadRepository, IBlacklistedTokenRepository blacklistedTokenRepository) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IGeneralUserRepository userRepository = userRepository;
        private readonly ITokenHandler tokenHandler = tokenHandler;
        private readonly IMapper mapper = mapper;
        private readonly IRolesRepository rolesRepository = rolesRepository;
        private readonly IUser_RoleRepository user_RoleRepository = user_RoleRepository;
        private readonly IUserRepository newUsersRepository = newUsersRepository;
        private readonly IUploadRepository uploadRepository = uploadRepository;
        private readonly IBlacklistedTokenRepository blacklistedTokenRepository = blacklistedTokenRepository;

        [HttpPut]
        [Route("update-device-token")]
        [Authorize]
        public async Task<IActionResult> UpdateDeviceTokenAsync([FromBody] AddDeviceToken request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var user = await userRepository.GetAsync(userGuid);

            if (user == null)
            {
                var error = new ErrorModel()
                {
                    Message = "User Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
            // convert back to dto
            else
            {

                var check = await userRepository.DeviceTokenExistAsync(request.DeviceToken!);
                if (check)
                {
                    var error = new ErrorModel()
                    {
                        Message = "Device Token Already Exist",
                        Status = true
                    };
                    return BadRequest(error);
                }
                else
                {
                    var drviceToken = new DeviceTokenModel()
                    {

                        DeviceTokenId = request.DeviceToken,
                        UserId = user.Id,
                        UserType = user.UserType,
                    };
                    drviceToken = await userRepository.AddDeviceTokenModelAsync(drviceToken);
                    return Ok(drviceToken);
                }

            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginRequset requset)
        {
            var user = await userRepository.AuthenticateAsync(requset.Email_PhoneNo!, requset.Password!);

            if (user != null)
            {

                if (user.EmailConfirmed == false)
                {
                    var user2 = await userRepository.RequestNewEmailTokenAsync(userD.Email!);
                    try { EmailTemplates.SendEmailVerificationCode(user.Email!, user2.VerificationToken!, user.LastName!); }
                    catch (Exception ex) { Console.WriteLine($"Warning: Email send failed: {ex.Message}"); }
                    //  string message1 = user.FirstName + " Your BMG(Bring My Gas) App Account Verication Code is " + user2.VerificationToken!;
                    var error = new ErrorModel()
                    {
                        Message = "User Email Not Yet Verify",
                        Status = true
                    };
                    return BadRequest(error);
                }
                else if (user.UserType != "User")
                {
                    var error = new ErrorModel()
                    {
                        Message = "Account Does Not Exist",
                        Status = true
                    };
                    return BadRequest(error);
                }
                else
                {
                    var userD = await newUsersRepository.GetAsync(user.Id);
                    var general = await userRepository.GetAsync(userD.Id);
                    //generate jwt token
                    var token = await tokenHandler.CreateTokenAsync(user);
                    var refreshToken = await tokenHandler.RefreshTokenAsync(user.Email!);
                    // var userId = userRepository.Userd;
                    //  
                    var userDto = new LoginDto()
                    {
                        Token = token,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                        UserId = user.Id,
                        Email = user.Email,
                        UserType = user.UserType,
                        EmailVerified = user.EmailConfirmed,
                        PhoneVerified = user.PhoneNoConfirmed,
                        FullName = $"{userD.FirstName} {userD.LastName}",
                        ProfileImage = userD.ProfilePicture,
                        IdAuthPassword = "",
                        Roles = general.Roles,
                        Permissions = general.Permissions,
                        
                    };
                    var request = await newUsersRepository.GetAsync(user.Id);
                    var users = new UsersDataModelTable()
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        PhoneNo = request.PhoneNo,
                        Sex = request.Sex,
                        UserStatus = request.UserStatus,
                        ProfilePicture = request.ProfilePicture,
                        ReferralCode = request.ReferralCode,
                        CreatedAt = request.CreatedAt,
                        Address = request.Address,
                        Locality = request.Locality,
                        State = request.State,
                        PostCodes = request.PostCodes,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude,
                        LastLoginAt = DateTime.UtcNow,
                        LastSeenAt = request.LastSeenAt,
                        ArchivedAccount = request.ArchivedAccount,
                        SuspendedAccount = request.SuspendedAccount,

                    };
                    await newUsersRepository.UpdateAsync(user.Id, users);
                    return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.UserId }, userDto);
                }
            }
            else
            {
                var error = new ErrorModel()
                {
                    Message = "User Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
        }


        [HttpPost("auth-login")]
        public async Task<IActionResult> GoogleAuth([FromBody] LoginExternalRequset requvest)
        {
            try
            {
                // Token is valid, create or fetch the user in your database
                var userExist = await userRepository.UserExistAsync(requvest.Email!);
                if (userExist)
                {
                    var user = await userRepository.AuthenticateAsync(requvest.Email!, requvest.ExternalId!);
                    var userD = await newUsersRepository.GetAsync(user.Id);
                    var general = await userRepository.GetAsync(userD.Id);

                    //generate jwt token
                    var token = tokenHandler.CreateTokenAsync(user);
                    var refreshToken = tokenHandler.RefreshTokenAsync(user.Email!);
                    var userDto = new LoginDto()
                    {
                        Token = await token,
                        RefreshToken = await refreshToken,
                        RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                        UserId = user.Id,
                        Email = user.Email,
                        UserType = user.UserType,
                        EmailVerified = user.EmailConfirmed,
                        PhoneVerified = user.PhoneNoConfirmed,
                        FullName = $"{userD.FirstName} {userD.LastName}",
                        ProfileImage = userD.ProfilePicture,
                        IdAuthPassword = requvest.ExternalId,
                        Roles = general.Roles,
                        Permissions = general.Permissions,
                    };
                    var request = await newUsersRepository.GetAsync(user.Id);
                    var users = new UsersDataModelTable()
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        PhoneNo = request.PhoneNo,
                        Sex = request.Sex,
                        UserStatus = request.UserStatus,
                        ProfilePicture = request.ProfilePicture,
                        ReferralCode = request.ReferralCode,
                        CreatedAt = request.CreatedAt,
                        Address = request.Address,
                        Locality = request.Locality,
                        State = request.State,
                        PostCodes = request.PostCodes,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude,
                        LastLoginAt = DateTime.UtcNow,
                        LastSeenAt = request.LastSeenAt,
                        AccountName = request.AccountName,
                        AccountNumber =request.AccountNumber,
                        BankName = request.BankName,
                        ArchivedAccount = request.ArchivedAccount,
                        SuspendedAccount = request.SuspendedAccount,
                        MoneyBoxBalance = request.MoneyBoxBalance,
                    };
                    await newUsersRepository.UpdateAsync(user.Id, users);
                    return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.UserId }, userDto);
                }
                else
                {
                    var generalUsers = new GeneralUsers()
                    {
                        UserName = "",
                        FirstName = requvest.FirstName,
                        LastName = requvest.LastName,
                        Email = requvest.Email,
                        UserType = RoleType.User.ToString(),
                        VerificationToken = CreateRandomToken(),
                        EmailConfirmed = true,
                        PhoneNo = requvest.PhoneNo,
                        ImagePath = requvest.ProfilePicture,
                        CreatedAt = DateTime.UtcNow,
                        LockOutEndEnabled = false,
                        AccessFailedCount = 0,
                        TwoFactorEnabled = false,
                        LockOutEnd = DateTime.UtcNow.AddDays(30),
                        PhoneNoConfirmed = false,
                        ResetTokenExpires = DateTime.UtcNow.AddMinutes(10),
                        EmailTokenExpires = DateTime.UtcNow.AddMinutes(10),
                        PhoneNoTokenExpires = DateTime.UtcNow.AddMinutes(10),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10),
                        VerifiedAt = DateTime.UtcNow,
                        PhoneVerificationToken = CreateRandomToken(),
                        PhoneVerifiedAt = DateTime.UtcNow.AddMinutes(10),
                        PasswordResetToken = "",
                        RefreshToken = "",
                        ArchivedAccount = false,
                        SuspendedAccount = false,
                        TwoFactorRecoveryCodes = "",
                        TwoFactorRecoveryCodesHash = "",
                        TwoFactorSecret = "",
                        ActivateWallet = false,
                      
                    };
                    generalUsers = await userRepository.AddAsync(generalUsers, requvest.ExternalId!);
                    RoleType roleEnum = RoleType.User;
                    var role = await RolePermissionHelper.GetRoleAsync(roleEnum, rolesRepository);
                    if (role != null)
                    {
                        await rolesRepository.AddUserRoleAsync(new User_Role
                        {
                            GeneralUsersId = generalUsers.Id,
                            RoleId = role.Id
                        });
                    }

                    List<PermissionType> permissionTypes = [
                        PermissionType.CanManageWallet,

                    ];

                    foreach (var permEnum in permissionTypes)
                    {
                        var perm = await RolePermissionHelper.GetPermissionAsync(permEnum, rolesRepository);
                        if (perm != null)
                        {
                            await rolesRepository.AddUserPermissionAsync(new UserPermissionUsage
                            {
                                GeneralUsersId = generalUsers.Id,
                                PermissionId = perm.Id
                            });
                        }
                    }

                    var users = new UsersDataModelTable()
                    {
                        Id = generalUsers.Id,
                        FirstName = generalUsers.FirstName,
                        LastName = generalUsers.LastName,
                        Email = generalUsers.Email,
                        PhoneNo = generalUsers.PhoneNo,
                        Sex = "",
                        UserStatus = false,
                        ProfilePicture = generalUsers.ImagePath,
                        ReferralCode = CreateRandomToken11(),
                        Address = "",
                        Locality = "",
                        State = "",
                        PostCodes = "",
                        Latitude = 1.11,
                        Longitude = 1.11,
                        CreatedAt = DateTime.Now,
                        LastLoginAt = DateTime.UtcNow,
                        LastSeenAt = DateTime.UtcNow,
                        AccountName="",
                        AccountNumber="",
                        BankName="",
                        ArchivedAccount=false,
                        SuspendedAccount=false,
                        MoneyBoxBalance=0,
                    };
                    // Pass detials to repository
                    users = await newUsersRepository.AddAsync(users);


                    //Now Login Here
                    var loginUser = await userRepository.AuthenticateAsync(generalUsers.Email!, requvest.ExternalId!);

                    var userD = await newUsersRepository.GetAsync(loginUser.Id);
                    var general = await userRepository.GetAsync(userD.Id);


                    //generate jwt token
                    var token = tokenHandler.CreateTokenAsync(loginUser);
                    var refreshToken = tokenHandler.RefreshTokenAsync(loginUser.Email!);
                    var userDto = new LoginDto()
                    {
                        Token = await token,
                        RefreshToken = await refreshToken,
                        RefreshTokenExpiryTime = loginUser.RefreshTokenExpiryTime,
                        UserId = loginUser.Id,
                        Email = loginUser.Email,
                        UserType = loginUser.UserType,
                        EmailVerified = loginUser.EmailConfirmed,
                        PhoneVerified = loginUser.PhoneNoConfirmed,
                        FullName = $"{userD.FirstName} {userD.LastName}",
                        ProfileImage = userD.ProfilePicture,
                        IdAuthPassword = requvest.ExternalId,
                        Roles = general.Roles,
                        Permissions = general.Permissions,
                    };
                    return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.UserId }, userDto);
                }
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Error = "Invalid Google token", Details = ex.Message });
            }
        }

        [HttpPost("tokens/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokenRequest request)
        {
            var user = await userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return Unauthorized();

            // VALIDATE + ROTATE TOKEN
            var newRefreshToken = await tokenHandler.RotateRefreshTokenAsync(
                request.Email,
                request.RefreshToken
            );

            if (newRefreshToken == null)
            {
                var error = new ErrorModel()
                {
                    Message = "Invalid refresh token",
                    Status = true
                };
                return Unauthorized(error);
            }

            // CREATE NEW ACCESS TOKEN
            var newAccessToken = await tokenHandler.CreateTokenAsync(user);

            var userDto = new LoginDto()
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                UserId = user.Id,
                Email = user.Email,
                UserType = user.UserType,
                EmailVerified = user.EmailConfirmed,
                PhoneVerified = user.PhoneNoConfirmed,
                FullName = $"{user.FirstName} {user.LastName}",
                ProfileImage = user.ImagePath,
                IdAuthPassword = "",
                Roles = user.Roles,
                Permissions = user.Permissions,
                

            };

            return Ok(userDto);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // GET JWT
            var token = HttpContext.Request.Headers.Authorization
                .ToString()
                .Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized();

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return Unauthorized("Invalid token");

            var jwtToken = handler.ReadJwtToken(token);

            // JWT ID
            var jti = jwtToken.Id;

            // EXPIRY
            var expiry = jwtToken.ValidTo;

            // USER EMAIL
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            // BLACKLIST ACCESS TOKEN
            var tokenEx = new BlacklistedToken()
            {
                Jti = jti,
                Expiry = expiry,
                CreatedAt = DateTime.UtcNow,
            };

            await blacklistedTokenRepository.BlacklistTokenAsync(tokenEx);

            // REMOVE REFRESH TOKEN
            if (!string.IsNullOrWhiteSpace(email))
            {
                await tokenHandler.LogoutAsync(email);
            }

            return Ok(new
            {
                message = "Logged out successfully"
            });
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Super_Admin", Policy = "CanManageUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] FilterLocationData filter)
        {
            var result = await newUsersRepository.GetAllUsersAsync(filter);
            return Ok(result);
        }

        [HttpGet]
        [Route("profile")]
        [ActionName("ProfileAsync")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ProfileAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var user = await newUsersRepository.GetAsync(userGuid);
            if (user == null)
            {
                var error = new ErrorModel()
                {
                    Message = "User Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
            else
            {
                var token = await userRepository.GetAllDeviceTokenAsync();
                var userDto = mapper.Map<UsersDataModelTableDto>(user);
                userDto.DeviceTokenModels = token.Where(x => x.UserId == user.Id).ToList();
                return Ok(userDto);
            }
        } 

        [HttpGet]
        [Route("for-admin/{id:guid}")]
        [ActionName("ProfileAsync")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> ProfileAsync([FromRoute] Guid id)
        {
            var user = await newUsersRepository.GetAsync(id);
            if (user == null)
            {
                var error = new ErrorModel()
                {
                    Message = "User Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
            else
            {
                var token = await userRepository.GetAllDeviceTokenAsync();
                var userDto = mapper.Map<UsersDataModelTableDto>(user);
                userDto.DeviceTokenModels = token.Where(x => x.UserId == user.Id).ToList();
                return Ok(userDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAsync(AddUserRequest request)
        {
            // Validate the request
            var check = await ValidateAddUserAsync(request);
            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var generalUsers = new GeneralUsers()
                {
                    UserName = "",
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    UserType = RoleType.User.ToString(),
                    VerificationToken = CreateRandomToken(),
                    EmailConfirmed = false,
                    PhoneNo = request.PhoneNo,
                    ImagePath = "",
                    CreatedAt = DateTime.UtcNow,
                    LockOutEndEnabled = false,
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    LockOutEnd = DateTime.UtcNow.AddDays(30),
                    PhoneNoConfirmed = false,
                    ResetTokenExpires = DateTime.UtcNow.AddMinutes(10),
                    EmailTokenExpires = DateTime.UtcNow.AddMinutes(10),
                    PhoneNoTokenExpires = DateTime.UtcNow.AddMinutes(10),
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10),
                    VerifiedAt = DateTime.UtcNow,
                    PhoneVerificationToken = CreateRandomToken(),
                    PhoneVerifiedAt = DateTime.UtcNow.AddMinutes(10),
                    PasswordResetToken = "",
                    RefreshToken = "",
                    ActivateWallet = false,
                    ArchivedAccount = false,
                    SuspendedAccount = false,
                    TwoFactorRecoveryCodes = "",
                    TwoFactorRecoveryCodesHash = "",
                    TwoFactorSecret = "",
                };
                generalUsers = await userRepository.AddAsync(generalUsers, request.Password!);
                var users = new UsersDataModelTable()
                {
                    Id = generalUsers.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNo = generalUsers.PhoneNo,
                    Sex = "",
                    UserStatus = false,
                    ProfilePicture = generalUsers.ImagePath,
                    ReferralCode = CreateRandomToken11(),
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow,
                    LastSeenAt = DateTime.UtcNow,
                    Address = "",
                    Locality = "",
                    State = "",
                    PostCodes = "",
                    Latitude = 1.11,
                    Longitude = 1.11,
                    AccountName="",
                    AccountNumber ="",
                    BankName="",
                    MoneyBoxBalance=0,
                    ArchivedAccount = false,
                    SuspendedAccount = false,
                };
                // Pass detials to repository
                users = await newUsersRepository.AddAsync(users);

                RoleType roleEnum = RoleType.User;
                var role = await RolePermissionHelper.GetRoleAsync(roleEnum, rolesRepository);
                if (role != null)
                {
                    await rolesRepository.AddUserRoleAsync(new User_Role
                    {
                        GeneralUsersId = users.Id,
                        RoleId = role.Id
                    });
                }

                List<PermissionType> permissionTypes = [
                    PermissionType.CanManageWallet,

                    ];

                foreach (var permEnum in permissionTypes)
                {
                    var perm = await RolePermissionHelper.GetPermissionAsync(permEnum, rolesRepository);
                    if (perm != null)
                    {
                        await rolesRepository.AddUserPermissionAsync(new UserPermissionUsage
                        {
                            GeneralUsersId = users.Id,
                            PermissionId = perm.Id
                        });
                    }
                }

                // Pass detials to repository
                EmailTemplates.SendEmailVerificationCode(users.Email!, generalUsers.VerificationToken!, users.FirstName!);
                // convert back to dto
                var userDto12 = await newUsersRepository.GetAsync(users.Id);
                var token = await userRepository.GetAllDeviceTokenAsync();
                var userDto = mapper.Map<UsersDataModelTableDto>(userDto12);
                userDto.DeviceTokenModels = [.. token.Where(x => x.UserId == users.Id)];
                return Ok(userDto);
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "User,Super_Admin")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            // Get the region from the database
            var user = await userRepository.DeleteAsync(id);
            // if null NotFound
            if (user == null)
            {
                var error = new ErrorModel()
                {
                    Message = "User Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }

            else
            {
                await newUsersRepository.DeleteAsync(id);
                var token = await userRepository.GetAllDeviceTokenAsync();
                token = token.Where(x => x.UserId == user.Id);
                foreach (var item in token)
                {
                    await userRepository.DeleteDeviceTokenModelAsync(item.DeviceTokenId!);
                }
                var error = new ErrorModel()
                {
                    Message = "Deleted Successfully",
                    Status = true
                };
                return Ok(error);
            }
        }

       
        [HttpPut]
        [Route("update-user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var userDto1 = await newUsersRepository.GetAsync(userGuid);
            if (userDto1 == null)
            {
                var error = new ErrorModel()
                {
                    Message = "User Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
            else
            {
                var user = new UsersDataModelTable()
                {
                    Sex = !string.IsNullOrWhiteSpace(request.Sex) ? request.Sex : userDto1.Sex,
                    FirstName = !string.IsNullOrWhiteSpace(request.FirstName) ? request.FirstName : userDto1.FirstName,
                    LastName = !string.IsNullOrWhiteSpace(request.LastName) ? request.LastName : userDto1.LastName,
                    PhoneNo = !string.IsNullOrWhiteSpace(request.PhoneNo) ? request.PhoneNo : userDto1.PhoneNo,
                    Address = !string.IsNullOrWhiteSpace(request.Address) ? request.Address : userDto1.Address,
                    ProfilePicture = !string.IsNullOrWhiteSpace(request.ProfilePicture) ? request.ProfilePicture : userDto1.ProfilePicture,
                    PostCodes = !string.IsNullOrWhiteSpace(request.PostCodes) ? request.PostCodes : userDto1.PostCodes,
                    Locality = !string.IsNullOrWhiteSpace(request.Locality) ? request.Locality : userDto1.Locality,
                    State = !string.IsNullOrWhiteSpace(request.State) ? request.State : userDto1.State,
                    ReferralCode = userDto1.ReferralCode,
                    LastLoginAt = userDto1.LastLoginAt,
                    LastSeenAt = userDto1.LastSeenAt,
                    CreatedAt = userDto1.CreatedAt,
                    Latitude = (double)(request.Latitude ?? userDto1.Latitude),
                    Longitude = (double)(request.Longitude ?? userDto1.Longitude),
                    UserStatus = (bool)(request.UserStatus ?? userDto1.UserStatus),
                    MoneyBoxBalance = userDto1.MoneyBoxBalance,
                    AccountName = userDto1.AccountName,
                    AccountNumber = userDto1.AccountNumber,
                    BankName = userDto1.BankName,
                };

                // Update detials to repository
                user = await newUsersRepository.UpdateAsync(userDto1.Id, user);
                var generalUsers = new GeneralUsers()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNo = user.PhoneNo,
                    ImagePath = user.ProfilePicture,
                };
                await userRepository.UpdateAsync(userDto1.Id, generalUsers);
                // convert back to dto
                var userDto12 = await newUsersRepository.GetAsync(user.Id);
                var token = await userRepository.GetAllDeviceTokenAsync();
                var userDto = mapper.Map<UsersDataModelTableDto>(userDto12);
                userDto.DeviceTokenModels = token.Where(x => x.UserId == user.Id).ToList();
                return Ok(userDto);

            }

        }

        [HttpPut]
        [Route("update-user-money-box")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUserMoneyBoxAsync([FromQuery] string method, [FromBody] UpdateMoneyBox request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var userDto1 = await newUsersRepository.GetAsync(userGuid);
            if (userDto1 == null)
            {
                var error = new ErrorModel()
                {
                    Message = "User Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
            else
            {
                var user = new UsersDataModelTable()
                {
                    Sex =  userDto1.Sex,
                    FirstName =  userDto1.FirstName,
                    LastName =  userDto1.LastName,
                    PhoneNo = userDto1.PhoneNo,
                    Address = userDto1.Address,
                    ProfilePicture = userDto1.ProfilePicture,
                    PostCodes = userDto1.PostCodes,
                    Locality = userDto1.Locality,
                    State = userDto1.State,
                    ReferralCode = userDto1.ReferralCode,
                    LastLoginAt = userDto1.LastLoginAt,
                    LastSeenAt = userDto1.LastSeenAt,
                    CreatedAt = userDto1.CreatedAt,
                    Latitude =  userDto1.Latitude,
                    Longitude =  userDto1.Longitude,
                    UserStatus =  userDto1.UserStatus,
                    MoneyBoxBalance = method=="Adding"? userDto1.MoneyBoxBalance + request.MoneyBoxBalance:
                    userDto1.MoneyBoxBalance-request.MoneyBoxBalance,
                    AccountName = userDto1.AccountName,
                    AccountNumber = userDto1.AccountNumber,
                    BankName = userDto1.BankName,
                };

                // Update detials to repository
                user = await newUsersRepository.UpdateAsync(userDto1.Id, user);
         
                // convert back to dto
                var userDto12 = await newUsersRepository.GetAsync(user.Id);
                var token = await userRepository.GetAllDeviceTokenAsync();
                var userDto = mapper.Map<UsersDataModelTableDto>(userDto12);
                userDto.DeviceTokenModels = token.Where(x => x.UserId == user.Id).ToList();
                return Ok(userDto);

            }

        }


        [HttpGet]
        [Authorize]
        [Route("delivery-address")]
        public async Task<IActionResult> GetAllUsersDeliveryAdressAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var token = await newUsersRepository.GetAllDeliveryAsync(userGuid);
            var userDto = mapper.Map<List<DeliveryAddressDto>>(token);
            return Ok(userDto);
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Super_Admin")]
        [Route("delivery-address-for-admin")]
        public async Task<IActionResult> GetAllUsersDeliveryAdressAsync([FromHeader] Guid userId)
        {
            var token = await newUsersRepository.GetAllDeliveryAsync(userId);
            var userDto = mapper.Map<List<DeliveryAddressDto>>(token);
            return Ok(userDto);
        }

        [HttpPut]
        [Route("add-new-address")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddAddressAsync( [FromBody] AddDeliveryAddress request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var userDto1 = await newUsersRepository.GetAsync(userGuid);
          
            // check the null value
            if (userDto1 == null)
            {
                var error = new ErrorModel()
                {
                    Message = "User Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
            // convert back to dto
            else
            {
                // Delivery Address
                var address = new DeliveryAddress()
                {
                    UserName = request.UserName,
                    PhoneNo = request.PhoneNo,
                    Address = request.Address,
                    AddressPostCodes = request.AddressPostCodes,
                    HouseNo = request.HouseNo,
                    Locality = request.Locality,
                    State = request.State,
                    Latitude = (double)request.Latitude!,
                    Longitude = (double)request.Longitude!,
                    UsersDataModelTableId = userDto1.Id,
                    CreatedAt = DateTime.UtcNow,
                };
                await newUsersRepository.AddDeliveryAddressAsync(address);
               
              
                var userDto12 = await newUsersRepository.GetAsync(userDto1.Id);
                var userDto = mapper.Map<UsersDataModelTableDto>(userDto12);
                return Ok(userDto);
            }

        }

        [HttpPut]
        [Route("update-delivery-address/{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateDeliveryAddressAsync([FromRoute] Guid id, [FromBody] AddDeliveryAddress request)
        {
            var userDto1 = await newUsersRepository.GetAddressAsync(id);
            if (userDto1 == null)
            {
                var error = new ErrorModel()
                {
                    Message = "Address Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
            var user = new DeliveryAddress()
            {
                UserName = !string.IsNullOrWhiteSpace(request.UserName) ? request.UserName : userDto1.UserName,
                PhoneNo = !string.IsNullOrWhiteSpace(request.PhoneNo) ? request.PhoneNo : userDto1.PhoneNo,
                Address = !string.IsNullOrWhiteSpace(request.Address) ? request.Address : userDto1.Address,
                AddressPostCodes = !string.IsNullOrWhiteSpace(request.AddressPostCodes) ? request.AddressPostCodes : userDto1.AddressPostCodes,
                HouseNo = !string.IsNullOrWhiteSpace(request.HouseNo) ? request.HouseNo : userDto1.HouseNo,
                Locality = !string.IsNullOrWhiteSpace(request.Locality) ? request.Locality : userDto1.Locality,
                Latitude = (double)(request.Latitude ?? userDto1.Latitude),
                Longitude = (double)(request.Longitude?? userDto1.Longitude),
            };

            // Update detials to repository
            await newUsersRepository.UpdateDeliveryAddressAsync(id, user);

            // check the null value
            var userDto12 = await newUsersRepository.GetAsync(userDto1.Id);
            var userDto = mapper.Map<UsersDataModelTableDto>(userDto12);
            return Ok(userDto);

        }

        [HttpDelete]
        [Route("delete-delivery-address/{id:guid}")]
        [Authorize(Roles = "User,Super_Admin")]
        public async Task<IActionResult> DeleteDeliveryAddressAsync([FromRoute] Guid id)
        {
            var user = await newUsersRepository.DeleteDeliveryAddressAsync(id);
            if (user == null)
            {
                var error = new ErrorModel()
                {
                    Message = "User Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
            else
            {
                var userDto = new ResponseModel()
                {
                    Message = "Deleted Successfully",
                    Status = true,
                };
                return Ok(userDto);
            }
        }

        [HttpPost("email-verification")]
        public async Task<IActionResult> Verify(EmailVerification verification)
        {

            var user = await userRepository.VerifyAsync(verification.Token!);
            if (user == null)
            {
                return BadRequest("Invalid token.");
            }
            else
            {
                user = await userRepository.AuthenticateAsync(user.Email!, verification.Password!);
                var userD = await newUsersRepository.GetAsync(user.Id);
                if (user != null)
                {
                    if (user.UserType != "User")
                    {
                        return BadRequest("Not A User Account");
                    }
                    else
                    {
                        //generate jwt token
                        var token = tokenHandler.CreateTokenAsync(user);
                        var refreshToken = tokenHandler.RefreshTokenAsync(user.Email!);
                        // var userId = userRepository.Userd;

                        var userDto = new LoginDto()
                        {

                            Token = await token,
                            RefreshToken = await refreshToken,
                            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                            UserId = user.Id,
                            Email = user.Email,
                            UserType = user.UserType,
                            EmailVerified = user.EmailConfirmed,
                            PhoneVerified = user.PhoneNoConfirmed,
                            FullName = $"{userD.FirstName} {userD.LastName}",
                            ProfileImage=userD.ProfilePicture
                        };
                        return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.UserId }, userDto);
                    }
                }
                else
                {
                    return BadRequest("InValid Password");
                }
            }

        }

        [HttpPost("forgot-password-request-token")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordRequest email)
        {
            var user = await userRepository.ForgetPasswordAsync(email.Email!);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            else if (user.UserType != "User")
            {
                return BadRequest("User not found.");
            }
            else
            {
                try { await EmailTemplates.SendChangePasswordCodeEmail(email.Email!, user.PasswordResetToken!, user.FirstName!); }
                catch (Exception ex) { Console.WriteLine($"Warning: Email send failed: {ex.Message}"); }
                return Ok($"Password Reset token has been Sent to your Email");
            }
        }

        [HttpPost("email-verification-request-token")]
        public async Task<IActionResult> EmailVerificationRequestToken(ForgetPasswordRequest email)
        {
            var user = await userRepository.RequestNewEmailTokenAsync(email.Email!);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            else
            {
                try { await EmailTemplates.SendEmailVerificationCode(email.Email!, user.VerificationToken!, user.LastName!); }
                catch (Exception ex) { Console.WriteLine($"Warning: Email send failed: {ex.Message}"); }
                return Ok($"New token has been Sent to your Email");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResettPassword(ResetPasswordRequest request)
        {

            var user = await userRepository.PasswordResetAsync(request.Token, request.Password);
            if (user == null)
            {
                return BadRequest("Invalid Token.");
            }
            else
            {
                return Ok("Password successfully reset");
            }

        }

        [HttpPost("phone-no-verification")]
        public async Task<IActionResult> PhoneNoVerify(PhoneVerification otp)
        {
            var user = await userRepository.PhoneNoVerifyAsync(otp.Otp!);
            if (user == null)
            {
                return BadRequest("Invalid otp token.");
            }
            else
            {
                return Ok($"PhoneNo verified!");
            }
        }

        [HttpPost("two-factor-enabled/{id:Guid}")]
        public async Task<IActionResult> TwoFactorEnabled([FromRoute] Guid id)
        {
            var user = await userRepository.TwoFactorEnabledAsync(id);
            if (user == null)
            {
                return BadRequest("User Does Not Exist");
            }
            else
            {
                return Ok($"User Two Factor Authentication Enabled");
            }
        }

        #region private methods

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
        private async Task<bool> ValidateAddUserAsync(AddUserRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError(nameof(request), $" Add User Data Is Required");
                return false;
            }

            var user = await userRepository.UserExistAsync(request.Email!);

            if (user)
            {
                ModelState.AddModelError($"{nameof(request.Email)}", $"{nameof(request.Email)}  already Exist");
            }
            var userPhone = await userRepository.UserPhoneNoExistAsync(request.PhoneNo!);

            if (userPhone)
            {
                ModelState.AddModelError($"{nameof(request.PhoneNo)}", $"{nameof(request.PhoneNo)}  already Exist");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;

        }
        private static string CreateRandomToken11()
        {
            char[] charArr = "ABCDEFGHIJKLMNOPQLSTUVWXYZ0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new();
            for (int i = 0; i < 11; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos)!.ToString()!)) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
        }

        #endregion

    }
}
