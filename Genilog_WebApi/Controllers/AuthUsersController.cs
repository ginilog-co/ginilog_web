using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Google.Cloud.Firestore;
using Genilog_WebApi.Model.UploadModels;
using Genilog_WebApi.Repository.UploadRepo;
using Genilog_WebApi.Model.UsersDataModel;
using Genilog_WebApi.Model;
using Genilog_WebApi.Repository.AuthRepo;
using Genilog_WebApi.Model.AuthModel;
using Genilog_WebApi.EmailSender;
using Genilog_WebApi.Repository.UserRepo;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Google.Apis.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Genilog_WebApi.Repository.NotificationRepo;

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUsersController(IHostEnvironment _env, IGeneralUserRepository userRepository, ITokenHandler tokenHandler, IMapper mapper, IRolesRepository rolesRepository, IUser_RoleRepository user_RoleRepository, IUserRepository newUsersRepository,
        IUploadRepository uploadRepository, IHubContext<UserHubRepository> _hubContext, IHubContext<NotificationHub> _notificationHubContext) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IGeneralUserRepository userRepository = userRepository;
        private readonly ITokenHandler tokenHandler = tokenHandler;
        private readonly IMapper mapper = mapper;
        private readonly IRolesRepository rolesRepository = rolesRepository;
        private readonly IUser_RoleRepository user_RoleRepository = user_RoleRepository;
        private readonly IUserRepository newUsersRepository = newUsersRepository;
        private readonly IUploadRepository uploadRepository = uploadRepository;
        private readonly IHubContext<UserHubRepository> _hubContext = _hubContext;
        private readonly IHubContext<NotificationHub> _notificationHubContext = _notificationHubContext;
        // readonly string keyPath = Path.Combine(_env.ContentRootPath, "Key\\ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json");

        private const string googleClientId = "650523296976-86526ebsjn0266ogajsl6fllfl08jn5h.apps.googleusercontent.com";
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
                var userD = await newUsersRepository.GetAsync(user.Id);
                if (user.EmailConfirmed == false)
                {
                    var user2 = await userRepository.RequestNewEmailTokenAsync(userD.Email!);
                    EmailTemplates.SendEmailVerificationCode(user.Email!, user2.VerificationToken!, user.LastName!);
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
                        Message = "Not A User Account",
                        Status = true
                    };
                    return BadRequest(error);
                }
                else
                {
                    //generate jwt token
                    var token = tokenHandler.CreateTokenAsync(user);
                    var refreshToken = tokenHandler.RefreshTokenAsync(user.Email!);
                    // var userId = userRepository.Userd;
                    //  
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
                        IdAuthPassword = ""
                    };
                   var request  =await newUsersRepository.GetAsync(user.Id);
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
                    };
                    await newUsersRepository.UpdateAsync(user.Id,users);
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

        [HttpPost("apple")]
        public async Task<IActionResult> AppleAuth([FromBody] LoginExternalRequset requvest)
        {
            try
            {
                // Decode the token
                var handler = new JwtSecurityTokenHandler();
                var tokenD = handler.ReadJwtToken(requvest.IdToken);

                // Fetch Apple's public keys
                var applePublicKeysUrl = "https://appleid.apple.com/auth/keys";
                using var client = new HttpClient();
                var keysResponse = client.GetStringAsync(applePublicKeysUrl).Result;
                var keys = JsonDocument.Parse(keysResponse).RootElement.GetProperty("keys");

                // Extract claims from the token
                var userId = tokenD.Claims.FirstOrDefault(c => c.Type == "sub")?.Value; // Unique user ID
                var email = tokenD.Claims.FirstOrDefault(c => c.Type == "email")?.Value; // User email
                var emailVerified = tokenD.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value == "true"; // Email verification status
                var givenName = tokenD.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value; // User's given name
                var familyName = tokenD.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value; // User's family name

                // Apple tokens do not include profile pictures directly

                var userExist = await userRepository.UserExistAsync(email!);
                if (userExist)
                {
                    var user = await userRepository.AuthenticateAsync(email!, userId!);
                    var userD = await newUsersRepository.GetAsync(user.Id);
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
                        IdAuthPassword = userId!
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
                    };
                    await newUsersRepository.UpdateAsync(user.Id, users);
                    return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.UserId }, userDto);
                }
                else
                {
                    var generalUsers = new GeneralUsers()
                    {
                        FirstName = givenName,
                        LastName = familyName,
                        Email = email,
                        UserType = "User",
                        VerificationToken = CreateRandomToken(),
                        EmailConfirmed = true,
                        PhoneNo = "",
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
                    };
                    generalUsers = await userRepository.AddAsync(generalUsers, userId!);
                    var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
                    var timeStamp = Timestamp.GetCurrentTimestamp();

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
                        CreatedAt = DateTime.UtcNow,
                        LastLoginAt = DateTime.UtcNow,
                        LastSeenAt = DateTime.UtcNow,
                        Address = "",
                        Locality = "",
                        State = "",
                        PostCodes = "",
                        Latitude = 1.11,
                        Longitude = 1.11,
                    };
                    // Pass detials to repository
                    users = await newUsersRepository.AddAsync(users);
                    var roles = new Roles()
                    {
                        Name = "User"
                    };
                    roles = await rolesRepository.AddAsync(roles);
                    var user_Roles = new User_Role()
                    {
                        GeneralUsersId = users.Id,
                        RoleId = roles.Id,
                    };
                    await user_RoleRepository.AddAsync(user_Roles);

                    //Now Login Here
                    var loginUser = await userRepository.AuthenticateAsync(generalUsers.Email!, userId!);
                    var userD = await newUsersRepository.GetAsync(loginUser.Id);
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
                        IdAuthPassword = userId!
                    };
                    await _notificationHubContext.Clients.Group(loginUser.Id.ToString()).SendAsync("NOTIFICATION", userDto);
                    return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.UserId }, userDto);
                }
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Error = "Invalid Apple token", Details = ex.Message });
            }
        }



        [HttpPost("google")]
        public async Task<IActionResult> GoogleAuth([FromBody] LoginExternalRequset requvest)
        {
            try
            {
                // Verify Google ID token
                var payload = await GoogleJsonWebSignature.ValidateAsync(requvest.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [googleClientId]
                });

                // Token is valid, create or fetch the user in your database
                var userExist = await userRepository.UserExistAsync(payload.Email!);
                if (userExist)
                {
                    var user = await userRepository.AuthenticateAsync(payload.Email!, payload.Subject!);
                    var userD = await newUsersRepository.GetAsync(user.Id);
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
                        IdAuthPassword = payload.Subject
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
                    };
                    await newUsersRepository.UpdateAsync(user.Id, users);
                    return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.UserId }, userDto);
                }
                else
                {
                    var generalUsers = new GeneralUsers()
                    {
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Email = payload.Email,
                        UserType = "User",
                        VerificationToken = CreateRandomToken(),
                        EmailConfirmed = true,
                        PhoneNo = "",
                        ImagePath = payload.Picture,
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
                    };
                    generalUsers = await userRepository.AddAsync(generalUsers, payload.Subject);
                    var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
                    var timeStamp = Timestamp.GetCurrentTimestamp();

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
                        CreatedAt = DateTime.UtcNow,
                        LastLoginAt = DateTime.UtcNow,
                        LastSeenAt = DateTime.UtcNow,
                        Address = "",
                        Locality = "",
                        State = "",
                        PostCodes = "",
                        Latitude = 1.11,
                        Longitude = 1.11,
                    };
                    // Pass detials to repository
                    users = await newUsersRepository.AddAsync(users);
                    var roles = new Roles()
                    {
                        Name = "User"
                    };
                    roles = await rolesRepository.AddAsync(roles);
                    var user_Roles = new User_Role()
                    {
                        GeneralUsersId = users.Id,
                        RoleId = roles.Id,
                    };
                    await user_RoleRepository.AddAsync(user_Roles);

                    //Now Login Here
                    var loginUser = await userRepository.AuthenticateAsync(generalUsers.Email!, payload.Subject);
                    var userD = await newUsersRepository.GetAsync(loginUser.Id);
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
                        IdAuthPassword = payload.Subject
                    };
                    await _notificationHubContext.Clients.Group(loginUser.Id.ToString()).SendAsync("NOTIFICATION", userDto);
                    return CreatedAtAction(nameof(ProfileAsync), new { id = userDto.UserId }, userDto);
                }
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Error = "Invalid Google token", Details = ex.Message });
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsersAsync([FromQuery] FilterLocationData data)
        {
            List<UsersDataModelTableDto> users = [];
            var token = await userRepository.GetAllDeviceTokenAsync();
            var user = await newUsersRepository.GetAllAsync();
            var allPosts = user.AsQueryable();

            if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality) && !string.IsNullOrEmpty(data.AnyItem))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality) && (x.FirstName!.Contains(data.AnyItem) ||
                x.LastName!.Contains(data.AnyItem) || x.Email!.Contains(data.AnyItem) || x.PhoneNo!.Contains(data.AnyItem)));
                var userDto = mapper.Map<List<UsersDataModelTableDto>>(allPosts);
                foreach (var item in userDto)
                {
                    item.DeviceTokenModels = token.Where(x => x.UserId == item.Id).ToList();
                    users.Add(item);
                }
                return Ok(users);
            }
            else if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<UsersDataModelTableDto>>(allPosts);
                foreach (var item in userDto)
                {
                    item.DeviceTokenModels = token.Where(x => x.UserId == item.Id).ToList();
                    users.Add(item);
                }
                return Ok(users);
            }

            else if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.AnyItem))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && (x.FirstName!.Contains(data.AnyItem) ||
                x.LastName!.Contains(data.AnyItem) || x.Email!.Contains(data.AnyItem) || x.PhoneNo!.Contains(data.AnyItem)));
                var userDto = mapper.Map<List<UsersDataModelTableDto>>(allPosts);
                foreach (var item in userDto)
                {
                    item.DeviceTokenModels = token.Where(x => x.UserId == item.Id).ToList();
                    users.Add(item);
                }
                return Ok(users);
            }
            else if (!string.IsNullOrEmpty(data.State))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State));
                var userDto = mapper.Map<List<UsersDataModelTableDto>>(allPosts);
                foreach (var item in userDto)
                {
                    item.DeviceTokenModels = token.Where(x => x.UserId == item.Id).ToList();
                    users.Add(item);
                }
                return Ok(users);
            }
            else if (!string.IsNullOrEmpty(data.Locality) && !string.IsNullOrEmpty(data.AnyItem))
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality) && (x.FirstName!.Contains(data.AnyItem) ||
                x.LastName!.Contains(data.AnyItem) || x.Email!.Contains(data.AnyItem) || x.PhoneNo!.Contains(data.AnyItem)));
                var userDto = mapper.Map<List<UsersDataModelTableDto>>(allPosts);
                foreach (var item in userDto)
                {
                    item.DeviceTokenModels = token.Where(x => x.UserId == item.Id).ToList();
                    users.Add(item);
                }
                return Ok(users);
            }
            else if (!string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<UsersDataModelTableDto>>(allPosts);
                foreach (var item in userDto)
                {
                    item.DeviceTokenModels = token.Where(x => x.UserId == item.Id).ToList();
                    users.Add(item);
                }
                return Ok(users);
            }
            else if (!string.IsNullOrEmpty(data.AnyItem))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.AnyItem) || x.Locality!.Contains(data.AnyItem) || x.FirstName!.Contains(data.AnyItem) ||
                x.LastName!.Contains(data.AnyItem) || x.Email!.Contains(data.AnyItem) || x.PhoneNo!.Contains(data.AnyItem));
                var userDto = mapper.Map<List<UsersDataModelTableDto>>(allPosts);
                foreach (var item in userDto)
                {
                    item.DeviceTokenModels = token.Where(x => x.UserId == item.Id).ToList();
                    users.Add(item);
                }
                return Ok(users);
            }
            else
            {
                var userDto = mapper.Map<List<UsersDataModelTableDto>>(user);
                foreach (var item in userDto)
                {
                    item.DeviceTokenModels = token.Where(x => x.UserId == item.Id).ToList();
                    users.Add(item);
                }
                return Ok(users);
            }

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
        [Route("for-admin")]
        [ActionName("ProfileAsync")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> ProfileAsync([FromHeader]Guid id)
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
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    UserType = "User",
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
                };
                // Pass detials to repository
                users = await newUsersRepository.AddAsync(users);
                var roles = new Roles()
                {
                    Name = "User"
                };
                roles = await rolesRepository.AddAsync(roles);
                var user_Roles = new User_Role()
                {
                    GeneralUsersId = users.Id,
                    RoleId = roles.Id,
                };
                await user_RoleRepository.AddAsync(user_Roles);
                EmailTemplates.SendEmailVerificationCode(users.Email!, generalUsers.VerificationToken!, users.FirstName!);
                // convert back to dto
                var user = await newUsersRepository.GetAsync(users.Id);
                var userDto = mapper.Map<UsersDataModelTableDto>(user);
                await _hubContext.Clients.All.SendAsync("GetUser", userDto);
                await _notificationHubContext.Clients.Group(users.Id.ToString()).SendAsync("NOTIFICATION", userDto);
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
                await _hubContext.Clients.All.SendAsync("GetUser", userDto);
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
            var token = await newUsersRepository.GetAllDeliveryAsync();
            token = token.Where(x => x.UsersDataModelTableId == userGuid).ToList();
            var userDto = mapper.Map<List<DeliveryAddressDto>>(token);
            return Ok(userDto);
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Super_Admin")]
        [Route("delivery-address-for-admin")]
        public async Task<IActionResult> GetAllUsersDeliveryAdressAsync([FromHeader] Guid userId)
        {
            var token = await newUsersRepository.GetAllDeliveryAsync();
            token = token.Where(x => x.UsersDataModelTableId == userId).ToList();
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
                await _hubContext.Clients.All.SendAsync("GetUser", userDto);
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
            await _hubContext.Clients.All.SendAsync("GetUser", userDto);
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
                EmailTemplates.SendChangePasswordCodeEmail(email.Email!, user.PasswordResetToken!, user.FirstName!);
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
                EmailTemplates.SendEmailVerificationCode(email.Email!, user.VerificationToken!, user.LastName!);
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
