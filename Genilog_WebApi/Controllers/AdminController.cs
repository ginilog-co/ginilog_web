using AutoMapper;
using Genilog_WebApi.EmailSender;
using Genilog_WebApi.Key;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.AdminsModel;
using Genilog_WebApi.Model.AuthModel;
using Genilog_WebApi.Model.WalletModel;
using Genilog_WebApi.Repository.AdminRepo;
using Genilog_WebApi.Repository.AuthRepo;
using Genilog_WebApi.Repository.AuthRepo.PolicyBased;
using Genilog_WebApi.Repository.UploadRepo;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;


namespace Genilog_WebApi.Controllers
{
    [Route("api/admin-controller")]
    [ApiController]
    public class AdminController(IHostEnvironment _env, IAdminRepository usersRepository, IMapper mapper, ITokenHandler tokenHandler, IRolesRepository rolesRepository,
     IUser_RoleRepository user_RoleRepository, IUploadRepository uploadRepository, 
     IGeneralUserRepository generalUserRepository, Cls_Keys keys, IBlacklistedTokenRepository blacklistedTokenRepository) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IAdminRepository usersRepository = usersRepository;
        private readonly ITokenHandler tokenHandler = tokenHandler;
        private readonly IMapper mapper = mapper;
        private readonly IRolesRepository rolesRepository = rolesRepository;
        private readonly IUser_RoleRepository user_RoleRepository = user_RoleRepository;
        private readonly IUploadRepository uploadRepository = uploadRepository;
        private readonly IGeneralUserRepository generalUserRepository = generalUserRepository;
        private readonly Cls_Keys keys = keys;
        private readonly IBlacklistedTokenRepository blacklistedTokenRepository = blacklistedTokenRepository;



        [HttpPost("user/{userId}/access")]
        [Authorize(Roles = "Super_Admin", Policy = "CanAssignRoles")]
        public async Task<IActionResult> AddAccessToUser([FromRoute] Guid userId, [FromBody] AddAccessRequest request)
        {
            // Null Body Handling
            if (request == null)
            {
                return BadRequest("Request body cannot be empty.");
            }

            if ((request.Roles == null || request.Roles.Count == 0) &&
                (request.Permissions == null || request.Permissions.Count == 0))
            {
                return BadRequest("No roles or permissions provided.");
            }

            var user = await generalUserRepository.GetAsync(userId);
            if (user == null) return NotFound("User not found");

            //-----------------------------
            // 1. ADD ROLES
            //-----------------------------
            if (request.Roles != null && request.Roles.Count != 0)
            {
                foreach (var roleEnum in request.Roles)
                {
                    var role = await RolePermissionHelper.GetRoleAsync(roleEnum, rolesRepository);

                    if (role != null)
                    {
                        var userRoles = await rolesRepository.GetUserRolesAsync(user.Id);

                        if (!userRoles.Any(r => r.Id == role.Id))
                        {
                            await rolesRepository.AddUserRoleAsync(new User_Role
                            {
                                GeneralUsersId = user.Id,
                                RoleId = role.Id
                            });
                        }
                    }
                }
            }

            //-----------------------------
            // 2. ADD PERMISSIONS
            //-----------------------------
            if (request.Permissions != null && request.Permissions.Count != 0)
            {
                foreach (var permissionEnum in request.Permissions)
                {
                    var permission = await RolePermissionHelper.GetPermissionAsync(permissionEnum, rolesRepository);

                    if (permission != null)
                    {
                        var userPermissions = await rolesRepository.GetUserPermissionsAsync(user.Id);

                        if (!userPermissions.Any(p => p.Id == permission.Id))
                        {
                            await rolesRepository.AddUserPermissionAsync(new UserPermissionUsage
                            {
                                GeneralUsersId = user.Id,
                                PermissionId = permission.Id
                            });
                        }
                    }
                }
            }

            return Ok(new
            {
                Message = "Access updated successfully."
            });
        }


        [HttpDelete("user/{userId}/roles")]
        [Authorize(Roles = "Super_Admin", Policy = "CanAssignRoles")]
        public async Task<IActionResult> RemoveRoleFromUser([FromRoute] Guid userId, [FromBody] RoleType roleToRemove)
        {
            var user = await generalUserRepository.GetAsync(userId);
            if (user == null) return NotFound("User not found");

            var role = await RolePermissionHelper.GetRoleAsync(roleToRemove, rolesRepository);
            if (role == null) return NotFound("Role does not exist");

            var userRoles = await rolesRepository.GetUserRolesAsync(userId);
            var userRoleEntry = userRoles.FirstOrDefault(r => r.Id == role.Id);

            if (userRoleEntry == null)
            {
                return BadRequest($"User does not have the '{role.Name}' role.");
            }

            await rolesRepository.RemoveUserRoleAsync(userId, role.Id);

            return Ok(new { Message = $"Role '{role.Name}' removed successfully." });
        }

