using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Genilog_WebApi.Repository.InfoRepo;
using Genilog_WebApi.Model.InfoModel;
using Genilog_WebApi.Key;
using Google.Cloud.Firestore;
using System.Net.Mail;
using System.Net;
using Genilog_WebApi.EmailSender;
using Genilog_WebApi.Repository.NotificationRepo;
using Microsoft.AspNetCore.SignalR;
using Genilog_WebApi.Repository.UserRepo;
using Genilog_WebApi.Model.UsersDataModel;

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController(IFeedbackRepository contactUsRepository,
        IMapper mapper, IHostEnvironment _env, IUserRepository newUsersRepository) : ControllerBase
    {
        private readonly IFeedbackRepository contactUsRepository = contactUsRepository;
        private readonly IUserRepository newUsersRepository = newUsersRepository;
        private readonly IMapper mapper = mapper;
        private readonly IHostEnvironment _env = _env;
        [HttpGet("feedback")]
        [Authorize(Roles = "Super_Admin,Admin")]
        public async Task<IActionResult> GetAllRiderPayoutAsync([FromQuery] SearchFeedback search)
        {
            var events = await contactUsRepository.GetAllAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var allPosts = events.AsQueryable();

            if (!string.IsNullOrEmpty(search.Search))
            {
                allPosts = allPosts.Where(x => x.Name!.Contains(search.Search) ||
                x.Email!.Contains(search.Search) || x.PhoneNo!.Contains(search.Search));
                var userDto = mapper.Map<List<FeedbackModelDataDto>>(allPosts);

                return Ok(userDto);
            }
            else
            {
                var userDto = mapper.Map<List<FeedbackModelDataDto>>(events);
                return Ok(userDto);
            }
        }

        [HttpGet]
        [Route("feedback/{id:guid}")]
        [ActionName("GetFeedbackAsync")]
        [Authorize(Roles = "Super_Admin,Admin")]
        public async Task<IActionResult> GetFeedbackAsync(Guid id)
        {
            var contacts = await contactUsRepository.GetAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                var contactsDto = mapper.Map<FeedbackModelDataDto>(contacts);
                return Ok(contactsDto);
            }

        }


        [HttpPost("feedback")]
        public async Task<IActionResult> AddFeedbackAsync(AddFeedback request)
        {
            var check = ValidateTicket(request);

            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();
                var contacts = new FeedbackModelData()
                {
                    Name = request.Name,
                    PhoneNo = request.PhoneNo,
                    Email = request.Email,
                    Feedback = request.Feedback,
                    DatePublished = date,
                    CreatedAt = DateTime.UtcNow,
                };
                // Pass detials to repository
                contacts = await contactUsRepository.AddAsync(contacts);

                // convert back to dto
                var contactsDto = new FeedbackModelDataDto()
                {
                    Name = contacts.Name,
                    PhoneNo = contacts.PhoneNo,
                    Email = contacts.Email,
                    Feedback = contacts.Feedback,
                    CreatedAt = contacts.CreatedAt,
                };
                return CreatedAtAction(nameof(GetFeedbackAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }

        [HttpPost("send-mail")]
        [Authorize(Roles = "Super_Admin,Admin")]
        public IActionResult SendMail(SendMailModel request)
        {
            var check = ValidateMail(request);

            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                EmailTemplates.SendEmail(request.Email!, request.Message!, request.Title!, request.Name!, ""!);
                return Ok($"Mail Sent Successfully");
            }
        }

        [HttpPost("send-all-mail")]
        [Authorize(Roles = "Super_Admin,Admin")]
        public async Task<IActionResult> SendAllMail(SendMailModel2 request)
        {
            var check = ValidateMail(request);

            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var user = await newUsersRepository.GetAllAsync();
                var userDto = mapper.Map<List<UsersDataModelTableDto>>(user);
                for (int i = 0; i < userDto.Count; i++)
                {
                    EmailTemplates.SendEmail(userDto[i].Email!, request.Message!, request.Title!, $"{userDto[i].FirstName} {userDto[i].LastName}", request.Link!);
                }
                return Ok($"Mail Sent Successfully");
            }
        }

        #region private methods

        private bool ValidateTicket(AddFeedback request)
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

        private bool ValidateMail(SendMailModel request)
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

        private bool ValidateMail(SendMailModel2 request)
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
