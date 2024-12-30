using AutoMapper;
using Genilog_WebApi.Key;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
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

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUsersController(IHostEnvironment _env, IGeneralUserRepository userRepository, ITokenHandler tokenHandler, IMapper mapper, IRolesRepository rolesRepository, IUser_RoleRepository user_RoleRepository, IUserRepository newUsersRepository,
        IUploadRepository uploadRepository) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IGeneralUserRepository userRepository = userRepository;
        private readonly ITokenHandler tokenHandler = tokenHandler;
        private readonly IMapper mapper = mapper;
        private readonly IRolesRepository rolesRepository = rolesRepository;
        private readonly IUser_RoleRepository user_RoleRepository = user_RoleRepository;
        private readonly IUserRepository newUsersRepository = newUsersRepository;
        private readonly IUploadRepository uploadRepository = uploadRepository;
        readonly string keyPath = Path.Combine(_env.ContentRootPath, "Key\\ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json");

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
        [Authorize(Roles = "User,Admin,Super_Admin")]
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
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();

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

                DocumentReference usrRef = firestoreDb!.Collection("UsersCollection").Document(users.Id.ToString());
                Dictionary<string, object> user3 = new()
                {
                    {"Id",users.Id.ToString() },
                    {"FirstName",users.FirstName!},
                    {"LastName",users.LastName!},
                    {"Email",users.Email! },
                    {"PhoneNo",users.PhoneNo!},
                    {"Sex",users.Sex!},
                    {"ProfilePicture",users.ProfilePicture! },
                    {"Address",users.Address!},
                    {"Locality",users.Locality! },
                    {"State",users.State! },
                    {"PostCodes",users.PostCodes! },
                    {"Latitude",users.Latitude! },
                    {"Longitude",users.Longitude! },
                    {"ReferralCode",users.ReferralCode! },
                    {"UserStatus",users.UserStatus! },
                    {"DatePublished",date!},
                    {"Timestamp",timeStamp!}

                };
                await usrRef.SetAsync(user3);
                EmailTemplates.SendEmailVerificationCode(users.Email!, generalUsers.VerificationToken!, users.FirstName!);
                // convert back to dto
                var user = await newUsersRepository.GetAsync(users.Id);
                var token = await userRepository.GetAllDeviceTokenAsync();
                var userDto = mapper.Map<UsersDataModelTableDto>(user);
                userDto.DeviceTokenModels = token.Where(x => x.UserId == user.Id).ToList();
                return Ok(userDto);
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "User,Super_Admin")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                DocumentReference usrRef = firestoreDb!.Collection("UsersCollection").Document(user.Id.ToString());
                await usrRef.DeleteAsync();
                var error = new ErrorModel()
                {
                    Message = "Deleted Successfully",
                    Status = true
                };
                return Ok(error);
            }
        }

        [HttpPut]
        [Route("update-device-token")]
        [Authorize]
        public async Task<IActionResult> UpdateDeviceTokenAsync([FromBody] AddDeviceToken request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                    DocumentReference usrRef = firestoreDb!.Collection("DeviceToken")
                        .Document(drviceToken.Id.ToString());
                    Dictionary<string, object> user3 = new()
                    {
                      { "Id", drviceToken.Id.ToString() },
                      { "DeviceTokenId", drviceToken.DeviceTokenId! },
                      { "UserId", drviceToken.UserId.ToString() },
                      { "UserType", drviceToken.UserType! },
                     };
                    await usrRef.SetAsync(user3);

                    return Ok(drviceToken);
                }

            }

        }

        [HttpPut]
        [Route("update")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserRequest request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                };

                // Update detials to repository
                user = await newUsersRepository.UpdateAsync(userDto1.Id, user);
                // check the null value

                // convert back to dto

                await userRepository.UpdatePhoneAsync(user.Id, user.PhoneNo!);
                var generalUsers = new GeneralUsers()
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                };
                await userRepository.UpdateAsync(userDto1.Id, generalUsers);
                DocumentReference usrRef = firestoreDb!.Collection("UsersCollection").Document(user.Id.ToString());
                Dictionary<string, object> user3 = new()
                       {
                        { "PhoneNo", user.PhoneNo! },
                        { "FirstName", user.FirstName! },
                        { "LastName", user.LastName! },
                        { "Sex", user.Sex! },
                         { "ProfilePicture", user.ProfilePicture! },
                       };
                await usrRef.UpdateAsync(user3);
                // convert back to dto
                var userDto12 = await newUsersRepository.GetAsync(user.Id);
                var token = await userRepository.GetAllDeviceTokenAsync();
                var userDto = mapper.Map<UsersDataModelTableDto>(userDto12);
                userDto.DeviceTokenModels = token.Where(x => x.UserId == user.Id).ToList();
                return Ok(userDto);

            }

        }


        [HttpPut("update-profile-image")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UploadFile( AddUploadFile path)
        {

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                var upload = new UploadFile()
                {
                    Image = path.Image,
                };
                var img = await uploadRepository.SavePostImageAsync(upload);

                var user2 = new GeneralUsers()
                {
                    ImagePath = img.ImagePath,
                };
                var user = new UsersDataModelTable()
                {
                    ProfilePicture = img.ImagePath,
                    Sex =  userDto1.Sex,
                    FirstName = userDto1.FirstName,
                    LastName = userDto1.LastName,
                    PhoneNo = userDto1.PhoneNo,
                };
                // Update detials to repository

                user = await newUsersRepository.UpdateAsync(userDto1.Id, user);
                 await userRepository.UploadPicsAsync(userDto1.Id, user2);
                DocumentReference usrRef = firestoreDb!.Collection("UsersCollection").Document(user2.Id.ToString());
                Dictionary<string, object> user3 = new()
                {
                 { "ProfilePicture", user.ProfilePicture! },
                };
                await usrRef.UpdateAsync(user3);
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
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);

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
                address = await newUsersRepository.AddDeliveryAddressAsync(address);
                DocumentReference usrRef2 = firestoreDb!.Collection("UsersCollection").Document(userDto1.Id.ToString()).
                    Collection("DeliveryAddress").Document(address.Id.ToString());
                var timeStamp = Timestamp.GetCurrentTimestamp();
                Dictionary<string, object> user4 = new()
                {
                    {"Id", address.Id.ToString()},
                    {"UserName",address.UserName! },
                    {"PhoneNo",address.PhoneNo!},
                    { "Address", address.Address! },
                    { "AddressPostCodes", address.AddressPostCodes! },
                    { "HouseNo", address.HouseNo! },
                    { "Locality", address.Locality! },
                    { "State", address.State! },
                    { "Latitude", address.Latitude! },
                    { "Longitude", address.Longitude! },
                    { "CreatedAt", timeStamp },
                };
                await usrRef2.SetAsync(user4);
                var userDto = new ResponseModel()
                {
                    Message = "Updated Successfully",
                    Status = true,
                };

                return Ok(userDto);
            }

        }

        [HttpPut]
        [Route("update-delivery-address/{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateDeliveryAddressAsync([FromRoute] Guid id, [FromBody] AddDeliveryAddress request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
            user = await newUsersRepository.UpdateDeliveryAddressAsync(id, user);

            // check the null value
            DocumentReference usrRef = firestoreDb!.Collection("UsersCollection").Document(user.UsersDataModelTableId.ToString()).
                      Collection("DeliveryAddress").Document(user.Id.ToString());
            Dictionary<string, object> user3 = new()
                {
                     {"UserName",user.UserName! },
                    {"PhoneNo",user.PhoneNo!},
                    { "Address", user.Address! },
                    { "AddressPostCodes", user.AddressPostCodes! },
                    { "HouseNo", user.HouseNo! },
                    { "Locality", user.Locality! },
                    { "Latitude", user.Latitude! },
                    { "Longitude", user.Longitude! },
                };

            await usrRef.UpdateAsync(user3);
            var userDto = new ResponseModel()
            {
                Message = "Updated Successfully",
                Status = true,
            };

            return Ok(userDto);

        }

        [HttpDelete]
        [Route("delete-delivery-address/{id:guid}")]
        [Authorize(Roles = "User,Super_Admin")]
        public async Task<IActionResult> DeleteDeliveryAddressAsync([FromRoute] Guid id)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                DocumentReference usrRef = firestoreDb!.Collection("UsersCollection").Document(user.UsersDataModelTableId.ToString()).
                    Collection("DeliveryAddress").Document(user.Id.ToString());
                await usrRef.DeleteAsync();
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