        [HttpDelete("user/{userId}/permissions")]
        [Authorize(Roles = "Super_Admin", Policy = "CanAssignRoles")]
        public async Task<IActionResult> RemovePermissionFromUser([FromRoute] Guid userId, [FromBody] PermissionType permissionToRemove)
        {
            var user = await generalUserRepository.GetAsync(userId);
            if (user == null) return NotFound("User not found");

            var permission = await RolePermissionHelper.GetPermissionAsync(permissionToRemove, rolesRepository);
            if (permission == null) return NotFound("Permission does not exist");

            var userPermissions = await rolesRepository.GetUserPermissionsAsync(userId);
            var userPermissionEntry = userPermissions.FirstOrDefault(p => p.Id == permission.Id);

            if (userPermissionEntry == null)
            {
                return BadRequest($"User does not have the '{permission.Name}' permission.");
            }

            await rolesRepository.RemoveUserPermissionAsync(userId, permission.Id);

            return Ok(new { Message = $"Permission '{permission.Name}' removed successfully." });
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginRequset requset)
        {

            var user = await generalUserRepository.AuthenticateAsync(requset.Email_PhoneNo!, requset.Password!);
            if (user != null)
            {
                var userD = await usersRepository.GetAsync(user.Id);
                if (user.UserType == "Super_Admin"|| user.UserType == "Admin" )
                {
                    //generate jwt token
                    var token = tokenHandler.CreateTokenAsync(user);
                    var refreshToken = tokenHandler.RefreshTokenAsync(user.Email!);
                    // var userId = usersRepository.Userd;
                    var userDto = new AdminLoginDto()
                    {
                        Token = await token,
                        RefreshToken = await refreshToken,
                        RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                        UserId = user.Id,
                        Email = user.Email,
                        UserType = user.UserType,
                        EmailVerified = user.EmailConfirmed,
                        PhoneVerified = user.PhoneNoConfirmed,
                        FullName = $"{user.FirstName} {user.LastName}",
                        State = userD.State,
                        Locality = userD.Locality,
                        Address = userD.Address,
                        Branch = userD.Branch,
                    };
                    return Ok(userDto);

                }
                else
                {
                    var error = new ErrorModel()
                    {
                        Message = "Not An Admin Account",
                        Status = true
                    };
                    return BadRequest(error);
                }
            }
            else
            {
                var error = new ErrorModel()
                {
                    Message = "Admin Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
        }

        [HttpPost]
        [Route("login-manager")]
        public async Task<IActionResult> LoginManagerAsync(LoginRequset requset)
        {

            var user = await generalUserRepository.AuthenticateAsync(requset.Email_PhoneNo!, requset.Password!);
            if (user != null)
            {
                var userD = await usersRepository.GetAsync(user.Id);
                if (user.UserType == "Manager" || user.UserType == "Staff_Admin" || user.UserType == "Staff")
                {
                    //generate jwt token
                    var token = tokenHandler.CreateTokenAsync(user);
                    var refreshToken = tokenHandler.RefreshTokenAsync(user.Email!);
                    // var userId = usersRepository.Userd;
                    var userDto = new AdminLoginDto()
                    {
                        Token = await token,
                        RefreshToken = await refreshToken,
                        RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                        UserId = user.Id,
                        Email = user.Email,
                        UserType = user.UserType,
                        EmailVerified = user.EmailConfirmed,
                        PhoneVerified = user.PhoneNoConfirmed,
                        FullName = $"{user.FirstName} {user.LastName}",
                        State = userD.State,
                        Locality = userD.Locality,
                        Address = userD.Address,
                        Branch = userD.Branch,
                    };
                    return Ok(userDto);

                }
                else
                {
                    var error = new ErrorModel()
                    {
                        Message = "Not An Company Account",
                        Status = true
                    };
                    return BadRequest(error);
                }
            }
            else
            {
                var error = new ErrorModel()
                {
                    Message = "Admin Does not Exist",
                    Status = true
                };
                return BadRequest(error);
            }
        }

        [HttpPost("tokens/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokenRequest request)
        {
            var user = await generalUserRepository.GetByEmailAsync(request.Email);

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
            var newAccessToken = tokenHandler.CreateTokenAsync(user);

            return Ok(new
            {
                token = newAccessToken,
                refreshToken = newRefreshToken,
            });
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



        [HttpPost]
        [Authorize(Roles = "Super_Admin", Policy = "CanCreateAdmin")]
        public async Task<IActionResult> AddMainAdminAsync(AddAdminRequest request)
        {
            // Validate request
            var isValid = await ValidateAddUserAsync(request);
            if (!isValid)
                return BadRequest(ModelState);

            // Ensure AdminType is valid enum
            if (!Enum.TryParse<RoleType>(request.AdminType, out var roleEnums))
            {
                return BadRequest(new ErrorModel
                {
                    Message = "Invalid Admin Type",
                    Status = true
                });
            }

            // Create GeneralUsers entity
            var admin = new GeneralUsers
            {
                FirstName = request.FirstName,
                LastName = request.SurName,
                Email = request.Email,
                UserType = roleEnums.ToString(),
                PhoneNo = request.PhoneNo,
                VerificationToken = CreateRandomToken(),
                EmailConfirmed = true,
                ImagePath = "",
                CreatedAt = DateTime.UtcNow,
                LockOutEndEnabled = false,
                AccessFailedCount = 0,
                TwoFactorEnabled = false,
                LockOutEnd = DateTime.UtcNow.AddDays(30),
                PhoneNoConfirmed = true,
                ResetTokenExpires = DateTime.UtcNow.AddMinutes(10),
                EmailTokenExpires = DateTime.UtcNow.AddMinutes(10),
                PhoneNoTokenExpires = DateTime.UtcNow.AddMinutes(10),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10),
                VerifiedAt = DateTime.UtcNow,
                PhoneVerificationToken = CreateRandomToken(),
                PhoneVerifiedAt = DateTime.UtcNow.AddMinutes(10),
                PasswordResetToken = "",
                RefreshToken = "",
                UserName="",
                ActivateWallet=false,
                ArchivedAccount=false,
                SuspendedAccount=false,
                TwoFactorSecret="",
                TwoFactorRecoveryCodes="",
                TwoFactorRecoveryCodesHash =""
            };

            admin = await generalUserRepository.AddAsync(admin, request.Password!);

            // Create AdminModelTable entity
            var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
            var userTable = new AdminModelTable
            {
                Id = admin.Id,
                Sex = request.Sex,
                ImagePath = "",
                FirstName = request.FirstName,
                SurName = request.SurName,
                Email = request.Email,
                StaffCode = request.StaffCode,
                PhoneNo = request.PhoneNo,
                Address = request.Address,
                Branch = request.Branch,
                Locality = request.Locality,
                State = request.State,
                AdminType = request.AdminType,
                DatePublished = date,
                CompanyType=request.CompanyType,
                CompanyName = request.CompanyName,
                CompanyUserName = request.CompanyUserName,
                ManagerId=admin.Id,
                CreatedAt = DateTime.UtcNow
            };

            userTable = await usersRepository.AddAsync(userTable);

            // Assign multiple roles
            foreach (var roleEnum in request.Roles)
            {
                var role = await RolePermissionHelper.GetRoleAsync(roleEnum, rolesRepository);
                if (role != null)
                {
                    await rolesRepository.AddUserRoleAsync(new User_Role
                    {
                        GeneralUsersId = userTable.Id,
                        RoleId = role.Id
                    });
                }
            }

            // Assign multiple permissions
            foreach (var permEnum in request.Permissions)
            {
                var perm = await RolePermissionHelper.GetPermissionAsync(permEnum, rolesRepository);
                if (perm != null)
                {
                    await rolesRepository.AddUserPermissionAsync(new UserPermissionUsage
                    {
                        GeneralUsersId = userTable.Id,
                        PermissionId = perm.Id
                    });
                }
            }

            // Convert to DTO
            var userDto = new AdminModelTableDto
            {
                Id = userTable.Id,
                SurName = userTable.SurName,
                FirstName = userTable.FirstName,
                Email = userTable.Email,
                Sex = userTable.Sex,
                ImagePath = userTable.ImagePath,
                StaffCode = userTable.StaffCode,
                Address = userTable.Address,
                Branch = userTable.Branch,
                Locality = userTable.Locality,
                State = userTable.State,
                AdminType = admin.UserType,
                PhoneNo = userTable.PhoneNo,
                DatePublished = userTable.DatePublished,
                
                CreatedAt = userTable.CreatedAt
            };

            return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.Id }, userDto);
        }

        [HttpGet]
        [Authorize(Roles = "Super_Admin", Policy = "CanViewAdmin")]
        public async Task<IActionResult> GetAllAdminUsers([FromQuery] FilterLocationData filter)
        {
            var result = await usersRepository.GetAllAdminAsync(filter);
            return Ok(result);
        }

        [HttpGet]
        [Route("profile")]
        [ActionName("ProfileAsync")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> ProfileAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var user = await usersRepository.GetAsync(userGuid);
            if (user == null)
            {
                return BadRequest("Admin Does not Exist");
            }
            else
            {
                var userDto = mapper.Map<AdminModelTableDto>(user);
                return Ok(userDto);
            }

        }
       
        // For Admin and Manager
        [HttpGet]
        [Route("profile/{id:guid}")]
        [ActionName("ProfileAsync")]
        [Authorize(Roles = "Admin,Super_Admin,Manager,Staff_Admin,Staff")]
        public async Task<IActionResult> ProfileAsync([FromRoute] Guid id)
        {
            var user = await usersRepository.GetAsync(id);
            if (user == null)
            {
                return BadRequest("Admin Does not Exist");
            }
            else
            {
                var userDto = mapper.Map<AdminModelTableDto>(user);
                return Ok(userDto);
            }

        }


        [HttpPost]
        [Route("add-manager")]
        public async Task<IActionResult> AddManagernAsync(AddAdminRequest request)
        {
            // Validate the request
            var check = await ValidateAddUserAsync(request);

            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                // Ensure AdminType is valid enum
                if (!Enum.TryParse<RoleType>(request.AdminType, out var roleEnums))
                {
                    return BadRequest(new ErrorModel
                    {
                        Message = "Invalid Admin Type",
                        Status = true
                    });
                }
                var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();
                var admin = new GeneralUsers()
                {
                    FirstName = request.FirstName,
                    LastName = request.SurName,
                    Email = request.Email,
                    UserType = request.AdminType,
                    PhoneNo = request.PhoneNo,
                    VerificationToken = CreateRandomToken(),
                    EmailConfirmed = true,
                    ImagePath = "",
                    CreatedAt = DateTime.UtcNow,
                    LockOutEndEnabled = false,
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    LockOutEnd = DateTime.UtcNow.AddDays(30),
                    PhoneNoConfirmed = true,
                    ResetTokenExpires = DateTime.UtcNow.AddMinutes(10),
                    EmailTokenExpires = DateTime.UtcNow.AddMinutes(10),
                    PhoneNoTokenExpires = DateTime.UtcNow.AddMinutes(10),
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10),
                    VerifiedAt = DateTime.UtcNow,
                    PhoneVerificationToken = CreateRandomToken(),
                    PhoneVerifiedAt = DateTime.UtcNow.AddMinutes(10),
                    PasswordResetToken = "",
                    RefreshToken = "",
                    UserName = "",
                    ActivateWallet = false,
                    ArchivedAccount = false,
                    SuspendedAccount = false,
                    TwoFactorSecret = "",
                    TwoFactorRecoveryCodes = "",
                    TwoFactorRecoveryCodesHash = ""
                };
                admin = await generalUserRepository.AddAsync(admin, request.Password!);
                var users = new AdminModelTable()
                {
                    Id = admin.Id,
                    Sex = request.Sex,
                    ImagePath = "",
                    FirstName = request.FirstName,
                    SurName = request.SurName,
                    Email = request.Email,
                    StaffCode = request.StaffCode,
                    PhoneNo = request.PhoneNo,
                    Address = request.Address,
                    Branch = request.Branch,
                    Locality = request.Locality,
                    State = request.State,
                    CompanyName = request.CompanyName,
                    CompanyUserName = request.CompanyUserName,
                    CompanyType = request.CompanyType,
                    AdminType = request.AdminType,
                    ManagerId = admin.Id,
                    DatePublished = date,
                    
                    CreatedAt = DateTime.UtcNow

                };
                // Pass detials to repository
                users = await usersRepository.AddAsync(users);

                // Assign multiple roles
                foreach (var roleEnum in request.Roles)
                {
                    var role = await RolePermissionHelper.GetRoleAsync(roleEnum, rolesRepository);
                    if (role != null)
                    {
                        await rolesRepository.AddUserRoleAsync(new User_Role
                        {
                            GeneralUsersId = users.Id,
                            RoleId = role.Id
                        });
                    }
                }

                // Assign multiple permissions
                foreach (var permEnum in request.Permissions)
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
                // convert back to dto
                var userDto = new AdminModelTableDto()
                {
                    Id = users.Id,
                    SurName = users.SurName,
                    FirstName = users.FirstName,
                    Email = users.Email,
                    Sex = users.Sex,
                    ImagePath = users.ImagePath,
                    StaffCode = users.StaffCode,
                    Address = users.Address,
                    Branch = users.Branch,
                    Locality = users.Locality,
                    State = users.State,
                    AdminType = admin.UserType,
                    PhoneNo = users.PhoneNo,
                    CompanyName = users.CompanyName,
                    CompanyUserName = users.CompanyUserName,
                    CompanyType = users.CompanyType,
                    ManagerId = users.ManagerId,
                    DatePublished = users.DatePublished,
                    CreatedAt = users.CreatedAt,
                };
                return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.Id }, userDto);

            }
        }

        [HttpPost]
        [Route("staff-admin")]
        [Authorize(Roles = "Super_Admin,Manager,Admin")]
        public async Task<IActionResult> AddStaffAdminAsync(AddAdminRequest request)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var user = await usersRepository.GetAsync(userGuid);
            if (user == null)
            {
                return BadRequest("Admin Does not Exist");
            }
            else
            {
                // Ensure AdminType is valid enum
                if (!Enum.TryParse<RoleType>(request.AdminType, out var roleEnums))
                {
                    return BadRequest(new ErrorModel
                    {
                        Message = "Invalid Admin Type",
                        Status = true
                    });
                }
                if (request.AdminType == "Staff_Admin" || request.AdminType == "Staff")
                {
                    var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
                    var timeStamp = Timestamp.GetCurrentTimestamp();
                    var admin = new GeneralUsers()
                    {
                        FirstName = request.FirstName,
                        LastName = request.SurName,
                        Email = request.Email,
                        UserType = request.AdminType,
                        PhoneNo = request.PhoneNo,
                        VerificationToken = CreateRandomToken(),
                        EmailConfirmed = true,
                        ImagePath = "",
                        CreatedAt = DateTime.UtcNow,
                        LockOutEndEnabled = false,
                        AccessFailedCount = 0,
                        TwoFactorEnabled = false,
                        LockOutEnd = DateTime.UtcNow.AddDays(30),
                        PhoneNoConfirmed = true,
                        ResetTokenExpires = DateTime.UtcNow.AddMinutes(10),
                        EmailTokenExpires = DateTime.UtcNow.AddMinutes(10),
                        PhoneNoTokenExpires = DateTime.UtcNow.AddMinutes(10),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10),
                        VerifiedAt = DateTime.UtcNow,
                        PhoneVerificationToken = CreateRandomToken(),
                        PhoneVerifiedAt = DateTime.UtcNow.AddMinutes(10),
                        PasswordResetToken = "",
                        RefreshToken = "",
                        UserName = "",
                        ActivateWallet = false,
                        ArchivedAccount = false,
                        SuspendedAccount = false,
                        TwoFactorSecret = "",
                        TwoFactorRecoveryCodes = "",
                        TwoFactorRecoveryCodesHash = ""
                    };
                    admin = await generalUserRepository.AddAsync(admin, request.Password!);
                    var users = new AdminModelTable()
                    {
                        Id = admin.Id,
                        Sex = request.Sex,
                        ImagePath = "",
                        FirstName = request.FirstName,
                        SurName = request.SurName,
                        Email = request.Email,
                        StaffCode = request.StaffCode,
                        PhoneNo = request.PhoneNo,
                        Address = request.Address,
                        Branch = request.Branch,
                        Locality = request.Locality,
                        State = request.State,
                        CompanyName = user.CompanyName,
                        CompanyUserName = user.CompanyUserName,
                        CompanyType = user.CompanyType,
                        AdminType = request.AdminType,
                        ManagerId = user.Id,
                        DatePublished = date,
                        CreatedAt = DateTime.UtcNow

                    };
                    // Pass detials to repository
                    users = await usersRepository.AddAsync(users);

                    // Assign multiple roles
                    foreach (var roleEnum in request.Roles)
                    {
                        var role = await RolePermissionHelper.GetRoleAsync(roleEnum, rolesRepository);
                        if (role != null)
                        {
                            await rolesRepository.AddUserRoleAsync(new User_Role
                            {
                                GeneralUsersId = users.Id,
                                RoleId = role.Id
                            });
                        }
                    }

                    // Assign multiple permissions
                    foreach (var permEnum in request.Permissions)
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

                    // convert back to dto
                    var userDto = new AdminModelTableDto()
                    {
                        Id = users.Id,
                        SurName = users.SurName,
                        FirstName = users.FirstName,
                        Email = users.Email,
                        Sex = users.Sex,
                        ImagePath = users.ImagePath,
                        StaffCode = users.StaffCode,
                        Address = users.Address,
                        Branch = users.Branch,
                        Locality = users.Locality,
                        State = users.State,
                        AdminType = admin.UserType,
                        PhoneNo = users.PhoneNo,
                        CompanyName = users.CompanyName,
                        CompanyUserName = users.CompanyUserName,
                        CompanyType = users.CompanyType,
                        ManagerId = users.ManagerId,
                        DatePublished = users.DatePublished,
                        CreatedAt = users.CreatedAt,
                    };
                    return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.Id }, userDto);
                }
                else
                {
                    var error = new ErrorModel()
                    {
                        Message = "Invalid Admin Type",
                        Status = true
                    };
                    return BadRequest(error);
                }
            }
        }

        [HttpPut]
        [Route("update")]
        [Authorize(Roles = "Super_Admin,Admin")]
        public async Task<IActionResult> UpdateAdminAsync( [FromBody] UpdateAdminRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var userDto = await usersRepository.GetAsync(userGuid);
            if (userDto == null)
            {
                return BadRequest("Admin Does not Exist");
            }
           var user = new AdminModelTable()
               {
                SurName = !string.IsNullOrWhiteSpace(request.SurName) ? request.SurName : userDto.SurName,
                FirstName = !string.IsNullOrWhiteSpace(request.FirstName) ? request.FirstName : userDto.FirstName,
                PhoneNo = !string.IsNullOrWhiteSpace(request.PhoneNo) ? request.PhoneNo : userDto.PhoneNo,
                StaffCode = !string.IsNullOrWhiteSpace(request.StaffCode) ? request.StaffCode : userDto.StaffCode,
                Address = !string.IsNullOrWhiteSpace(request.Address) ? request.Address : userDto.Address,
                Branch = !string.IsNullOrWhiteSpace(request.Branch) ? request.Branch : userDto.Branch,
                Locality = !string.IsNullOrWhiteSpace(request.Locality) ? request.Locality : userDto.Locality,
                State = !string.IsNullOrWhiteSpace(request.State) ? request.State : userDto.State,
                ImagePath=!string.IsNullOrWhiteSpace(request.ImagePath) ? request.ImagePath : userDto.ImagePath,
                CompanyName =  userDto.CompanyName,
                CompanyUserName = userDto.CompanyUserName,
                CompanyType = userDto.CompanyType,
                ManagerId = userDto.ManagerId,
                DatePublished = userDto.DatePublished,
                CreatedAt = userDto.CreatedAt,
                AdminType = userDto.AdminType,
                Email = userDto.Email,
                Sex = userDto.Sex,

           };
            // Update detials to repository
            user = await usersRepository.UpdateAsync(userDto.Id, user);
           var gen = new GeneralUsers()
            {
                LastName =  user.SurName,
                FirstName =  user.FirstName,
                PhoneNo = user.PhoneNo,
                ImagePath = user.ImagePath,
            };
          var t=  await generalUserRepository.UpdateAsync(user.Id, gen);
            // check the null value
            var userDto2 = new AdminModelTableDto()
            {
                Id = user.Id,
                SurName = user.SurName,
                FirstName = user.FirstName,
                Email = user.Email,
                Sex = user.Sex,
                ImagePath = user.ImagePath,
                StaffCode = user.StaffCode,
                Address = user.Address,
                Branch = user.Branch,
                Locality = user.Locality,
                State = user.State,
                AdminType = t.UserType,
                PhoneNo = user.PhoneNo,
                CompanyName = user.CompanyName,
                CompanyUserName = user.CompanyUserName,
                CompanyType = user.CompanyType,
                ManagerId = user.ManagerId,
                DatePublished = user.DatePublished,
                CreatedAt = user.CreatedAt,
            };
            return Ok(userDto2);
        }



        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Super_Admin")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            // Get the region from the database
            var user = await generalUserRepository.DeleteAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("User Does not Exist");
            }

            else
            {
                await usersRepository.DeleteAsync(id);
                return Ok("Deleted Sucessfully");
            }


        }


        [HttpPut]
        [Route("update-device-token")]
        [Authorize]
        public async Task<IActionResult> UpdateDeviceTokenAsync( [FromBody] AddDeviceToken request)
        {
            
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var user = await generalUserRepository.GetAsync(userGuid);

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

                var check = await generalUserRepository.DeviceTokenExistAsync(request.DeviceToken!);
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
                    drviceToken = await generalUserRepository.AddDeviceTokenModelAsync(drviceToken);
                    return Ok(drviceToken);
                }

            }

        }

     
        [HttpPost("forgot-password-request-token")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordRequest request)
        {
            var user = await generalUserRepository.ForgetPasswordAsync(request.Email!);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            else
            {
                try {await EmailTemplates.SendChangePasswordCodeEmail(user.Email!, user.PasswordResetToken!, user.FirstName!); }
                catch (Exception ex) { Console.WriteLine($"Warning: Email send failed: {ex.Message}"); }
                return Ok("Password Reset token has been Sent to your Email and \n");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResettPassword(ResetPasswordRequest request)
        {

            var user = await generalUserRepository.PasswordResetAsync(request.Token, request.Password);
            if (user == null)
            {
                return BadRequest("Invalid Token.");
            }
            else
            {
                return Ok("Password successfully reset");
            }

        }

        // ADVERT LINE
        [HttpGet]
        [Route("advert")]
        [Authorize]
        public async Task<IActionResult> GetAllAirlinesAsync()
        {
            var events = await usersRepository.GetAllAdvertAsync();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }

            var user = await generalUserRepository.GetAsync(userGuid);
            if (user.UserType == "User")
            {
                var now = DateTime.UtcNow;
                events = [.. events.Where(x => x.ExpiredAt > now && x.TransStatus==true).OrderByDescending(x => x.CreatedAt)];
                var userDto = mapper.Map<List<AdvertHolderModelDto>>(events);
                return Ok(userDto);
            }
            else if (user.UserType == "Manager" || user.UserType == "StaffAdmin" || user.UserType == "Staff")
            {
                var admin = await usersRepository.GetAsync(userGuid);
                events = [.. events.Where(x =>  x.AdminId == admin.ManagerId).OrderByDescending(x => x.CreatedAt)];
                var userDto = mapper.Map<List<AdvertHolderModelDto>>(events);
                return Ok(userDto);
            }
            else
            {
                events = [.. events.OrderByDescending(x => x.CreatedAt)];
                var userDto = mapper.Map<List<AdvertHolderModelDto>>(events);
                return Ok(userDto);

            }
        }

        [HttpGet]
        [Route("advert/{id:guid}")]
        [ActionName("GetAdvertAsync")]
        [Authorize]
        public async Task<IActionResult> GetAdvertAsync(Guid id)
        {
            var contacts = await usersRepository.GetAdvertAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                var contactsDto = mapper.Map<AdvertHolderModelDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("advert/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> DeleteAdvertAsync(Guid id)
        {
            // Get the region from the database
            var user = await usersRepository.DeleteAdvertAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Advert Does not Exist");
            }

            else
            {
                var userDto = mapper.Map<AdvertHolderModelDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("advert/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin,StaffAdmin,Staff")]
        public async Task<IActionResult> UpdateAdvertAsync([FromRoute] Guid id, [FromBody] AddAdvert request)
        {
            var userDto1 = await usersRepository.GetAdvertAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("Id Does not Exist");
            }

            // convert back to dto
            else
            {
                var user = new AdvertHolderModel()
                {
                    AdvertImage = !string.IsNullOrWhiteSpace(request.AdvertImage) ? request.AdvertImage : userDto1.AdvertImage,
                    AdvertName = !string.IsNullOrWhiteSpace(request.AdvertName) ? request.AdvertName : userDto1.AdvertName,
                    AdvertType = !string.IsNullOrWhiteSpace(request.AdvertType) ? request.AdvertType : userDto1.AdvertType,
                    AdvertItemCost = request.AdvertDays4 == null || request.AdvertDays4 == 0 ? userDto1.AdvertItemCost : (int)(request.AdvertDays4 * 1000.50)!,
                    TransRef = request.AdvertDays4 == null || request.AdvertDays4 == 0 ? userDto1.TransRef :"",
                    TransStatus = (request.AdvertDays4 == null || request.AdvertDays4 == 0) && userDto1.TransStatus,
                    AdvertItemDescription = !string.IsNullOrWhiteSpace(request.AdvertItemDescription) ? request.AdvertItemDescription : userDto1.AdvertItemDescription,
                    AdvertDays4 = (request.AdvertDays4 ?? userDto1.AdvertDays4),
                    ExpiredAt= request.AdvertDays4==null|| request.AdvertDays4==0? userDto1.ExpiredAt: DateTime.Now.AddDays((int)request.AdvertDays4),
                };
                // Update detials to repository
                user = await usersRepository.UpdateAdvertAsync(id, user);
                var contact = await usersRepository.GetAdvertAsync(user.Id);
                var userDto = mapper.Map<AdvertHolderModelDto>(contact);
                return Ok(userDto);
            }
        }

        [HttpPost("advert")]
        [Authorize(Roles = "Manager,Admin,Super_Admin,StaffAdmin,Staff")]
        public async Task<IActionResult> AddAdvertAsync([FromBody] AddAdvert request)
        {
            var check = ValidateAdvert(request);
            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userId, out Guid userGuid))
                {
                    return BadRequest("Invalid User ID format.");
                }
                var admin = await usersRepository.GetAsync(userGuid);
                var contacts = new AdvertHolderModel()
                {
                    AdminId = admin.ManagerId,
                    AdvertItemId = request.AdvertItemId,
                    AdvertImage = request.AdvertImage,
                    AdvertItemDescription = request.AdvertItemDescription,
                    AdvertType = request.AdvertType,
                    AdvertName = request.AdvertName,
                    AdvertItemCost = (int)(request.AdvertDays4 * 1000.50)!,
                    AdvertDays4 = (int)request.AdvertDays4!,
                    TransRef="",
                    TransStatus = false,
                    CreatedAt = DateTime.Now,
                    ExpiredAt = DateTime.Now.AddDays((int)request.AdvertDays4!),

                };
                // Pass detials to repository
                contacts = await usersRepository.AddAdvertAsync(contacts);

                // convert back to dto
                var contact = await usersRepository.GetAdvertAsync(contacts.Id);
                var contactsDto = mapper.Map<AdvertHolderModelDto>(contact);
                return CreatedAtAction(nameof(GetAdvertAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }

        //paystack
        [HttpPut("initialize-paystack-advert-payment/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> InitializePayment([FromRoute] Guid id)
        {

            var events = await usersRepository.GetAdvertAsync(id);
            if (events == null)
            {
                return BadRequest("Accommodation Does not Exist");
            }
            else
            {
                var admin= await usersRepository.GetAsync(events.AdminId);
                var url = "https://api.paystack.co/transaction/initialize";
                var data = new
                {
                    email = admin.Email,
                    amount = (events.AdvertItemCost) * 100,  // Amount in Kobo (100 kobo = 1 Naira)
                    callback_url = $"{keys.ServerURL}/api/Admin/verify-paystack-advert-payment?orderId={events.Id}", // The URL to redirect after payment
                    channels = new[] { "card", "bank", "ussd", "mobile_money", "bank_transfer" },
                    metadata = new
                    {
                        cancel_action = $"{keys.ServerURL}/api/paystack-redirect?status=cancelled"
                    }
                };

                using var httpClient = new HttpClient();

                StringContent content = new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keys.PaystackSecretKey);
                using var response = await httpClient.PostAsync($"{url}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var paystackResponse = JsonConvert.DeserializeObject<PaystackResponse>(apiResponse);

                    return Ok(paystackResponse);
                }
                else
                {
                    var error = new ErrorModel()
                    {
                        Message = $"{apiResponse}",
                        Status = true
                    };
                    return BadRequest(error);
                }

            }

        }

        [HttpGet("verify-paystack-advert-payment")]
        public async Task<IActionResult> VerifyPharmacyProductOrderPayment([FromQuery] Guid orderId, [FromQuery] string trxref, [FromQuery] string reference)
        {

            var url = $"https://api.paystack.co/transaction/verify/{reference}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keys.PaystackSecretKey);
            using var response = await httpClient.GetAsync($"{url}");
            string apiResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var paystackResponse = new PaymentSucessData()
                {
                    Status = true,
                    Message = "Payment Made Sucessfully",
                    AccessCode = trxref,
                    Reference = reference
                };

                var tronData = await usersRepository.GetAdvertAsync(orderId);
                var user = new AdvertHolderModel()
                {

                    AdminId = tronData.AdminId,
                    AdvertItemId = tronData.AdvertItemId,
                    AdvertImage = tronData.AdvertImage,
                    AdvertItemDescription = tronData.AdvertItemDescription,
                    AdvertType = tronData.AdvertType,
                    TransRef = $"Paystack_{paystackResponse.Reference }",
                    AdvertName = tronData.AdvertName,
                    TransStatus = paystackResponse.Status,
                    AdvertItemCost = tronData.AdvertItemCost,
                    AdvertDays4 = tronData.AdvertDays4,
                    ExpiredAt = tronData.ExpiredAt,
                };

                // Update detials to repository
               await usersRepository.UpdateAdvertAsync(tronData.Id, user);
                return Ok(paystackResponse);
            }
            else
            {
                var error = new ErrorModel()
                {
                    Message = $"{apiResponse}",
                    Status = true
                };
                return BadRequest(error);
            }
        }


        //flutterwave
        [HttpPut("initialize-flutterwave-advert-payment/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> InitializeFlutterwavePayment([FromRoute] Guid id)
        {
            var events = await usersRepository.GetAdvertAsync(id);

            if (events == null)
            {
                return BadRequest("Accommodation Does not Exist");
            }
            else
            {
                var admin = await usersRepository.GetAsync(events.AdminId);
                var url = "https://api.flutterwave.com/v3/payments";
                var data = new
                {
                    tx_ref = CreateRandomTokenSix(),
                    amount = events.AdvertItemCost,  // Amount in Kobo (100 kobo = 1 Naira)
                    customer = new
                    {
                        email = admin.Email,
                        name = admin.FirstName + " " + admin.SurName,
                    },
                    currency = "NGN",
                    redirect_url = $"{keys.ServerURL}/api/Admin/verify-flutterwave-advert-payment?orderId={events.Id}", // The URL to redirect after payment
                    customizations = new
                    {
                        title = "My App Payment",
                        description = "Payment for Advert Item"
                    }
                };


                using var httpClient = new HttpClient();

                StringContent content = new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keys.FlutterwaveSecretKey);
                using var response = await httpClient.PostAsync($"{url}", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var paystackResponse = JsonConvert.DeserializeObject<FlutterwaveResponse>(apiResponse);
                    return Ok(paystackResponse);
                }
                else
                {
                    var error = new ErrorModel()
                    {
                        Message = $"{apiResponse}",
                        Status = true
                    };
                    return BadRequest(error);
                }

            }

        }

        [HttpGet("verify-flutterwave-advert-payment")]
        public async Task<IActionResult> VerifyFlutterwavePayment([FromQuery] Guid orderId, [FromQuery] string status, [FromQuery] string tx_ref, [FromQuery] string transaction_id)
        {
            var url = $"https://api.flutterwave.com/v3/transactions/{transaction_id}/verify";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keys.FlutterwaveSecretKey);
            using var response = await httpClient.GetAsync($"{url}");
            string apiResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var paystackResponse = new PaymentSucessData()
                {
                    Status = true,
                    Message = "Payment Made Sucessfully",
                    AccessCode = transaction_id,
                    Reference = tx_ref,
                    PaymentStatus = status
                };

                if (paystackResponse.PaymentStatus == "completed" || paystackResponse.PaymentStatus == "successful")
                {
                    var tronData = await usersRepository.GetAdvertAsync(orderId);
                    var user = new AdvertHolderModel()
                    {

                        AdminId = tronData.AdminId,
                        AdvertItemId = tronData.AdvertItemId,
                        AdvertImage = tronData.AdvertImage,
                        AdvertItemDescription = tronData.AdvertItemDescription,
                        AdvertType = tronData.AdvertType,
                        TransRef = $"Flutterwave_{paystackResponse.Reference}",
                        AdvertName = tronData.AdvertName,
                        TransStatus = paystackResponse.Status,
                        AdvertItemCost = tronData.AdvertItemCost,
                        AdvertDays4 = tronData.AdvertDays4,
                        ExpiredAt = tronData.ExpiredAt,
                    };

                    // Update detials to repository
                    await usersRepository.UpdateAdvertAsync(tronData.Id, user);
                    // TODO: Mark payment as successful in DB
                    return Redirect($"{keys.ServerURL}/api/flutterwave-redirect?status={paystackResponse.PaymentStatus}");
                }
                else
                {

                    return Redirect($"{keys.ServerURL}/api/flutterwave-redirect?status={paystackResponse.PaymentStatus}");
                }

            }
            else
            {
                var error = new ErrorModel()
                {
                    Message = $"{apiResponse}",
                    Status = true
                };
                return BadRequest(error);
            }
        }



        // Company Apply Data

        [HttpGet]
        [Route("company-apply")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> GetAllCompanyApplyAsync([FromQuery] FilterLocationData filter)
        {
            var result = await usersRepository.GetAllPaginatedCompanyApplyAsync(filter);
            return Ok(result);
        }

        [HttpGet]
        [Route("company-apply/{id:guid}")]
        [ActionName("GetCompanyApplyAsync")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> GetCompanyApplyAsync(Guid id)
        {
            var contacts = await usersRepository.GetCompanyApplyAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                var contactsDto = mapper.Map<CompanyApplyDataModelDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("company-apply/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> DeleteCompanyApplyAsync(Guid id)
        {
            // Get the region from the database
            var user = await usersRepository.DeleteCompanyApplyAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Advert Does not Exist");
            }

            else
            {
                var userDto = mapper.Map<CompanyApplyDataModelDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("company-apply/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdateCompanyApplyAsync([FromRoute] Guid id, [FromBody] AddCompanyApply request)
        {
            var userDto1 = await usersRepository.GetCompanyApplyAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("Id Does not Exist");
            }

            // convert back to dto
            else
            {
                var user = new CompanyApplyDataModel()
                {
                    Email = !string.IsNullOrWhiteSpace(request.Email) ? request.Email : userDto1.Email,
                    SurName = !string.IsNullOrWhiteSpace(request.SurName) ? request.SurName : userDto1.SurName,
                    FirstName = !string.IsNullOrWhiteSpace(request.FirstName) ? request.FirstName : userDto1.FirstName,
                    PhoneNo = !string.IsNullOrWhiteSpace(request.PhoneNo)?request.PhoneNo:userDto1.PhoneNo,
                    CompanyName = !string.IsNullOrWhiteSpace(request.CompanyName) ? request.CompanyName : userDto1.CompanyName,
                    CompanyUserName = !string.IsNullOrWhiteSpace(request.CompanyUserName) ? request.CompanyUserName : userDto1.CompanyUserName,
                    CompanyAddress = !string.IsNullOrWhiteSpace(request.CompanyAddress) ? request.CompanyAddress : userDto1.CompanyAddress,
                    CompanyType= request.CompanyType?? userDto1.CompanyType,
                };
                // Update detials to repository
                user = await usersRepository.UpdateCompanyApplyAsync(id, user);
                var contact = await usersRepository.GetCompanyApplyAsync(user.Id);
                var userDto = mapper.Map<CompanyApplyDataModelDto>(contact);
                return Ok(userDto);
            }
        }

        [HttpPost("company-apply")]
        public async Task<IActionResult> AddCompanyApplyAsync([FromBody] AddCompanyApply request)
        {
            var check = ValidateCompanyApply(request);


            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var contacts = new CompanyApplyDataModel()
                {
                    Email = request.Email,
                    SurName = request.SurName,
                    FirstName = request.FirstName,
                    PhoneNo = request.PhoneNo,
                    CompanyName = request.CompanyName,
                    CompanyUserName = request.CompanyUserName,
                    CompanyAddress = request.CompanyAddress,
                    CompanyType = request.CompanyType,
                    CreatedAt = DateTime.Now,

                };
                // Pass detials to repository
                contacts = await usersRepository.AddCompanyApplyAsync(contacts);

                // convert back to dto
                var contact = await usersRepository.GetCompanyApplyAsync(contacts.Id);
                var contactsDto = mapper.Map<CompanyApplyDataModelDto>(contact);
                return CreatedAtAction(nameof(GetCompanyApplyAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }

        #region private methods

        private static string Generate_otp()
        {
            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new();
            for (int i = 0; i < 4; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos)!.ToString()!)) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
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
        private async Task<bool> ValidateAddUserAsync(AddAdminRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError(nameof(request), $" Add User Data Is Required");
                return false;
            }

            var user = await generalUserRepository.UserExistAsync(request.Email!);

            if (user)
            {
                ModelState.AddModelError($"{nameof(request.Email)}", $"{nameof(request.Email)}  already Exist");
            }
            var userPhone = await generalUserRepository.UserPhoneNoExistAsync(request.PhoneNo!);

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

        private bool ValidateAdvert(AddAdvert request)
        {
            if (request == null)
            {
                ModelState.AddModelError(nameof(request), $"Add  Data Is Required");
                return false;
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;

        }
        private bool ValidateCompanyApply(AddCompanyApply request)
        {
            if (request == null)
            {
                ModelState.AddModelError(nameof(request), $"Add  Data Is Required");
                return false;
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;

        }

        private static string CreateRandomTokenSix()
        {
            char[] charArr = "ABCDEFGHIJKLMNOPQLSTUVWXYZ0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new();
            for (int i = 0; i < 6; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos)!.ToString()!)) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
        }
        private static string CreateRandomTokenFour()
        {
            char[] charArr = "ABCDEFGHIJKLMNOPQLSTUVWXYZ0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new();
            for (int i = 0; i < 4; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos)!.ToString()!)) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
        }
        private static string CreateRandomTokenThree()
        {
            char[] charArr = "ABCDEFGHIJKLMNOPQLSTUVWXYZ0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new();
            for (int i = 0; i < 3; i++)
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
