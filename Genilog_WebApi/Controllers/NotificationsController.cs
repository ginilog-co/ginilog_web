using AutoMapper;
using FirebaseAdmin.Messaging;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Genilog_WebApi.Key;
using Genilog_WebApi.Model.Notification_Model;
using Genilog_WebApi.Repository.AuthRepo;
using Genilog_WebApi.Repository.NotificationRepo;
using Genilog_WebApi.Model.AuthModel;
using Genilog_WebApi.Model;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Google.Cloud.Firestore.V1;
using Microsoft.Extensions.Logging;

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController(IMapper mapper, IHostEnvironment _env, IGeneralUserRepository generalUserRepository,INotificationRepository notificationRepository) : ControllerBase
    {
        private readonly IMapper mapper = mapper;
        private readonly IHostEnvironment _env = _env;
        private readonly IGeneralUserRepository generalUserRepository = generalUserRepository;
        private readonly INotificationRepository notificationRepository = notificationRepository;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllNotificationAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var events = await notificationRepository.GetAllAsync();
            var valid = events.Where(x => x.UserId == userGuid);
            events = [.. valid.OrderByDescending(x => x.CreatedAt)];
            var contactsDto = mapper.Map<List<NotificationModelDto>>(events);
            return Ok(contactsDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetNotificationAsync")]
        [Authorize]
        public async Task<IActionResult> GetNotificationAsync(Guid id)
        {
            var contacts = await notificationRepository.GetAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                var contactsDto = mapper.Map<NotificationModelDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateNotificationAsync([FromRoute] Guid id, [FromBody] AddNotification request)
        {
            
            
         var userDto1 = await notificationRepository.GetAsync(id);

            // check the null value
            if (userDto1 == null)
            {
                return BadRequest("User Does not Exist");
            }
            var user = new NotificationModel()
            {

                Title = !string.IsNullOrWhiteSpace(request.Title) ? request.Title : userDto1.Title,
                Body = !string.IsNullOrWhiteSpace(request.Body) ? request.Body : userDto1.Body,
                IsRead = request.IsRead ?? userDto1.IsRead,
                UpdatedAt = DateTime.UtcNow
            };

            // Update detials to repository
            user = await notificationRepository.UpdateAsync(id, user);

            // check the null value
            if (user == null)
            {
                return BadRequest("User Does not Exist");
            }
            // convert back to dto
            else
            {
                var userDto = new NotificationModelDto()
                {
                    Id = user.Id,
                    UserId = user.UserId,
                    Title = user.Title,
                    Body = user.Body,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };
                return Ok(userDto);
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddNotificationAsync( AddNotification request)
        {
            
            
            var check = ValidateNotification(request);

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
                var notify = await generalUserRepository.GetAllDeviceTokenAsync();
                notify = notify.Where(c => c.UserId == userGuid).ToList();
                List<DeviceTokenModel> sendinh = notify.ToList();
                string result = "";
                for (int i = 0; i < sendinh.Count; i++)
                {
                    result = await SendNotification(sendinh[i].DeviceTokenId!, $"{request.Title}", request.Body!, "https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Fplaystore-icon.png?alt=media&token=8f700536-b14d-4f3b-b35b-0275963461cd", request.NotificationType!, userGuid);
                }
                if (!string.IsNullOrEmpty(result))
                {
                    var userDto = new ResponseModel()
                    {
                        Message = "Added Notifications",
                        Status = true,
                    };

                    return Ok(userDto);

                }
                else
                {
                    return Ok("Error sending the message");
                }

            }
        }

       

        [HttpPost("send-notification")]
        [Authorize]
        public async Task<IActionResult> SendNotificationHubAsync(AddNotification request)
        {
            
            
            var check = ValidateNotification(request);

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
                var contacts = new NotificationModel()
                {
                    UserId = userGuid,
                    Title = request.Title,
                    Body = request.Body,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = "",
                    NotificationType = request.NotificationType,
                };
                // Pass detials to repository
                await notificationRepository.AddAsync(contacts);
               
                return Ok("Notification Sends");
               

            }
        }

        #region private methods
        private bool ValidateNotification(AddNotification request)
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

        private async Task<string> SendNotification(string deviceToken, string title,string body, string imageUrl,string notificationType, Guid userId)
        {
            
            
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                    ImageUrl = imageUrl
                },
                Data = new Dictionary<string, string>()
                {
                    ["CustomData"] = "Hello, how are you doing?"
                },
                Token = deviceToken
            };

            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            var contacts = new NotificationModel()
            {
                UserId = userId,
                Title = message.Notification.Title,
                Body = message.Notification.Body,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                ImageUrl = message.Notification.ImageUrl,
                NotificationType = notificationType,
            };
            // Pass detials to repository
            await notificationRepository.AddAsync(contacts);
         
            return result;
        }

        #endregion
    }
}
