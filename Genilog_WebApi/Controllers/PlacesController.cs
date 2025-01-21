using AutoMapper;
using Genilog_WebApi.Key;
using Genilog_WebApi.Model.UploadModels;
using Genilog_WebApi.Repository.UploadRepo;
using Genilog_WebApi.Repository.UserRepo;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Genilog_WebApi.Repository.PlacesRepo;
using Genilog_WebApi.Model.PlacesModel;
using Genilog_WebApi.Model;
using Genilog_WebApi.Repository.AuthRepo;
using System.Security.Claims;
using Genilog_WebApi.Model.LogisticsModel;
using Genilog_WebApi.Repository.WalletRepo;
using Microsoft.AspNetCore.SignalR;
namespace Genilog_WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlacesController(IHostEnvironment _env, IMapper mapper, IHotelRepository hotelRepository
        , IGeneralUserRepository generalUserRepository, IUploadRepository uploadRepository, IPlacesRepository placesRepository
        , IHubContext<PlacesHubRepository> _hubContext) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IMapper mapper = mapper;
        private readonly IHotelRepository hotelRepository = hotelRepository;
        private readonly IGeneralUserRepository generalUserRepository = generalUserRepository;
        private readonly IUploadRepository uploadRepository = uploadRepository;
        private readonly IPlacesRepository placesRepository = placesRepository;
        private readonly IHubContext<PlacesHubRepository> _hubContext = _hubContext;
        readonly string keyPath = Path.Combine(_env.ContentRootPath, "Key\\ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json");


        // This Is PLACES SECTION
        [HttpGet]
       [Authorize]
        public async Task<IActionResult> GetAllPlacesAsync()
        {
            var contacts = await placesRepository.GetAllAsync();
            var contactsDto = mapper.Map<List<PlacesDataModelDto>>(contacts);
            await _hubContext.Clients.All.SendAsync("GetAllPlaces", contactsDto);
            return Ok(contactsDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetPlacesAsync")]
        [Authorize]
        public async Task<IActionResult> GetPlacesAsync(Guid id)
        {
            var contacts = await placesRepository.GetAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                var contactsDto = mapper.Map<PlacesDataModelDto>(contacts);
                await _hubContext.Clients.All.SendAsync($"GetPlaces{id}", contactsDto);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> DeletePlacesAsync(Guid id)
        {
            // Get the region from the database
            var user = await placesRepository.DeleteAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Hotel Does not Exist");
            }

            else
            {
                var userDto = mapper.Map<PlacesDataModelDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdatePlacesAsync([FromRoute] Guid id, [FromBody] UpdatePlaces request)
        {
            var userDto1 = await placesRepository.GetAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("Id Does not Exist");
            }
           
            // convert back to dto
            else
            {
                var user = new PlacesDataModel()
                {
                    PlaceName = !string.IsNullOrWhiteSpace(request.PlaceName) ? request.PlaceName : userDto1.PlaceName,
                    PlaceEmail = !string.IsNullOrWhiteSpace(request.PlaceEmail) ? request.PlaceEmail : userDto1.PlaceEmail,
                    CheckInTime = !string.IsNullOrWhiteSpace(request.CheckInTime) ? request.CheckInTime : userDto1.CheckInTime,
                    CheckOutTime = !string.IsNullOrWhiteSpace(request.CheckOutTime) ? request.CheckOutTime : userDto1.CheckOutTime,
                    PlaceWebsite = !string.IsNullOrWhiteSpace(request.PlaceWebsite) ? request.PlaceWebsite : userDto1.PlaceWebsite,
                    PlacePhoneNo = !string.IsNullOrWhiteSpace(request.PlacePhoneNo) ? request.PlacePhoneNo : userDto1.PlacePhoneNo,
                    Location = !string.IsNullOrWhiteSpace(request.Location) ? request.Location : userDto1.Location,
                    Locality = !string.IsNullOrWhiteSpace(request.Locality) ? request.Locality : userDto1.Locality,
                    Postcode = !string.IsNullOrWhiteSpace(request.Postcode) ? request.Postcode : userDto1.Postcode,
                    Latitude = (double)(request.Latitude ?? userDto1.Latitude),
                    Longitude = (double)(request.Longitude ?? userDto1.Longitude),
                    Rating = userDto1.Rating,
                    BookingAmount = (double)(request.BookingAmount ?? userDto1.BookingAmount),
                    PlaceAdvertType = !string.IsNullOrWhiteSpace(request.PlaceAdvertType) ? request.PlaceAdvertType : userDto1.PlaceAdvertType,
                    PlaceOverview = !string.IsNullOrWhiteSpace(request.PlaceOverview) ? request.PlaceOverview : userDto1.PlaceOverview,
                    PlacesAdditionalInfo = !string.IsNullOrWhiteSpace(request.PlacesAdditionalInfo) ? request.PlacesAdditionalInfo : userDto1.PlacesAdditionalInfo,
                    CancelationPolicy = !string.IsNullOrWhiteSpace(request.CancelationPolicy) ? request.CancelationPolicy : userDto1.CancelationPolicy,
                    PlacesHighlights = !string.IsNullOrWhiteSpace(request.PlacesHighlights) ? request.PlacesHighlights : userDto1.PlacesHighlights,
                    State = !string.IsNullOrWhiteSpace(request.State) ? request.State : userDto1.State,
                    Country = !string.IsNullOrWhiteSpace(request.Country) ? request.Country : userDto1.Country,
                    IsPayment = (bool)(request.IsPayment ?? userDto1.IsPayment),
                    Available = (bool)(request.Available ?? userDto1.Available),
                    TicketType = !string.IsNullOrWhiteSpace(request.TicketType) ? request.TicketType : userDto1.TicketType,
                    PlaceType = !string.IsNullOrWhiteSpace(request.PlaceType) ? request.PlaceType : userDto1.PlaceType,
                };
                // Update detials to repository
                user = await placesRepository.UpdateAsync(id, user);
                var contact = await placesRepository.GetAsync(user.Id);
                var userDto = mapper.Map<PlacesDataModelDto>(contact);
                return Ok(userDto);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> AddPlacesAsync( [FromBody] AddPlaces request)
        {
            var check = ValidatePlace(request);
           

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
                var date = DateTime.Now.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();
                List<PlaceImages> ticketBonus = [];
                var rtnlist = new List<string>();
                var contacts = new PlacesDataModel()
                {
                    PlacesMonday = new PlacesMondayModel()
                    {
                        HourStart = request.TimeSchedule!.Monday!.Start,
                        HourEnd = request.TimeSchedule!.Monday!.End,
                        IsClosed = request.TimeSchedule!.Monday!.IsClosed,
                    },
                    PlacesTuesday = new PlacesTuesdayModel()
                    {
                        HourStart = request.TimeSchedule!.Tuesday!.Start,
                        HourEnd = request.TimeSchedule!.Tuesday!.End,
                        IsClosed = request.TimeSchedule!.Tuesday!.IsClosed,
                    },
                    PlacesWednesday = new PlacesWednesdayModel()
                    {
                        HourStart = request.TimeSchedule!.Wednesday!.Start,
                        HourEnd = request.TimeSchedule!.Wednesday!.End,
                        IsClosed = request.TimeSchedule!.Wednesday!.IsClosed,
                    },
                    PlacesThursday = new PlacesThursdayModel()
                    {
                        HourStart = request.TimeSchedule!.Thursday!.Start,
                        HourEnd = request.TimeSchedule!.Thursday!.End,
                        IsClosed = request.TimeSchedule!.Thursday!.IsClosed,
                    },
                    PlacesFriday = new PlacesFridayModel()
                    {
                        HourStart = request.TimeSchedule!.Friday!.Start,
                        HourEnd = request.TimeSchedule!.Friday!.End,
                        IsClosed = request.TimeSchedule!.Friday!.IsClosed,
                    },
                    PlacesSaturday = new PlacesSaturdayModel()
                    {
                        HourStart = request.TimeSchedule!.Saturday!.Start,
                        HourEnd = request.TimeSchedule!.Saturday!.End,
                        IsClosed = request.TimeSchedule!.Saturday!.IsClosed,
                    },
                    PlacesSunday = new PlacesSundayModel()
                    {
                        HourStart = request.TimeSchedule!.Sunday!.Start,
                        HourEnd = request.TimeSchedule!.Sunday!.End,
                        IsClosed = request.TimeSchedule!.Sunday!.IsClosed,
                    },
                    AdminId = userGuid,
                    PlaceName = request.PlaceName,
                    PlaceEmail = request.PlaceEmail,
                    PlaceOverview = request.PlaceOverview,
                    PlacesHighlights = request.PlacesHighlights,
                    PlacesAdditionalInfo = request.PlacesAdditionalInfo,
                    CancelationPolicy = request.CancelationPolicy,
                    PlaceType = request.PlaceType,
                    CheckInTime = request.CheckInTime,
                    CheckOutTime = request.CheckOutTime,
                    PlaceWebsite = request.PlaceWebsite,
                    PlacePhoneNo = request.PlacePhoneNo,
                    Location = request.Location,
                    State = request.State,
                    Country = request.Country,
                    Locality = request.Locality,
                    Postcode = request.Postcode,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    BookingAmount = request.BookingAmount,
                    Rating = 0,
                    IsPayment = request.IsPayment,
                    TicketType = request.TicketType,
                    PlaceAdvertType = "",
                    Available=true,
                    PlaceLogo =request.PlaceLogo,
                    CreatedAt = DateTime.Now,

                };
                // Pass detials to repository
                contacts = await placesRepository.AddAsync(contacts);
                for (var i = 0; i < request.PlacesImages!.Count; i++)
                {
                    rtnlist.Add(request.PlacesImages[i]);
                    var imageUpdate = new PlaceImages()
                    {
                        PlacesDataModelId = contacts.Id,
                        ImagePath = rtnlist[i],
                    };
                    imageUpdate = await placesRepository.AddPlaceImageAsync(imageUpdate);
                    ticketBonus.Add(imageUpdate);
                }
               
                // convert back to dto
                var contact = await placesRepository.GetAsync(contacts.Id);
                var contactsDto = mapper.Map<PlacesDataModelDto>(contact);
                return CreatedAtAction(nameof(GetPlacesAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }

        [HttpPut("update-places-time-schedule/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdatePlacesTimeScheduleAsync([FromRoute] Guid id, AddTimeSchedule request)
        {
            var contacts = new PlacesDataModel()
            {
                PlacesMonday = new PlacesMondayModel()
                {
                    HourStart = request.Monday!.Start,
                    HourEnd = request.Monday!.End,
                    IsClosed = request.Monday!.IsClosed,
                },
                PlacesTuesday = new PlacesTuesdayModel()
                {
                    HourStart = request.Tuesday!.Start,
                    HourEnd = request.Tuesday!.End,
                    IsClosed = request.Tuesday!.IsClosed,
                },
                PlacesWednesday = new PlacesWednesdayModel()
                {
                    HourStart = request.Wednesday!.Start,
                    HourEnd = request.Wednesday!.End,
                    IsClosed = request.Wednesday!.IsClosed,
                },
                PlacesThursday = new PlacesThursdayModel()
                {
                    HourStart = request.Thursday!.Start,
                    HourEnd = request.Thursday!.End,
                    IsClosed = request.Thursday!.IsClosed,
                },
                PlacesFriday = new PlacesFridayModel()
                {
                    HourStart = request.Friday!.Start,
                    HourEnd = request.Friday!.End,
                    IsClosed = request.Friday!.IsClosed,
                },
                PlacesSaturday = new PlacesSaturdayModel()
                {
                    HourStart = request.Saturday!.Start,
                    HourEnd = request.Saturday!.End,
                    IsClosed = request.Saturday!.IsClosed,
                },
                PlacesSunday = new PlacesSundayModel()
                {
                    HourStart = request.Sunday!.Start,
                    HourEnd = request.Sunday!.End,
                    IsClosed = request.Sunday!.IsClosed,
                },
            };

            // Update detials to repository
            contacts = await placesRepository.UpdateTimeSheduleAsync(id, contacts);

            // check the null value
            if (contacts == null)
            {
                return BadRequest("Brand Does not Exist");
            }
            // convert back to dto
            else
            {
                var userDto = new ResponseModel()
                {
                    Message = "Updated Successfully",
                    Status = true
                };
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("update-places-images/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdatePlacesImageAsync([FromRoute] Guid id, [FromForm] AddImageList request)
        {
            // Update detials to repository

            List<PlaceImages> ticketBonus = [];
            var rtnlist = new List<string>();
            var events = await placesRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Event Does not Exist");
            }
            // convert back to dto
            else
            {
                var upload = new ImageListUpload()
                {
                    Image = request.Image,
                };
                var img = await uploadRepository.SaveListImageAsync(upload);
                for (var i = 0; i < img.ImagePath!.Count; i++)
                {
                    rtnlist.Add(img.ImagePath[i]);
                    var imageUpdate = new PlaceImages()
                    {
                        PlacesDataModelId = events.Id,
                        ImagePath = rtnlist[i],
                    };
                    imageUpdate = await placesRepository.AddPlaceImageAsync(imageUpdate);
                    ticketBonus.Add(imageUpdate);
                }
                var ticketBonusDtos = mapper.Map<List<PlaceImagesDto>>(ticketBonus);
                
                var userDto = new PlacesDataModelDto()
                {
                    Id = events.Id,
                    PlaceImages = ticketBonusDtos,

                };
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("update-places-facilities/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdatePlacesFacilitiesAsync([FromRoute] Guid id, AddPlacesFacilities request)
        {

            var events = await placesRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Places Does not Exist");
            }
            // convert back to dto
            else
            {
                var interested = new PlaceFacilities()
                {
                    Facilities = request.Facilities,
                    PlacesDataModelId = events.Id,
                };

                await placesRepository.AddPlaceFacilitiesAsync(interested);
               
                var userDto = new ResponseModel()
                {
                    Message = "Update Facilities",
                    Status = true,
                };

                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("update-places-what-to-Expect/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdatePlaceWhatToExpectAsync([FromRoute] Guid id, AddPlaceWhatToExpect request)
        {
            var events = await placesRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Places Does not Exist");
            }
            // convert back to dto
            else
            {
                var interested = new PlaceWhatToExpect()
                {
                    Titles = request.Titles,
                    Description = request.Description,
                    SubTitle = request.SubTitle,
                    PlacesDataModelId = events.Id,
                };

                await placesRepository.AddPlaceWhatToExpectAsync(interested);
                var userDto = new ResponseModel()
                {
                    Message = "Updated Successfully",
                    Status = true,
                };

                return Ok(userDto);
            }
        }


        [HttpPut]
        [Route("update-places-review/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdatePlaceReviewAsync([FromRoute] Guid id, [FromBody] AddPlacesReview request)
        {
            
            var events = await placesRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Place Does not Exist");
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
                var interested = new PlaceReviewModel()
                {
                    UserName = $"{user.FirstName} {user.LastName}",
                    ProfileImage = user.ImagePath,
                    PlacesDataModelId = events.Id,
                    UserId = user.Id,
                    ReviewMessage = request.ReviewMessage,
                    CreatedAt = DateTime.Now,
                };

                await placesRepository.AddPlaceReviewAsync(interested);
                var userDto = new ResponseModel()
                {
                    Message = "Update Facilities",
                    Status = true,
                };

                return Ok(userDto);
            }
        }


        // Places Chat Message
        [HttpGet("all-places-chat-messages")]
        [Authorize]
        public async Task<IActionResult> GetAllPlacesChatAsync()
        {
            List<PlacesChatModelDto> data = [];
            var events = await placesRepository.GetAllPlacesChatAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var userDto = mapper.Map<List<PlacesChatModelDto>>(events);
            for (var i = 0; i < userDto.Count; i++)
            {
                var sender = await generalUserRepository.GetAsync(userDto[i].SenderId);
                var receiver = await placesRepository.GetAsync(userDto[i].ReceiverId);
                userDto[i].SenderProfileImage = sender.ImagePath;
                userDto[i].SenderUserName = $"{sender.FirstName} {sender.LastName}";
                userDto[i].ReceiverProfileImage = receiver.PlaceLogo;
                userDto[i].ReceiverUserName = $"{receiver.PlaceName}";
                data.Add(userDto[i]);
            }
            var lid = data.OrderByDescending(x => x.CreatedAt).ToList();
            var userDto2 = mapper.Map<List<PlacesChatModelDto>>(lid);
            return Ok(userDto2);
        }

        [HttpGet("places-chat-messages")]
        [Authorize]
        public async Task<IActionResult> GetPlacesChatAsync([FromHeader] Guid senderId, [FromHeader] Guid receiverId)
        {
            var sender = await generalUserRepository.GetAsync(senderId);
            var receiver = await placesRepository.GetAsync(receiverId);
            if (sender == null)
            {
                var error = new ResponseModel()
                {
                    Message = "User Does not Exist",
                    Status = true,
                };
                return BadRequest(error);
            }
            else if (receiver == null)
            {
                var error = new ResponseModel()
                {
                    Message = "User Does not Exist",
                    Status = true,
                };
                return BadRequest(error);
            }
            else
            {
                List<PlacesChatModelDto> data = [];

                var groupChatId = $"{sender.Id}-{receiver.Id}";
                var events = await placesRepository.GetAllPlacesChatAsync();
                var valid = events.Where(x => x.GroupChatId == groupChatId);
                events = [.. valid.OrderByDescending(x => x.CreatedAt)];
                var userDto = mapper.Map<List<PlacesChatModelDto>>(events);
                for (var i = 0; i < userDto.Count; i++)
                {
                    userDto[i].SenderProfileImage = sender.ImagePath;
                    userDto[i].SenderUserName = $"{sender.FirstName} {sender.LastName}";
                    userDto[i].ReceiverProfileImage = receiver.PlaceLogo;
                    userDto[i].ReceiverUserName = $"{receiver.PlaceName}";
                    data.Add(userDto[i]);
                }
                var lid = data.OrderByDescending(x => x.CreatedAt).ToList();
                var userDto2 = mapper.Map<List<PlacesChatModelDto>>(lid);
                return Ok(userDto2);
            }
        }

        [HttpPost]
        [Route("places-chat-messages")]
        [Authorize]
        public async Task<IActionResult> AddPlacesChatAsync([FromHeader] Guid senderId, [FromHeader] Guid receiverId, [FromBody] AddChatMessage message)
        {
            var validate = ValidateChat(message);

            if (!validate)
            {
                return BadRequest("Product Already Saved");
            }
            else
            {
                var sender = await generalUserRepository.GetAsync(senderId);
                var receiver = await generalUserRepository.GetAsync(receiverId);
                if (sender == null)
                {
                    var error = new ResponseModel()
                    {
                        Message = "User Does not Exist",
                        Status = true,
                    };
                    return BadRequest(error);
                }
                else if (receiver == null)
                {
                    var error = new ResponseModel()
                    {
                        Message = "User Does not Exist",
                        Status = true,
                    };
                    return BadRequest(error);
                }
                else
                {
                    var interested = new PlacesChatModel()
                    {
                        SenderId = sender.Id,
                        ReceiverId = receiver.Id,
                        Message = message.Message,
                        GroupChatId = $"{sender.Id}-{receiver.Id}",
                        MessageType = message.MessageType,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                    };

                   await placesRepository.AddPlacesChatAsync(interested);
                    var userDto = new ResponseModel()
                    {
                        Message = "Success",
                        Status = true,
                    };

                    return Ok(userDto);
                }
            }
        }



        // This Is HOTEL SECTION
        [HttpGet]
        [Route("hotel")]
        public async Task<IActionResult> GetAllHotelAsync()
        {
            var contacts = await hotelRepository.GetAllAsync();
            var contactsDto = mapper.Map<List<HotelDataModelDto>>(contacts);
            await _hubContext.Clients.All.SendAsync("GetAllHotel", contactsDto);
            return Ok(contactsDto);
        }

        [HttpGet]
        [Route("hotel/{id:guid}")]
        [ActionName("GetHotelAsync")]
        public async Task<IActionResult> GetHotelAsync(Guid id)
        {
            var contacts = await hotelRepository.GetAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {

                var contactsDto = mapper.Map<HotelDataModelDto>(contacts);
                await _hubContext.Clients.All.SendAsync($"GetHotel{id}", contactsDto);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("hotel/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> DeleteHotelAsync(Guid id)
        {
            
            
            // Get the region from the database
            var user = await hotelRepository.DeleteAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Hotel Does not Exist");
            }

            else
            {
                var userDto = mapper.Map<HotelDataModelDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("hotel/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdateHotelAsync([FromRoute] Guid id, [FromBody] UpdateHotel request)
        {
            
            
            var userDto1 = await hotelRepository.GetAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("Id Does not Exist");
            }
           
            // convert back to dto
            else
            {
                var user = new HotelDataModel()
                {
                    HotelName = !string.IsNullOrWhiteSpace(request.HotelName) ? request.HotelName : userDto1.HotelName,
                    HotelDescription = !string.IsNullOrWhiteSpace(request.HotelDescription) ? request.HotelDescription : userDto1.HotelDescription,
                    CheckInTime = !string.IsNullOrWhiteSpace(request.CheckInTime) ? request.CheckInTime : userDto1.CheckInTime,
                    CheckOutTime = !string.IsNullOrWhiteSpace(request.CheckOutTime) ? request.CheckOutTime : userDto1.CheckOutTime,
                    HotelWebsite = !string.IsNullOrWhiteSpace(request.HotelWebsite) ? request.HotelWebsite : userDto1.HotelWebsite,
                    HotelPhoneNo = !string.IsNullOrWhiteSpace(request.HotelPhoneNo) ? request.HotelPhoneNo : userDto1.HotelPhoneNo,
                    NoOfRooms = (int)(request.NoOfRooms ?? userDto1.NoOfRooms),
                    State = !string.IsNullOrWhiteSpace(request.State) ? request.State : userDto1.State,
                    Country = !string.IsNullOrWhiteSpace(request.Country) ? request.Country : userDto1.Country,
                    Location = !string.IsNullOrWhiteSpace(request.Location) ? request.Location : userDto1.Location,
                    Locality = !string.IsNullOrWhiteSpace(request.Locality) ? request.Locality : userDto1.Locality,
                    Postcode = !string.IsNullOrWhiteSpace(request.Postcode) ? request.Postcode : userDto1.Postcode,
                    Latitude = (double)(request.Latitude ?? userDto1.Latitude),
                    Longitude = (double)(request.Longitude ?? userDto1.Longitude),
                    Rating =userDto1.Rating,
                    BookingAmount = (double)(request.BookingAmount ?? userDto1.BookingAmount),
                    HotelAdvertType = !string.IsNullOrWhiteSpace(request.HotelAdvertType) ? request.HotelAdvertType : userDto1.HotelAdvertType,
                    HotelEmail = !string.IsNullOrWhiteSpace(request.HotelEmail) ? request.HotelEmail : userDto1.HotelEmail,
                    Available = (bool)(request.Available ?? userDto1.Available),
                };

                // Update detials to repository
                user = await hotelRepository.UpdateAsync(id, user);
             
                
                var contacts = await hotelRepository.GetAsync(user.Id);
                var userDto = mapper.Map<HotelDataModelDto>(contacts);
                return Ok(userDto);
            }
        }

        [HttpPost("hotel")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> AddHotelAsync([FromBody] AddHotel request)
        {
            var check = ValidateHotel(request);

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
                List<HotelImages> ticketBonus = [];
                var rtnlist = new List<string>();
                var contacts = new HotelDataModel()
                {
                    HotelMonday = new HotelMondayModel()
                    {
                        HourStart = request.TimeSchedule!.Monday!.Start,
                        HourEnd = request.TimeSchedule!.Monday!.End,
                        IsClosed = request.TimeSchedule!.Monday!.IsClosed,
                    },
                    HotelTuesday = new HotelTuesdayModel()
                    {
                        HourStart = request.TimeSchedule!.Tuesday!.Start,
                        HourEnd = request.TimeSchedule!.Tuesday!.End,
                        IsClosed = request.TimeSchedule!.Tuesday!.IsClosed,
                    },
                    HotelWednesday = new HotelWednesdayModel()
                    {
                        HourStart = request.TimeSchedule!.Wednesday!.Start,
                        HourEnd = request.TimeSchedule!.Wednesday!.End,
                        IsClosed = request.TimeSchedule!.Wednesday!.IsClosed,
                    },
                    HotelThursday = new HotelThursdayModel()
                    {
                        HourStart = request.TimeSchedule!.Thursday!.Start,
                        HourEnd = request.TimeSchedule!.Thursday!.End,
                        IsClosed = request.TimeSchedule!.Thursday!.IsClosed,
                    },
                    HotelFriday = new HotelFridayModel()
                    {
                        HourStart = request.TimeSchedule!.Friday!.Start,
                        HourEnd = request.TimeSchedule!.Friday!.End,
                        IsClosed = request.TimeSchedule!.Friday!.IsClosed,
                    },
                    HotelSaturday = new HotelSaturdayModel()
                    {
                        HourStart = request.TimeSchedule!.Saturday!.Start,
                        HourEnd = request.TimeSchedule!.Saturday!.End,
                        IsClosed = request.TimeSchedule!.Saturday!.IsClosed,
                    },
                    HotelSunday = new HotelSundayModel()
                    {
                        HourStart = request.TimeSchedule!.Sunday!.Start,
                        HourEnd = request.TimeSchedule!.Sunday!.End,
                        IsClosed = request.TimeSchedule!.Sunday!.IsClosed,
                    },
                    AdminId = userGuid,
                    HotelName = request.HotelName,
                    HotelEmail = request.HotelEmail,
                    HotelDescription = request.HotelDescription,
                    HotelType = request.HotelType,
                    CheckInTime = request.CheckInTime,
                    CheckOutTime = request.CheckOutTime,
                    HotelWebsite = request.HotelWebsite,
                    HotelPhoneNo = request.HotelPhoneNo,
                    Location = request.Location,
                    State = request.State,
                    Country = request.Country,
                    Locality = request.Locality,
                   Postcode = request.Postcode,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    BookingAmount = request.BookingAmount,
                    Rating = 0,
                    NoOfRooms = request.NoOfRooms,
                    HotelAdvertType = "",
                    HotelLogo=request.HotelLogo,
                    Available=true,
                    CreatedAt = DateTime.Now,

                };
                // Pass detials to repository
                contacts = await hotelRepository.AddAsync(contacts);
                for (var i = 0; i < request.HotelImages!.Count; i++)
                {
                    rtnlist.Add(request.HotelImages[i]);
                    var imageUpdate = new HotelImages()
                    {
                        HotelDataTableId = contacts.Id,
                        ImagePath = rtnlist[i],
                    };
                    imageUpdate = await hotelRepository.AddHotelImageAsync(imageUpdate);
                    ticketBonus.Add(imageUpdate);
                }
                // convert back to dto
                var contact = await hotelRepository.GetAsync(contacts.Id);
                var contactsDto = mapper.Map<HotelDataModelDto>(contact);
                return CreatedAtAction(nameof(GetHotelAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }

        [HttpPut("update-hotel-time-schedule/{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateHotelTimeScheduleAsync([FromRoute] Guid id, AddTimeSchedule request)
        {
            var contacts = new HotelDataModel()
            {
                HotelMonday = new HotelMondayModel()
                {
                    HourStart = request.Monday!.Start,
                    HourEnd = request.Monday!.End,
                    IsClosed = request.Monday!.IsClosed,
                },
                HotelTuesday = new HotelTuesdayModel()
                {
                    HourStart = request.Tuesday!.Start,
                    HourEnd = request.Tuesday!.End,
                    IsClosed = request.Tuesday!.IsClosed,
                },
                HotelWednesday = new HotelWednesdayModel()
                {
                    HourStart = request.Wednesday!.Start,
                    HourEnd = request.Wednesday!.End,
                    IsClosed = request.Wednesday!.IsClosed,
                },
                HotelThursday = new HotelThursdayModel()
                {
                    HourStart = request.Thursday!.Start,
                    HourEnd = request.Thursday!.End,
                    IsClosed = request.Thursday!.IsClosed,
                },
                HotelFriday = new HotelFridayModel()
                {
                    HourStart = request.Friday!.Start,
                    HourEnd = request.Friday!.End,
                    IsClosed = request.Friday!.IsClosed,
                },
                HotelSaturday = new HotelSaturdayModel()
                {
                    HourStart = request.Saturday!.Start,
                    HourEnd = request.Saturday!.End,
                    IsClosed = request.Saturday!.IsClosed,
                },
                HotelSunday = new HotelSundayModel()
                {
                    HourStart = request.Sunday!.Start,
                    HourEnd = request.Sunday!.End,
                    IsClosed = request.Sunday!.IsClosed,
                },
            };

            // Update detials to repository
            contacts = await hotelRepository.UpdateTimeSheduleAsync(id, contacts);

            // check the null value
            if (contacts == null)
            {
                return BadRequest("Brand Does not Exist");
            }
            // convert back to dto
            else
            {
                var userDto = new ResponseModel()
                {
                    Message = "Updated Successfully",
                    Status = true
                };
                return Ok(userDto);
            }
        }



        [HttpPut]
        [Route("update-hotel-images/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdateHotelImageAsync([FromRoute] Guid id, [FromForm] AddImageList request)
        {
            // Update detials to repository

            List<HotelImages> ticketBonus = [];
            var rtnlist = new List<string>();
            var events = await hotelRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Event Does not Exist");
            }
            // convert back to dto
            else
            {
                var upload = new ImageListUpload()
                {
                    Image = request.Image,
                };
                var img = await uploadRepository.SaveListImageAsync(upload);
                for (var i = 0; i < img.ImagePath!.Count; i++)
                {
                    rtnlist.Add(img.ImagePath[i]);
                    var imageUpdate = new HotelImages()
                    {
                        HotelDataTableId = events.Id,
                        ImagePath = rtnlist[i],
                    };
                    imageUpdate = await hotelRepository.AddHotelImageAsync(imageUpdate);
                    ticketBonus.Add(imageUpdate);
                }
                var ticketBonusDtos = mapper.Map<List<HotelImagesDto>>(ticketBonus);
                
                var userDto = new HotelDataModelDto()
                {
                    Id = events.Id,
                    HotelImages = ticketBonusDtos,

                };
                return Ok(userDto);
            }
        }

       
        [HttpPut]
        [Route("update-hotel-facilities/{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateHotelFacilitiesAsync([FromRoute] Guid id, AddHotelFacilities request)
        {

            var events = await hotelRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Hotel Does not Exist");
            }
            // convert back to dto
            else
            {
                var interested = new HotelFacilities()
                {
                    Facilities = request.Facilities,
                    HotelDataTableId = events.Id,
                };

               await hotelRepository.AddHotelFacilitiesAsync(interested);
                var userDto = new ResponseModel()
                {
                    Message = "Update Facilities",
                    Status = true,
                };

                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("update-hotel-review/{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateHotelReviewAsync([FromRoute] Guid id, [FromBody] AddHotelReview request)
        {
            var events = await hotelRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Hotel Does not Exist");
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
                var interested = new HotelReviewModel()
                {
                    UserName = $"{user.FirstName} {user.LastName}",
                    ProfileImage = user.ImagePath,
                    HotelDataTableId = events.Id,
                    UserId = user.Id,
                    ReviewMessage = request.ReviewMessage,
                    CreatedAt = DateTime.Now,
                };

                interested = await hotelRepository.AddHotelReviewAsync(interested);
                var userDto = new ResponseModel()
                {
                    Message = "Update Facilities",
                    Status = true,
                };

                return Ok(userDto);
            }
        }

        // Hotel Chat Message
        [HttpGet("all-hotel-chat-messages")]
        [Authorize]
        public async Task<IActionResult> GetAllHotelChatAsync()
        {
            List<HotelChatModelDto> data = [];
            var events = await hotelRepository.GetAllHotelChatAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var userDto = mapper.Map<List<HotelChatModelDto>>(events);
            for (var i = 0; i < userDto.Count; i++)
            {
                var sender = await generalUserRepository.GetAsync(userDto[i].SenderId);
                var receiver = await placesRepository.GetAsync(userDto[i].ReceiverId);
                userDto[i].SenderProfileImage = sender.ImagePath;
                userDto[i].SenderUserName = $"{sender.FirstName} {sender.LastName}";
                userDto[i].ReceiverProfileImage = receiver.PlaceLogo;
                userDto[i].ReceiverUserName = $"{receiver.PlaceName}";
                data.Add(userDto[i]);
            }
            var lid = data.OrderByDescending(x => x.CreatedAt).ToList();
            var userDto2 = mapper.Map<List<HotelChatModelDto>>(lid);
            return Ok(userDto2);
        }

        [HttpGet("hotel-chat-messages")]
        [Authorize]
        public async Task<IActionResult> GetHotelChatAsync([FromHeader] Guid senderId, [FromHeader] Guid receiverId)
        {
            var sender = await generalUserRepository.GetAsync(senderId);
            var receiver = await placesRepository.GetAsync(receiverId);
            if (sender == null)
            {
                var error = new ResponseModel()
                {
                    Message = "User Does not Exist",
                    Status = true,
                };
                return BadRequest(error);
            }
            else if (receiver == null)
            {
                var error = new ResponseModel()
                {
                    Message = "User Does not Exist",
                    Status = true,
                };
                return BadRequest(error);
            }
            else
            {
                List<HotelChatModelDto> data = [];

                var groupChatId = $"{sender.Id}-{receiver.Id}";
                var events = await hotelRepository.GetAllHotelChatAsync();
                var valid = events.Where(x => x.GroupChatId == groupChatId);
                events = [.. valid.OrderByDescending(x => x.CreatedAt)];
                var userDto = mapper.Map<List<HotelChatModelDto>>(events);
                for (var i = 0; i < userDto.Count; i++)
                {
                    userDto[i].SenderProfileImage = sender.ImagePath;
                    userDto[i].SenderUserName = $"{sender.FirstName} {sender.LastName}";
                    userDto[i].ReceiverProfileImage = receiver.PlaceLogo;
                    userDto[i].ReceiverUserName = $"{receiver.PlaceName}";
                    data.Add(userDto[i]);
                }
                var lid = data.OrderByDescending(x => x.CreatedAt).ToList();
                var userDto2 = mapper.Map<List<HotelChatModelDto>>(lid);
                return Ok(userDto2);
            }
        }

        [HttpPost]
        [Route("hotel-chat-messages")]
        [Authorize]
        public async Task<IActionResult> AddHotelChatAsync([FromHeader] Guid senderId, [FromHeader] Guid receiverId, [FromBody] AddChatMessage message)
        {
            var validate = ValidateChat(message);

            if (!validate)
            {
                return BadRequest("Product Already Saved");
            }
            else
            {
                var sender = await generalUserRepository.GetAsync(senderId);
                var receiver = await generalUserRepository.GetAsync(receiverId);
                if (sender == null)
                {
                    var error = new ResponseModel()
                    {
                        Message = "User Does not Exist",
                        Status = true,
                    };
                    return BadRequest(error);
                }
                else if (receiver == null)
                {
                    var error = new ResponseModel()
                    {
                        Message = "User Does not Exist",
                        Status = true,
                    };
                    return BadRequest(error);
                }
                else
                {
                    var interested = new HotelChatModel()
                    {
                        SenderId = sender.Id,
                        ReceiverId = receiver.Id,
                        Message = message.Message,
                        GroupChatId = $"{sender.Id}-{receiver.Id}",
                        MessageType = message.MessageType,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                    };

                    await hotelRepository.AddHotelChatAsync(interested);
                   
                    var userDto = new ResponseModel()
                    {
                        Message = "Success",
                        Status = true,
                    };

                    return Ok(userDto);
                }
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

        private static void SendEmailVerificationCode(string emailId, string activationcode, string username)
        {

            var fromMail = new MailAddress("office@cnticketstravels.com", "CN Ticket & Travels Ltd");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "Chukwudi21$";
            string subject = " Email Verification Token";
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

        private bool ValidatePlace(AddPlaces request)
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

        private bool ValidateHotel(AddHotel request)
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

        #endregion
    }
}
