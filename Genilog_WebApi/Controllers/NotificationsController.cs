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

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController(IMapper mapper, IHostEnvironment _env, IGeneralUserRepository generalUserRepository,INotificationRepository notificationRepository, IHubContext<NotificationHub> _hubContext) : ControllerBase
    {
        private readonly IMapper mapper = mapper;
        private readonly IHostEnvironment _env = _env;
        private readonly IGeneralUserRepository generalUserRepository = generalUserRepository;
        private readonly INotificationRepository notificationRepository = notificationRepository;
        readonly string keyPath = Path.Combine(_env.ContentRootPath, "Key\\bmg-project-edf2f-firebase-adminsdk-gqzbj-f6515ebae6.json");
        private readonly IHubContext<NotificationHub> _hubContext= _hubContext;
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllNotificationAsync()
        {
            var contacts = await notificationRepository.GetAllAsync();
            var contactsDto = mapper.Map<List<NotificationModelDto>>(contacts);
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
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                DeviceToken = !string.IsNullOrWhiteSpace(request.DeviceToken) ? request.DeviceToken : userDto1.DeviceToken,
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
                var timeStamp = Timestamp.GetCurrentTimestamp();
                DocumentReference usrRef = firestoreDb!.Collection("NotificationsCollections").Document(user.Id.ToString());
                Dictionary<string, object> user3 = new()
                {
                    {"Title",user.Title!},
                    {"Body",user.Body!},
                    {"DeviceToken",user.DeviceToken!},
                    {"UpdatedAt",timeStamp},
                };
                await usrRef.UpdateAsync(user3);
                var userDto = new NotificationModelDto()
                {
                    Id = user.Id,
                    UserId = user.UserId,
                    Title = user.Title,
                    Body = user.Body,
                    DeviceToken = user.DeviceToken,
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
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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

        [HttpPost("send-hub")]
        [Authorize]
        public async Task<IActionResult> AddNotificationHubAsync(AddNotification request)
        {
            var check = ValidateNotification(request);

            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", request.Title, request.Body);
                return Ok("Error sending the message");
               

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
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
            var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
            var timeStamp = Timestamp.GetCurrentTimestamp();
            var contacts = new NotificationModel()
            {
                UserId = userId,
                Title = message.Notification.Title,
                Body = message.Notification.Body,
                DeviceToken = message.Token,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                ImageUrl = message.Notification.ImageUrl,
                NotificationType = notificationType,
            };
            // Pass detials to repository
            contacts = await notificationRepository.AddAsync(contacts);
            DocumentReference usrRef = firestoreDb!.Collection("NotificationsCollections").Document(contacts.Id.ToString());

            Dictionary<string, object> user3 = new()
                {
                    {"Id",contacts.Id.ToString()},
                    {"UserId",contacts.UserId.ToString()},
                    {"Title", contacts.Title!},
                    {"Body", contacts.Body!},
                    {"DeviceToken", contacts.DeviceToken!},
                    {"ImageUrl",contacts.ImageUrl!},
                    {"NotificationType",contacts.NotificationType!},
                    {"UpdatedAt",timeStamp},
                    {"DatePublished",date!},
                    {"Timestamp",timeStamp!}
                };
            await usrRef.SetAsync(user3);
            return result;
        }


        private async Task<string> SendNotificationHub( string user, string message, string notificationType, Guid userId)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
           
            var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
            var timeStamp = Timestamp.GetCurrentTimestamp();
            var contacts = new NotificationModel()
            {
                UserId = userId,
                Title = "",
                Body = message,
                DeviceToken = "",
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                ImageUrl = "",
                NotificationType = notificationType,
            };
            // Pass detials to repository
            contacts = await notificationRepository.AddAsync(contacts);
            DocumentReference usrRef = firestoreDb!.Collection("NotificationsCollections").Document(contacts.Id.ToString());

            Dictionary<string, object> user3 = new()
                {
                    {"Id",contacts.Id.ToString()},
                    {"UserId",contacts.UserId.ToString()},
                    {"Title", contacts.Title!},
                    {"Body", contacts.Body!},
                    {"DeviceToken", contacts.DeviceToken!},
                    {"ImageUrl",contacts.ImageUrl!},
                    {"NotificationType",contacts.NotificationType!},
                    {"UpdatedAt",timeStamp},
                    {"DatePublished",date!},
                    {"Timestamp",timeStamp!}
                };
            await usrRef.SetAsync(user3);
            return "Successfully";
        }


        #endregion
    }
}
