using AutoMapper;
using FirebaseAdmin.Messaging;
using Genilog_WebApi.EmailSender;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.AuthModel;
using Genilog_WebApi.Model.LogisticsModel;
using Genilog_WebApi.Model.Notification_Model;
using Genilog_WebApi.Repository.AuthRepo;
using Genilog_WebApi.Repository.LogisticsRepo;
using Genilog_WebApi.Repository.NotificationRepo;
using Genilog_WebApi.Repository.UploadRepo;
using Genilog_WebApi.Repository.UserRepo;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticsController(IHostEnvironment _env, IMapper mapper, ITokenHandler tokenHandler
        , IGeneralUserRepository generalUserRepository, IUploadRepository uploadRepository, IRolesRepository rolesRepository, IUser_RoleRepository user_RoleRepository, 
        IRidersRepository ridersRepository, ICompanyRepository companyRepository
         , INotificationRepository notificationRepository, IHubContext<LogisticsHubRepository> _hubContext, IHubContext<NotificationHub> _notificationHubContext,
        IUserRepository newUsersRepository) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IMapper mapper = mapper;
        private readonly ITokenHandler tokenHandler = tokenHandler;
        private readonly IGeneralUserRepository generalUserRepository = generalUserRepository;
        private readonly IRolesRepository rolesRepository = rolesRepository;
        private readonly IUser_RoleRepository user_RoleRepository = user_RoleRepository;
        private readonly IUploadRepository uploadRepository = uploadRepository;
        private readonly ICompanyRepository companyRepository = companyRepository;
        private readonly IRidersRepository ridersRepository = ridersRepository;
        readonly INotificationRepository notificationRepository = notificationRepository;
        private readonly IHubContext<LogisticsHubRepository> _hubContext = _hubContext;
        private readonly IHubContext<NotificationHub> _notificationHubContext = _notificationHubContext;
        private readonly IUserRepository newUsersRepository = newUsersRepository;
        //  readonly string keyPath = Path.Combine(_env.ContentRootPath, "Key\\ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json");


        // This Is Rider SECTION
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginRequset requset)
        {

            var user = await generalUserRepository.AuthenticateAsync(requset.Email_PhoneNo!, requset.Password!);

            if (user != null)
            {

                if (user.UserType == "Manager" || user.UserType == "Rider")
                {

                    if (user.EmailConfirmed == false)
                    {
                        var user2 = await generalUserRepository.RequestNewEmailTokenAsync(user.Email!);
                        EmailTemplates.SendEmailVerificationCode(user.Email!, user2.VerificationToken!, user.LastName!);
                        //  string message1 = user.FirstName + " Your BMG(Bring My Gas) App Account Verication Code is " + user2.VerificationToken!;
                        var error = new ErrorModel()
                        {
                            Message = "User Email Not Yet Verify",
                            Status = true
                        };
                        return BadRequest(error);
                    }
                    else
                    {
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
                            FullName = $"{user.FirstName} {user.LastName}",
                            ProfileImage = user.ImagePath,

                        };
                        return Ok(userDto);
                    }
                }
                else
                {
                    var error = new ErrorModel()
                    {
                        Message = "Not A Manager Or Rider Account",
                        Status = true
                    };
                    return BadRequest(error);

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

        [HttpPost("email-verification")]
        public async Task<IActionResult> Verify(EmailVerification verification)
        {

            var user = await generalUserRepository.VerifyAsync(verification.Token!);
            if (user == null)
            {
                return BadRequest("Invalid token.");
            }
            else
            {
                user = await generalUserRepository.AuthenticateAsync(user.Email!, verification.Password!);
                var userD = await generalUserRepository.GetAsync(user.Id);
                if (user != null)
                {
                    if (user.UserType != "Manager")
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
                        return Ok(userDto);
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
            var user = await generalUserRepository.ForgetPasswordAsync(email.Email!);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            else if (user.UserType == "Manager" || user.UserType == "Rider")
            {
                SendChangePasswordCodeEmail(email.Email!, user.PasswordResetToken!, user.FirstName!);
                return Ok($"Password Reset token has been Sent to your Email");
            }
            else
            {
                return BadRequest("User not found.");
               
            }
        }

        [HttpPost("email-verification-request-token")]
        public async Task<IActionResult> EmailAvailableRequestToken(ForgetPasswordRequest email)
        {
            var user = await generalUserRepository.RequestNewEmailTokenAsync(email.Email!);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
           
            else if (user.UserType == "Manager" || user.UserType == "Rider")
            {
                SendEmailAvailableCode(email.Email!, user.VerificationToken!, user.LastName!);
                return Ok($"New token has been Sent to your Email");
            }
            else
            {
               return BadRequest("User not found.");
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

        [HttpPost("phone-no-verification")]
        public async Task<IActionResult> PhoneNoVerify(PhoneVerification otp)
        {
            var user = await generalUserRepository.PhoneNoVerifyAsync(otp.Otp!);
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
            var user = await generalUserRepository.TwoFactorEnabledAsync(id);
            if (user == null)
            {
                return BadRequest("User Does Not Exist");
            }
            else
            {
                return Ok($"User Two Factor Authentication Enabled");
            }
        }



        // This Is Gas Station SECTION
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCompanyAsync([FromQuery] FilterLocationData data)
        {
            var events = await companyRepository.GetAllAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var allPosts = events.AsQueryable();

            if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<CompanyModelDataDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.State))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State));
                var userDto = mapper.Map<List<CompanyModelDataDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<CompanyModelDataDto>>(allPosts);

                return Ok(userDto);
            }
            else
            {
                var userDto = mapper.Map<List<CompanyModelDataDto>>(events);
                return Ok(userDto);
            }


        }

        [HttpGet]
        [Route("{id:guid}")]
        [Authorize]
        [ActionName("GetCompanyAsync")]
        public async Task<IActionResult> GetCompanyAsync(Guid id)
        {
            var contacts = await companyRepository.GetAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                var contactsDto = mapper.Map<CompanyModelDataDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Super_Admin")]
        public async Task<IActionResult> DeleteCompanyAsync(Guid id)
        {
            // Get the region from the database
            var user = await companyRepository.DeleteAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("company Does not Exist");
            }

            else
            {
                await generalUserRepository.DeleteAsync(user.Id);
                var userDto = mapper.Map<CompanyModelDataDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "Station_User,Super_Admin")]
        public async Task<IActionResult> UpdateCompanyAsync([FromRoute] Guid id, [FromBody] UpdateCompany request)
        {
            var userDto1 = await companyRepository.GetAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("User Does not Exist");
            }

            // convert back to dto
            else
            {
                var gen = await generalUserRepository.GetAsync(userDto1.Id);
                var user = new CompanyModelData()
                {
                    CompanyName = !string.IsNullOrWhiteSpace(request.CompanyName) ? request.CompanyName : userDto1.CompanyName,
                    CompanyAddress = !string.IsNullOrWhiteSpace(request.CompanyAddress) ? request.CompanyAddress : userDto1.CompanyAddress,
                    CompanyRegNo = !string.IsNullOrWhiteSpace(request.CompanyRegNo) ? request.CompanyRegNo : userDto1.CompanyRegNo,
                    CompanyInfo = !string.IsNullOrWhiteSpace(request.CompanyInfo) ? request.CompanyInfo : userDto1.CompanyInfo,
                    PhoneNumber = !string.IsNullOrWhiteSpace(request.PhoneNumber) ? request.PhoneNumber : userDto1.PhoneNumber,
                    CompanyLogo = !string.IsNullOrWhiteSpace(request.CompanyLogo) ? request.CompanyLogo : userDto1.CompanyLogo,
                    Locality = !string.IsNullOrWhiteSpace(request.Locality) ? request.Locality : userDto1.Locality,
                    PostCodes = !string.IsNullOrWhiteSpace(request.PostCodes) ? request.PostCodes : userDto1.PostCodes,
                    Latitude = (double)(request.Latitude ?? userDto1.Latitude),
                    Longitude = (double)(request.Longitude ?? userDto1.Longitude),
                    Rating = (int)(request.Rating ?? userDto1.Rating),
                    ValueCharge = (double)(request.ValueCharge ?? userDto1.ValueCharge),
                    AccountName = !string.IsNullOrWhiteSpace(request.AccountName) ? request.AccountName : userDto1.AccountName,
                    AccountNumber = !string.IsNullOrWhiteSpace(request.AccountNumber) ? request.AccountNumber : userDto1.AccountNumber,
                    BankName = !string.IsNullOrWhiteSpace(request.BankName) ? request.BankName : userDto1.BankName,
                    State = !string.IsNullOrWhiteSpace(request.State) ? request.State : userDto1.State,
                    NofOfBikes = (int)(request.NofOfBikes ?? userDto1.NofOfBikes),
                    Available = (bool)(request.Available ?? userDto1.Available),
                    NoOfTrucks = (int)(request.NoOfTrucks ?? userDto1.NoOfTrucks),
                };
                // Update detials to repository
                user = await companyRepository.UpdateAsync(id, user);
                gen = new GeneralUsers()
                {
                    LastName = !string.IsNullOrWhiteSpace(request.LastName) ? $"{request.LastName}" : gen.LastName,
                    FirstName = !string.IsNullOrWhiteSpace(request.FirstName) ? $"{request.FirstName}" : gen.FirstName,
                    PhoneNo = user.PhoneNumber,
                    ImagePath = user.CompanyLogo,
                };
                await generalUserRepository.UpdateAsync(user.Id, gen);
                var contact = await companyRepository.GetAsync(user.Id);
                var userDto = mapper.Map<CompanyModelDataDto>(contact);
                await _hubContext.Clients.All.SendAsync("GetCompany", userDto);
                return Ok(userDto);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> AddCompanyAsync(AddCompany request)
        {
            var check = ValidateCompany(request);
            var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
            var timeStamp = Timestamp.GetCurrentTimestamp();
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
                    Email = request.CompanyEmail,
                    UserType = "Manager",
                    VerificationToken = CreateRandomToken(),
                    EmailConfirmed = false,
                    PhoneNo = request.PhoneNumber,
                    ImagePath = request.CompanyLogo,
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
                generalUsers = await generalUserRepository.AddAsync(generalUsers, request.Password!);
                var contacts = new CompanyModelData()
                {
                    Id = generalUsers.Id,
                    CompanyName = request.CompanyName,
                    CompanyInfo = request.CompanyInfo,
                    CompanyEmail = request.CompanyEmail,
                    PhoneNumber = request.PhoneNumber,
                    CompanyLogo = request.CompanyLogo,
                    CompanyAddress = request.CompanyAddress,
                    CompanyRegNo = request.CompanyRegNo,
                    Rating = 0,
                    Available= false,
                    NofOfBikes = request.NofOfBikes,
                    NoOfTrucks = request.NoOfTrucks ,
                    ValueCharge = 0,
                    BankName = request.BankName,
                    AccountNumber = request.AccountNumber,
                    AccountName = request.AccountName,
                    Locality = request.Locality,
                    State = request.State,
                    PostCodes = request.PostCodes,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    CreatedAt = DateTime.UtcNow,
                };
                // Pass detials to repository
                contacts = await companyRepository.AddAsync(contacts);
                var roles = new Roles()
                {
                    Name = "Manager"
                };
                roles = await rolesRepository.AddAsync(roles);
                var user_Roles = new User_Role()
                {
                    GeneralUsersId = contacts.Id,
                    RoleId = roles.Id,
                };
                await user_RoleRepository.AddAsync(user_Roles);

                EmailTemplates.SendEmailVerificationCode(generalUsers.Email!, generalUsers.VerificationToken!, generalUsers.LastName!);
                // convert back to dto
                var contactsDto = new CompanyModelDataDto()
                {
                    Id = contacts.Id,
                    CompanyName = contacts.CompanyName,
                    CompanyInfo = contacts.CompanyInfo,
                    CompanyEmail = contacts.CompanyEmail,
                    PhoneNumber = contacts.PhoneNumber,
                    CompanyAddress = contacts.CompanyAddress,
                    Rating = contacts.Rating,
                    NofOfBikes = contacts.NofOfBikes,
                    NoOfTrucks  = contacts.NoOfTrucks ,
                    ValueCharge = contacts.ValueCharge,
                    CompanyLogo = contacts.CompanyLogo,
                    CompanyRegNo = contacts.CompanyRegNo,
                    AccountName = contacts.AccountName,
                    AccountNumber = contacts.AccountNumber,
                    BankName = contacts.BankName,
                    State = contacts.State,
                    Latitude = contacts.Latitude,
                    Longitude = contacts.Longitude,
                    Locality = contacts.Locality,
                    PostCodes = contacts.PostCodes,
                    Available = contacts.Available,
                    CreatedAt = contacts.CreatedAt,
                };
                await _hubContext.Clients.All.SendAsync("GetCompany", contactsDto);
                await _notificationHubContext.Clients.Group(contacts.Id.ToString()).SendAsync("NOTIFICATION", contactsDto);
                return CreatedAtAction(nameof(GetCompanyAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }


        [HttpGet("rider-type")]
        [Authorize]
        public IActionResult GetAllRiderTypeAsync()
        {
            List<string> contacts =[
                "Car Rider",
                "Bike Rider",
                "Flight",
                "Individual"
            ];
            return Ok(contacts);
        }


        [HttpGet("rider")]
        [Authorize]
        public async Task<IActionResult> GetAllRidersAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var user = await generalUserRepository.GetAsync(userGuid);
            if (user == null)
            {
                return BadRequest("User Does not Exist");
            }
            else if (user.UserType == "Manager")
            {
                var contacts = await ridersRepository.GetAllAsync();
                var contactsDto = mapper.Map<List<RidersModelDataDto>>(contacts);
                contactsDto = contactsDto.Where(x => x.CompanyId == user.Id).ToList();
                return Ok(contactsDto);
            }
            else
            {
                var contacts = await ridersRepository.GetAllAsync();
                var contactsDto = mapper.Map<List<RidersModelDataDto>>(contacts);
                return Ok(contactsDto);
            }
          
        }

        [HttpGet]
        [Route("rider/{id:guid}")]
        [ActionName("GetRidersAsync")]
        [Authorize]
        public async Task<IActionResult> GetRidersAsync(Guid id)
        {
            var contacts = await ridersRepository.GetAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                var contactsDto = mapper.Map<RidersModelDataDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("rider/{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> DeleteRidersAsync(Guid id)
        {
            // Get the region from the database
            var user = await ridersRepository.DeleteAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Hotel Does not Exist");
            }

            else
            {
                var userDto = mapper.Map<RidersModelDataDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("rider/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateRidersAsync([FromRoute] Guid id, [FromBody] UpdateRiders request)
        {
            
            
            var userDto1 = await ridersRepository.GetAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("Id Does not Exist");
            }

            // convert back to dto
            else
            {
                var  gen = await generalUserRepository.GetAsync(userDto1.Id);
                var user = new RidersModelData()
                {
                    CompanyName = !string.IsNullOrWhiteSpace(request.CompanyName) ? request.CompanyName : userDto1.CompanyName,
                    PhoneNumber = !string.IsNullOrWhiteSpace(request.PhoneNumber) ? request.PhoneNumber : userDto1.PhoneNumber,
                    Address = !string.IsNullOrWhiteSpace(request.Address) ? request.Address : userDto1.Address,
                    Locality = !string.IsNullOrWhiteSpace(request.Locality) ? request.Locality : userDto1.Locality,
                    PostCodes = !string.IsNullOrWhiteSpace(request.Postcode) ? request.Postcode : userDto1.PostCodes,
                    Latitude = (double)(request.Latitude ?? userDto1.Latitude),
                    Longitude = (double)(request.Longitude ?? userDto1.Longitude),
                    Rating = userDto1.Rating,
                    AccountName = !string.IsNullOrWhiteSpace(request. AccountName) ? request. AccountName : userDto1. AccountName,
                    AccountNumber = !string.IsNullOrWhiteSpace(request.AccountNumber) ? request.AccountNumber : userDto1.AccountNumber,
                    BankName = !string.IsNullOrWhiteSpace(request.BankName) ? request.BankName : userDto1.BankName,
                    Name = !string.IsNullOrWhiteSpace(request.FirstName)|| !string.IsNullOrWhiteSpace(request.LastName) ? $"{request.FirstName} {request.LastName}" : userDto1.Name,
                    State = !string.IsNullOrWhiteSpace(request.State) ? request.State : userDto1.State,
                    ProfilePicture = !string.IsNullOrWhiteSpace(request.ProfilePicture) ? request.ProfilePicture : userDto1.ProfilePicture,
                    Available = (bool)(request.Available ?? userDto1.Available),
                    IsVerified = (bool)(request.IsVerified ?? userDto1.IsVerified),
                };
                // Update detials to repository
                user = await ridersRepository.UpdateAsync(id, user);
                 gen = new GeneralUsers() 
                     {
                    LastName =  !string.IsNullOrWhiteSpace(request.LastName) ? $"{request.LastName}" : gen.LastName,
                    FirstName = !string.IsNullOrWhiteSpace(request.FirstName) ? $"{request.FirstName}" : gen.FirstName,
                    PhoneNo=user.PhoneNumber,
                   ImagePath = user.ProfilePicture,
                 };
                await generalUserRepository.UpdateAsync(user.Id, gen);
                var contact = await ridersRepository.GetAsync(user.Id);
                var userDto = mapper.Map<RidersModelDataDto>(contact);
                await _hubContext.Clients.All.SendAsync("GetRider", userDto);
                return Ok(userDto);
            }
        }

        [HttpPost("rider")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> AddRidersAsync([FromHeader] Guid companyId, [FromBody] AddRiders request)
        {
            var check = ValidateRiders(request);
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
                    UserType = "Rider",
                    VerificationToken = CreateRandomToken(),
                    EmailConfirmed = true,
                    PhoneNo = request.PhoneNumber,
                    ImagePath = request.ProfilePictureUrl,
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
                };
                generalUsers = await generalUserRepository.AddAsync(generalUsers, request.Password!);
                var company = await companyRepository.GetAsync(companyId);

                var contacts = new RidersModelData()
                {
                    Id = generalUsers.Id,
                    CompanyId = company.Id,
                    CompanyName = company.CompanyName,
                    Email = request.Email,
                    AccountNumber = request.AccountNumber,
                    BankName = request.BankName,
                    Name = $"{request.FirstName} {request.LastName}",
                    PhoneNumber = request.PhoneNumber,
                    Address= request.Address,
                    State = request.State,
                    Locality = request.Locality,
                    PostCodes = request.Postcode,
                    Latitude = (double)request.Latitude!,
                    Longitude = (double)request.Longitude!,
                    Rating = 0,
                    AccountName = request.AccountName,
                    Available = true,
                    ProfilePicture = request.ProfilePictureUrl,
                    IsVerified=false,
                    CreatedAt = DateTime.Now,

                };
                // Pass detials to repository
                contacts = await ridersRepository.AddAsync(contacts);
                var roles = new Roles()
                {
                    Name = "Rider"
                };
                roles = await rolesRepository.AddAsync(roles);
                var user_Roles = new User_Role()
                {
                    GeneralUsersId = contacts.Id,
                    RoleId = roles.Id,
                };
                await user_RoleRepository.AddAsync(user_Roles);
                // convert back to dto
                var contact = await ridersRepository.GetAsync(contacts.Id);
                var contactsDto = mapper.Map<RidersModelDataDto>(contact);
                await _hubContext.Clients.All.SendAsync("GetRider", contactsDto);
                return CreatedAtAction(nameof(GetRidersAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }

      
        [HttpPut]
        [Route("update-riders-review/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateRidersReviewAsync([FromRoute] Guid id, [FromBody] AddRidersReview request)
        {
            var events = await ridersRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Riders Does not Exist");
            }
            // convert back to dto
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userId, out Guid userGuid))
                {
                    return BadRequest("Invalid User ID format.");
                }
                var user = await generalUserRepository.GetAsync(userGuid);
                var interested = new RidersReviewModel()
                {
                    UserName = $"{user.FirstName} {user.LastName}",
                    ProfileImage = user.ImagePath,
                    RidersModelDataId = events.Id,
                    UserId = user.Id,
                    ReviewMessage = request.ReviewMessage,
                    CreatedAt = DateTime.Now,
                };

              await ridersRepository.AddRidersReviewAsync(interested);
      
                var userDto = new ResponseModel()
                {
                    Message = "Update Facilities",
                    Status = true,
                };
                return Ok(userDto);
            }
        }

        // Orders
        [HttpGet("package-orders")]
        [Authorize]
        public async Task<IActionResult> GetAllPackageOrderAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var user = await generalUserRepository.GetAsync(userGuid);

            if (user.UserType=="Super_Admin"|| user.UserType=="Admin") 
            {
                var events = await companyRepository.GetAllOrderAsync();
                events = [.. events.OrderByDescending(x => x.CreatedAt)];
                var userDto = mapper.Map<List<OrderModelDataDto>>(events);
                return Ok(userDto);
            }
           else
            {
                var events = await companyRepository.GetAllOrderAsync();
                events = [.. events.OrderByDescending(x => x.CreatedAt)];
                events = events.Where(x => x.UserId == user.Id || x.CompanyId == user.Id || x.RiderId == user.Id).ToList();
                var userDto = mapper.Map<List<OrderModelDataDto>>(events);
                return Ok(userDto);
            }
           
        }

        [HttpGet]
        [Route("package-orders/{id:guid}")]
        [ActionName("GetPackageOrderAsync")]
        [Authorize]
        public async Task<IActionResult> GetPackageOrderAsync(Guid id)
        {
            var contacts = await companyRepository.GetOrderAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                var contactsDto = mapper.Map<OrderModelDataDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("package-orders/{id:guid}")]
        [Authorize(Roles = "Super_Admin")]
        public async Task<IActionResult> DeletePackageOrderAsync(Guid id)
        {
            // Get the region from the database
            var user = await companyRepository.DeleteOrderAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("gasStation Does not Exist");
            }

            else
            {
                var userDto = mapper.Map<OrderModelDataDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPost("package-orders")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> AddOrderAsync([FromHeader] Guid companyId, [FromBody] AddOrder request)
        {
            var check = ValidateOrder(request);

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
                var user = await newUsersRepository.GetAsync(userGuid);
                var company = await companyRepository.GetAsync(companyId);
               
                DateTime localTime = DateTime.Now;
                TimeZoneInfo nigeriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
                DateTime nigeriaTime = TimeZoneInfo.ConvertTimeFromUtc(localTime.ToUniversalTime(), nigeriaTimeZone);
                var contacts = new OrderModelData()
                {
                    UserId = user.Id,
                    TrackingNum = $"{CreateRandomTokenSix()}-{CreateRandomTokenFour()}-{CreateRandomTokenThree()}",
                    OrderStatus = "Open",
                    ItemCost=request.ItemCost,
                    ItemDescription=request.ItemDescription,
                    ItemModelNumber=request.ItemModelNumber,
                    ItemName=request.ItemName,
                    ItemQuantity=request.ItemQuantity,
                    ConfirmationImage="",
                    Comment="",
                    QRCode="",
                    PackageType=request.PackageType,
                    ExpectedDeliveryTime="",
                    CurrentLatitude= request.SenderLatitude,
                    CurrentLongitude= request.SenderLongitude,
                    CurrentLocation= request.SenderAddress,
                    // Company
                    CompanyId = company.Id,
                    CompanyAddress=company.CompanyAddress,
                    CompanyEmail=company.CompanyEmail,
                    CompanyName=company.CompanyName,
                    CompanyPhoneNo=company.PhoneNumber,
                    // Sender
                    SenderName = $"{user.FirstName} {user.LastName}",
                    SenderPhoneNo = user.PhoneNo,
                    SenderEmail = user.Email,
                    SenderAddress = request.SenderAddress,
                    SenderLocality = request.SenderState,
                    SenderState = request.SenderState,
                    SenderPostalCode = request.SenderPostalCode,
                    SenderLatitude = request.SenderLatitude,
                    SenderLongitude = request.SenderLongitude,
                    // Receiver
                    RecieverAddress= request.RecieverAddress,
                    RecieverEmail= request.RecieverEmail,
                    RecieverLocality= request.RecieverLocality,
                    RecieverName= request.RecieverName,
                    RecieverState= request.RecieverState,
                    RecieverPhoneNo= request.RecieverPhoneNo,
                    RecieverPostalCode= request.RecieverPostalCode,
                    RecieverLatitude= request.RecieverLatitude,
                    RecieverLongitude= request.RecieverLongitude,
                    ShippingCost=0.001,
                    TrnxReference = "",
                    PaymentStatus = false,
                    PaymentChannel = "",
                    RiderId = Guid.Empty,
                    RiderName = "",
                    UpdatedAt = nigeriaTime,
                    CreatedAt = nigeriaTime,
                   
                };
                // Pass detials to repository
                contacts = await companyRepository.AddOrderAsync(contacts);
                var datra = await companyRepository.GetOrderAsync(contacts.Id);
                var contactsDto = mapper.Map<OrderModelDataDto>(datra);
                // await _hubContext.Clients.All.SendAsync("ReceiveMessage", user.RiderName, $"The Rider has the Order {user.OrderNum} on {user.OrderStatus} Status");
                // var contactsDto = mapper.Map<GasOrderModelDataDto>(order);
                // await _gasStationHubContext.Clients.Group(user.Id.ToString()).SendAsync("ORDER", contactsDto);
                return Ok(contactsDto);
            }
        }


        [HttpPut]
        [Route("package-orders/{id:guid}")]
        [Authorize(Roles = "Rider_User,User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateGasOrderAsync([FromRoute] Guid id, [FromBody] UpdateOrder request)
        {

            DateTime localTime = DateTime.Now;
            TimeZoneInfo nigeriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            DateTime nigeriaTime = TimeZoneInfo.ConvertTimeFromUtc(localTime.ToUniversalTime(), nigeriaTimeZone);

           var userDto1 = await companyRepository.GetOrderAsync(id);

            // check the null value
            if (userDto1 == null)
            {
                return BadRequest("User Does not Exist");
            }
            // convert back to dto
            else
            {

                var user = new OrderModelData()
                {
                    OrderStatus = !string.IsNullOrWhiteSpace(request.OrderStatus) ? request.OrderStatus : userDto1.OrderStatus,
                    ExpectedDeliveryTime= !string.IsNullOrWhiteSpace(request.ExpectedDeliveryTime) ? request.ExpectedDeliveryTime : userDto1.ExpectedDeliveryTime,
                    ConfirmationImage = !string.IsNullOrWhiteSpace(request.ConfirmationImage) ? request.ConfirmationImage : userDto1.ConfirmationImage,
                    ShippingCost = (double)(request.ShippingCost ?? userDto1.ShippingCost),
                    TrnxReference = !string.IsNullOrWhiteSpace(request.TrnxReference) ? request.TrnxReference : userDto1.TrnxReference,
                    PaymentChannel = !string.IsNullOrWhiteSpace(request.PaymentChannel) ? request.PaymentChannel : userDto1.PaymentChannel,
                    PaymentStatus = (bool)(request.PaymentStatus ?? userDto1.PaymentStatus),
                    Comment = !string.IsNullOrWhiteSpace(request.Comment) ? request.Comment : userDto1.Comment,
                    CurrentLatitude = (double)(request.CurrentLatitude ?? userDto1.CurrentLatitude),
                    CurrentLongitude = (double)(request.CurrentLongitude ?? userDto1.CurrentLongitude),
                    CurrentLocation = !string.IsNullOrWhiteSpace(request.CurrentLocation) ? request.CurrentLocation : userDto1.CurrentLocation,
                    UpdatedAt = nigeriaTime,
                };

                // Update detials to repository
                user = await companyRepository.UpdateOrderAsync(id, user);
                var datra = await companyRepository.GetOrderAsync(user.Id);
                var contactsDto = mapper.Map<OrderModelDataDto>(datra);
                // await _hubContext.Clients.All.SendAsync("ReceiveMessage", user.RiderName, $"The Rider has the Order {user.OrderNum} on {user.OrderStatus} Status");
                // var contactsDto = mapper.Map<GasOrderModelDataDto>(order);
                // await _gasStationHubContext.Clients.Group(user.Id.ToString()).SendAsync("ORDER", contactsDto);
                return Ok(contactsDto);
            }
        }

        [HttpPut]
        [Route("package-orders-rider/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateRiderAsync([FromRoute] Guid id, [FromHeader] Guid riderId)
        {
            DateTime localTime = DateTime.Now;
            TimeZoneInfo nigeriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            DateTime nigeriaTime = TimeZoneInfo.ConvertTimeFromUtc(localTime.ToUniversalTime(), nigeriaTimeZone);

            var rider = await ridersRepository.GetAsync(riderId);
            if (rider == null)
            {
                return BadRequest("Rider Does not Exist");
            }
            else
            {
                var user = new OrderModelData()
                {
                    RiderId = rider.Id,
                    RiderName = $"{rider.Name}",
                };

                // Update detials to repository
                user = await companyRepository.AssignRiderAsync(id, user);
                // check the null value
                if (user == null)
                {
                    return BadRequest("User Does not Exist");
                }
                // convert back to dto
                else
                {
                    var user2 = new OrderModelData()
                    {
                        OrderStatus = "Open",
                        ExpectedDeliveryTime = user.ExpectedDeliveryTime,
                        ConfirmationImage = user.ConfirmationImage,
                        ShippingCost = user.ShippingCost,
                        TrnxReference = user.TrnxReference,
                        PaymentChannel = user.PaymentChannel,
                        PaymentStatus = user.PaymentStatus,
                        Comment = user.Comment,
                        CurrentLatitude = user.CurrentLatitude,
                        CurrentLongitude = user.CurrentLongitude,
                        CurrentLocation = user.CurrentLocation,
                        UpdatedAt = nigeriaTime,
                    };

                    // Update detials to repository
                    user2 = await companyRepository.UpdateOrderAsync(id, user2);
                    var datra = await companyRepository.GetOrderAsync(user2.Id);
                    var contactsDto = mapper.Map<OrderModelDataDto>(datra);
                    // await _hubContext.Clients.All.SendAsync("ReceiveMessage", user.RiderName, $"The Rider has the Order {user.OrderNum} on {user.OrderStatus} Status");
                    // var contactsDto = mapper.Map<GasOrderModelDataDto>(order);
                    // await _gasStationHubContext.Clients.Group(user.Id.ToString()).SendAsync("ORDER", contactsDto);
                    return Ok(contactsDto);

                }
            }
        }


        [HttpPut]
        [Route("package-orders-image/{id:guid}")]
        [Authorize(Roles = "Rider_User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdatePurchaseImageAsync([FromRoute] Guid id, UpdatesPurchaseImages request)
        {
            List<PackageImageList> ticketBonus = [];
            var rtnlist = new List<string>();
            DateTime localTime = DateTime.Now;
            TimeZoneInfo nigeriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            DateTime nigeriaTime = TimeZoneInfo.ConvertTimeFromUtc(localTime.ToUniversalTime(), nigeriaTimeZone);
            // Update detials to repository
            var user = await companyRepository.GetOrderAsync(id);

            // check the null value
            if (user == null)
            {
                return BadRequest("Order Does not Exist");
            }
            // convert back to dto
            else
            {
                for (var i = 0; i < request.ImageURL!.Count; i++)
                {
                    rtnlist.Add(request.ImageURL[i]);
                    var languageUpdate = new PackageImageList()
                    {
                        OrderModelDataId = user.Id,
                        ImageUrlFile = rtnlist[i],
                    };
                    languageUpdate = await companyRepository.AddPackageImagesAsync(languageUpdate);
                    ticketBonus.Add(languageUpdate);
                }

                var datra = await companyRepository.GetOrderAsync(user.Id);
                var contactsDto = mapper.Map<OrderModelDataDto>(datra);
                // await _hubContext.Clients.All.SendAsync("ReceiveMessage", user.RiderName, $"The Rider has the Order {user.OrderNum} on {user.OrderStatus} Status");
                // var contactsDto = mapper.Map<GasOrderModelDataDto>(order);
                // await _gasStationHubContext.Clients.Group(user.Id.ToString()).SendAsync("ORDER", contactsDto);
                return Ok(contactsDto);
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
        private static void SendEmailAvailableCode(string emailId, string activationcode, string username)
        {

            var fromMail = new MailAddress("office@cnticketstravels.com", "CN Ticket & Travels Ltd");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "Chukwudi21$";
            string subject = " Email Available Token";
            string body = "<br/><br/>" + username + "  Your Email verification token!<br/>" +
              " Please copy on the below code and verify your account now" +
              " <br/><br/><strong><center " + activationcode + "'>" + activationcode + "</strong></center> ";

            var smtp = new SmtpClient
            {

                Host = "mail.cnticketstravels.com",
                Port = 8889,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)

            };
            using var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            smtp.Send(message);
        }
        private static void SendChangePasswordCodeEmail(string emailId, string activationcode, string username)
        {
            //var varifyUrl = scheme + "://" + host + ":" + port + "/JobSeeker/ActivateAccount/" + activationcode;
            var fromMail = new MailAddress("office@cnticketstravels.com", "CN Ticket & Travels.");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "Chukwudi21$";
            string subject = username + " Passsword Reset Token";
            string body = "<br/><br/>" + username + "  Your Password Reset token!<br/>" +
              " Please copy on the below code and verify your account now" +
              " <br/><br/><strong><center " + activationcode + "'>" + activationcode + "</strong></center> ";

            var smtp = new SmtpClient
            {
                Host = "mail.cnticketstravels.com",
                Port = 8889,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)

            };
            using var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            smtp.Send(message);
        }
        private bool ValidateCompany(AddCompany request)
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
        private bool ValidateRiders(AddRiders request)
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
        private bool ValidateChat(AddChatMessage request)
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

        private bool ValidateOrder(AddOrder request)
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
        private async Task<string> SendNotification(Guid userId,Guid managerId,Guid riderId, string deviceToken, string names, double gasInputkg, string gasStation, string gasStationImage)
        {


            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = "Customer Order Request",
                    Body = $"{names} Has Open an Order of {gasInputkg}kg from {gasStation}",
                    ImageUrl = $"{gasStationImage}"
                },
                Data = new Dictionary<string, string>()
                {
                    ["CustomData"] = "Hello, how are you doing?"
                },
                Token = deviceToken
            };

            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");

            var contacts = new NotificationModel()
            {
                UserId = userId,
                Title = message.Notification.Title,
                Body = message.Notification.Body,
                DeviceToken = message.Token,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                ImageUrl = message.Notification.ImageUrl,
                NotificationType = "Orders",
            };
            // Pass detials to repository
            contacts = await notificationRepository.AddAsync(contacts);
            return result;
        }

        #endregion
    }
}
