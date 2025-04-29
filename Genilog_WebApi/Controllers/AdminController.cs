using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Genilog_WebApi.Repository.AdminRepo;
using Genilog_WebApi.Repository.AuthRepo;
using Genilog_WebApi.Key;
using Genilog_WebApi.Model.AdminsModel;
using Genilog_WebApi.Repository.UploadRepo;
using Google.Cloud.Firestore;
using Genilog_WebApi.Model.UploadModels;
using Genilog_WebApi.Model.AuthModel;
using Genilog_WebApi.Model;
using System.Security.Claims;
using Genilog_WebApi.EmailSender;
using Genilog_WebApi.Repository.NotificationRepo;
using Microsoft.AspNetCore.SignalR;
using Genilog_WebApi.Model.BookingsModel;
using Genilog_WebApi.Repository.BookingsRepo;


namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IHostEnvironment _env, IAdminRepository usersRepository, IMapper mapper, ITokenHandler tokenHandler, IRolesRepository rolesRepository,
     IUser_RoleRepository user_RoleRepository, IUploadRepository uploadRepository, IGeneralUserRepository generalUserRepository) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IAdminRepository usersRepository = usersRepository;
        private readonly ITokenHandler tokenHandler = tokenHandler;
        private readonly IMapper mapper = mapper;
        private readonly IRolesRepository rolesRepository = rolesRepository;
        private readonly IUser_RoleRepository user_RoleRepository = user_RoleRepository;
        private readonly IUploadRepository uploadRepository = uploadRepository;
        private readonly IGeneralUserRepository generalUserRepository = generalUserRepository;
        readonly string keyPath = Path.Combine(_env.ContentRootPath, "Key\\ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json");

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginRequset requset)
        {

            var user = await generalUserRepository.AuthenticateAsync(requset.Email_PhoneNo!, requset.Password!);
            if (user != null)
            {
                var userD = await usersRepository.GetAsync(user.Id);
                if (user.UserType == "Super_Admin"|| user.UserType == "Admin")
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
                    return BadRequest("Not An Admin Account");
                }
            }
            else
            {
                return BadRequest("Admin Does not Exist");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Super_Admin")]
        public async Task<IActionResult> GetAllAdminAsync()
        {
            List<AdminModelTableDto> dto = [];
            var users = await usersRepository.GetAllAsync();
            var userDto = mapper.Map<List<AdminModelTableDto>>(users);
            foreach (var item in userDto)
            {
                var t = await generalUserRepository.GetAsync(item.Id);
                item.AdminType = t.UserType;
                dto.Add(item);
            }
            return Ok(dto);
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
                var t = await generalUserRepository.GetAsync(user.Id);
                userDto.AdminType = t.UserType;
               
                return Ok(userDto);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Super_Admin")]
        public async Task<IActionResult> AddAdminAsync(AddAdminRequest request)
        {
            // Validate the request
            var check = await ValidateAddUserAsync(request);

            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                if (request.AdminType == "Super_Admin" || request.AdminType == "Admin") 
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
                        DatePublished = date,
                        CreatedAt = DateTime.UtcNow

                    };
                    // Pass detials to repository
                    users = await usersRepository.AddAsync(users);

                    var roles = new Roles()
                    {
                        Name = admin.UserType
                    };
                    roles = await rolesRepository.AddAsync(roles);
                    var user_Roles = new User_Role()
                    {
                        GeneralUsersId = users.Id,
                        RoleId = roles.Id,
                    };
                    await user_RoleRepository.AddAsync(user_Roles);
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
                EmailTemplates.SendChangePasswordCodeEmail(user.Email!, user.PasswordResetToken!, user.FirstName!);
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
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var userDto = mapper.Map<List<AdvertHolderModelDto>>(events);
            return Ok(userDto);
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
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
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
        [Authorize(Roles = "User,Manager,Admin,Super_Admin")]
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
                    AdvertItemCost = (request.AdvertItemCost ?? userDto1.AdvertItemCost),
                    AdvertItemDescription = !string.IsNullOrWhiteSpace(request.AdvertItemDescription) ? request.AdvertItemDescription : userDto1.AdvertItemDescription,
                    AdvertDays4 = (request.AdvertDays4 ?? userDto1.AdvertDays4),
                };
                // Update detials to repository
                user = await usersRepository.UpdateAdvertAsync(id, user);
                var contact = await usersRepository.GetAdvertAsync(user.Id);
                var userDto = mapper.Map<AdvertHolderModelDto>(contact);
                return Ok(userDto);
            }
        }

        [HttpPost("advert")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
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
                var contacts = new AdvertHolderModel()
                {
                    AdminId = userGuid,
                    AdvertItemId = request.AdvertItemId,
                    AdvertImage = request.AdvertImage,
                    AdvertItemDescription = request.AdvertItemDescription,
                    AdvertType = request.AdvertType,
                    AdvertName = request.AdvertName,
                    AdvertItemCost =(double)request.AdvertItemCost!,
                    AdvertDays4 = (int)request.AdvertDays4!,
                    CreatedAt = DateTime.Now,

                };
                // Pass detials to repository
                contacts = await usersRepository.AddAdvertAsync(contacts);

                // convert back to dto
                var contact = await usersRepository.GetAdvertAsync(contacts.Id);
                var contactsDto = mapper.Map<AdvertHolderModelDto>(contact);
                return CreatedAtAction(nameof(GetAdvertAsync), new { id = contactsDto.Id }, contactsDto);
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

        #endregion
    }
}
