using AutoMapper;
using Genilog_WebApi.Model.BookingsModel;
using Genilog_WebApi.Model.LogisticsModel;
using Genilog_WebApi.Model;
using Genilog_WebApi.Repository.AuthRepo;
using Genilog_WebApi.Repository.BookingsRepo;
using Genilog_WebApi.Repository.NotificationRepo;
using Genilog_WebApi.Repository.PlacesRepo;
using Genilog_WebApi.Repository.UploadRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(IHostEnvironment _env, IMapper mapper, IHotelRepository hotelRepository
        , IGeneralUserRepository generalUserRepository, IUploadRepository uploadRepository, IAirlineRepository airlineRepository
        , IHubContext<BookingsHubRepository> _hubContext, IHubContext<NotificationHub> _notificationHubContext) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IMapper mapper = mapper;
        private readonly IHotelRepository hotelRepository = hotelRepository;
        private readonly IGeneralUserRepository generalUserRepository = generalUserRepository;
        private readonly IUploadRepository uploadRepository = uploadRepository;
        private readonly IAirlineRepository airlineRepository = airlineRepository;
        private readonly IHubContext<BookingsHubRepository> _hubContext = _hubContext;
        private readonly IHubContext<NotificationHub> _notificationHubContext = _notificationHubContext;
        readonly string keyPath = Path.Combine(_env.ContentRootPath, "Key\\ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json");


        // This Is PLACES SECTION
        [HttpGet]
        [Route("airline")]
        [Authorize]
        public async Task<IActionResult> GetAllAirlinesAsync([FromQuery] FilterLocationData data)
        {
            var events = await airlineRepository.GetAllAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var allPosts = events.AsQueryable();

            if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<AirlineDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.State))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State));
                var userDto = mapper.Map<List<AirlineDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<AirlineDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else
            {
                var userDto = mapper.Map<List<AirlineDataModelDto>>(events);
                return Ok(userDto);
            }


        }

        [HttpGet]
        [Route("airline/{id:guid}")]
        [ActionName("GetAirlinesAsync")]
        [Authorize]
        public async Task<IActionResult> GetAirlinesAsync(Guid id)
        {
            var contacts = await airlineRepository.GetAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                var contactsDto = mapper.Map<AirlineDataModelDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("airline/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> DeletePlacesAsync(Guid id)
        {
            // Get the region from the database
            var user = await airlineRepository.DeleteAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Hotel Does not Exist");
            }

            else
            {
                var userDto = mapper.Map<AirlineDataModelDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("airline/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateAirlinesAsync([FromRoute] Guid id, [FromBody] UpdateAirlines request)
        {
            var userDto1 = await airlineRepository.GetAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("Id Does not Exist");
            }

            // convert back to dto
            else
            {
                var user = new AirlineDataModel()
                {
                    AirlineName = !string.IsNullOrWhiteSpace(request.AirlineName) ? request.AirlineName : userDto1.AirlineName,
                    AirlineLogo = !string.IsNullOrWhiteSpace(request.AirlineLogo) ? request.AirlineLogo : userDto1.AirlineLogo,
                    AirlineEmail = !string.IsNullOrWhiteSpace(request.AirlineEmail) ? request.AirlineEmail : userDto1.AirlineEmail,
                    AirlineWebsite = !string.IsNullOrWhiteSpace(request.AirlineWebsite) ? request.AirlineWebsite : userDto1.AirlineWebsite,
                    Locality = !string.IsNullOrWhiteSpace(request.Locality) ? request.Locality : userDto1.Locality,
                    Rating = userDto1.Rating,
                    BookingAmount = (double)(request.BookingAmount ?? userDto1.BookingAmount),
                    AirlineInfo = !string.IsNullOrWhiteSpace(request.AirlineInfo) ? request.AirlineInfo : userDto1.AirlineInfo,
                    AirlineType = !string.IsNullOrWhiteSpace(request.AirlineType) ? request.AirlineType : userDto1.AirlineType,
                    AirlinePhoneNo = !string.IsNullOrWhiteSpace(request.AirlinePhoneNo) ? request.AirlinePhoneNo : userDto1.AirlinePhoneNo,
                    State = !string.IsNullOrWhiteSpace(request.State) ? request.State : userDto1.State,
                    Country = !string.IsNullOrWhiteSpace(request.Country) ? request.Country : userDto1.Country,
                    Available = (bool)(request.Available ?? userDto1.Available),
                };
                // Update detials to repository
                user = await airlineRepository.UpdateAsync(id, user);
                var contact = await airlineRepository.GetAsync(user.Id);
                var userDto = mapper.Map<AirlineDataModelDto>(contact);
                return Ok(userDto);
            }
        }

        [HttpPost("airline")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> AddAirlinesAsync([FromBody] AddAirlines request)
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
               
                List<AirlineImages> ticketBonus = [];
                var rtnlist = new List<string>();
                var contacts = new AirlineDataModel()
                {
                    AdminId = userGuid,
                    AirlineName = request.AirlineName,
                    AirlineEmail = request.AirlineEmail,
                    AirlineInfo = request.AirlineInfo,
                    AirlineType = request.AirlineType,
                    AirlinePhoneNo = request.AirlinePhoneNo,
                    State = request.State,
                    Country = request.Country,
                    Locality = request.Locality,
                    BookingAmount = request.BookingAmount,
                    Rating = 0,
                    Available = true,
                    AirlineLogo = request.AirlineLogo,
                    AirlineWebsite = request.AirlineWebsite,
                    CreatedAt = DateTime.Now,

                };
                // Pass detials to repository
                contacts = await airlineRepository.AddAsync(contacts);
                for (var i = 0; i < request.AirlineImages!.Count; i++)
                {
                    rtnlist.Add(request.AirlineImages[i]);
                    var imageUpdate = new AirlineImages()
                    {
                        AirlineDataModelId = contacts.Id,
                        ImagePath = rtnlist[i],
                    };
                    imageUpdate = await airlineRepository.AddAirlineImageAsync(imageUpdate);
                    ticketBonus.Add(imageUpdate);
                }

                // convert back to dto
                var contact = await airlineRepository.GetAsync(contacts.Id);
                var contactsDto = mapper.Map<AirlineDataModelDto>(contact);
                return CreatedAtAction(nameof(GetAirlinesAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }


        [HttpPut]
        [Route("add-airline-images/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdateAirlinesImageAsync([FromRoute] Guid id, [FromBody] AddAirlinesImages request)
        {
            // Update detials to repository

            List<AirlineImages> ticketBonus = [];
            var rtnlist = new List<string>();
            var events = await airlineRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Event Does not Exist");
            }
            // convert back to dto
            else
            {

                for (var i = 0; i < request.AirlineImages!.Count; i++)
                {
                    rtnlist.Add(request.AirlineImages[i]);
                    var imageUpdate = new AirlineImages()
                    {
                        AirlineDataModelId = events.Id,
                        ImagePath = rtnlist[i],
                    };
                    imageUpdate = await airlineRepository.AddAirlineImageAsync(imageUpdate);
                    ticketBonus.Add(imageUpdate);
                }
                var ticketBonusDtos = mapper.Map<List<AirlineImagesDto>>(ticketBonus);

                var userDto = new AirlineDataModelDto()
                {
                    Id = events.Id,
                    AirlineImages = ticketBonusDtos,

                };
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("add-aircraft/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateAirlineAircraftAsync([FromRoute] Guid id, AddAirCraft request)
        {

            var events = await airlineRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Places Does not Exist");
            }
            // convert back to dto
            else
            {
                var interested = new AirCraftList()
                {
                    Model = request.Model,
                    Manufacturer = request.Manufacturer,
                    Capacity = request.Capacity,
                    AirlineDataModelId = events.Id,
                };

                await airlineRepository.AddAirCraftListAsync(interested);

                var userDto = new ResponseModel()
                {
                    Message = "Update Facilities",
                    Status = true,
                };

                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("add-airline-payment/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateAirlinePaymentAsync([FromRoute] Guid id, AddAirlinePayment request)
        {
            var events = await airlineRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Places Does not Exist");
            }
            // convert back to dto
            else
            {
                var interested = new AirlinePayment()
                {
                    Titles = request.Titles,
                    Amount = request.Amount,
                    AirlineDataModelId = events.Id,
                };

                await airlineRepository.AddAirlinePaymentAsync(interested);
                var userDto = new ResponseModel()
                {
                    Message = "Updated Successfully",
                    Status = true,
                };

                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("add-airline-service-location/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateAirlineServiceLocationAsync([FromRoute] Guid id, AddAirlinesServiceLocation request)
        {
            var events = await airlineRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Places Does not Exist");
            }
            // convert back to dto
            else
            {
                var interested = new AirLineServiceLocation()
                {
                    Name = request.Name,
                    City = request.City,
                    Code = request.Code,
                    Country = request.Country,
                    AirlineDataModelId = events.Id,
                };

                await airlineRepository.AddAirLineServiceLocationAsync(interested);
                var userDto = new ResponseModel()
                {
                    Message = "Updated Successfully",
                    Status = true,
                };

                return Ok(userDto);
            }
        }



        [HttpPut]
        [Route("add-airline-review/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateAirlineReviewAsync([FromRoute] Guid id, [FromBody] AddAirlinesReview request)
        {

            var events = await airlineRepository.GetAsync(id);
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
                var interested = new AirlineReviewModel()
                {
                    UserName = $"{user.FirstName} {user.LastName}",
                    ProfileImage = user.ImagePath,
                    AirlineDataModelId = events.Id,
                    UserId = user.Id,
                    ReviewMessage = request.ReviewMessage,
                    CreatedAt = DateTime.Now,
                };

                await airlineRepository.AddAirlineReviewAsync(interested);
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
        public async Task<IActionResult> GetAllAirlineChatAsync()
        {
            List<AirlineChatModelDto> data = [];
            var events = await airlineRepository.GetAllAirlineChatAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var userDto = mapper.Map<List<AirlineChatModelDto>>(events);
            for (var i = 0; i < userDto.Count; i++)
            {
                var sender = await generalUserRepository.GetAsync(userDto[i].SenderId);
                var receiver = await airlineRepository.GetAsync(userDto[i].ReceiverId);
                userDto[i].SenderProfileImage = sender.ImagePath;
                userDto[i].SenderUserName = $"{sender.FirstName} {sender.LastName}";
                userDto[i].ReceiverProfileImage = receiver.AirlineLogo;
                userDto[i].ReceiverUserName = $"{receiver.AirlineName}";
                data.Add(userDto[i]);
            }
            var lid = data.OrderByDescending(x => x.CreatedAt).ToList();
            var userDto2 = mapper.Map<List<AirlineChatModelDto>>(lid);
            return Ok(userDto2);
        }

        [HttpGet("places-chat-messages")]
        [Authorize]
        public async Task<IActionResult> GetAirlineChatAsync([FromHeader] Guid senderId, [FromHeader] Guid receiverId)
        {
            var sender = await generalUserRepository.GetAsync(senderId);
            var receiver = await airlineRepository.GetAsync(receiverId);
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
                List<AirlineChatModelDto> data = [];

                var groupChatId = $"{sender.Id}-{receiver.Id}";
                var events = await airlineRepository.GetAllAirlineChatAsync();
                var valid = events.Where(x => x.GroupChatId == groupChatId);
                events = [.. valid.OrderByDescending(x => x.CreatedAt)];
                var userDto = mapper.Map<List<AirlineChatModelDto>>(events);
                for (var i = 0; i < userDto.Count; i++)
                {
                    userDto[i].SenderProfileImage = sender.ImagePath;
                    userDto[i].SenderUserName = $"{sender.FirstName} {sender.LastName}";
                    userDto[i].ReceiverProfileImage = receiver.AirlineLogo;
                    userDto[i].ReceiverUserName = $"{receiver.AirlineName}";
                    data.Add(userDto[i]);
                }
                var lid = data.OrderByDescending(x => x.CreatedAt).ToList();
                var userDto2 = mapper.Map<List<AirlineChatModelDto>>(lid);
                return Ok(userDto2);
            }
        }

        [HttpPost]
        [Route("places-chat-messages")]
        [Authorize]
        public async Task<IActionResult> AddAirlineChatAsync([FromHeader] Guid senderId, [FromHeader] Guid receiverId, [FromBody] AddChatMessage message)
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
                    var interested = new AirlineChatModel()
                    {
                        SenderId = sender.Id,
                        ReceiverId = receiver.Id,
                        Message = message.Message,
                        GroupChatId = $"{sender.Id}-{receiver.Id}",
                        MessageType = message.MessageType,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                    };

                    await airlineRepository.AddAirlineChatAsync(interested);
                    var userDto = new ResponseModel()
                    {
                        Message = "Success",
                        Status = true,
                    };

                    return Ok(userDto);
                }
            }
        }

        // Airline Ticket
        [HttpGet]
        [Route("airline-flight-ticket")]
        [Authorize]
        public async Task<IActionResult> GetAllAirlinesFlightTicketAsync([FromQuery] FilterLocationData data)
        {
            var events = await airlineRepository.GetAllAirlineFlightTicketAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var allPosts = events.AsQueryable();

           if (!string.IsNullOrEmpty(data.AnyItem))
            {
                allPosts = allPosts.Where(x => x.AirlineName!.Contains(data.AnyItem)||x.AirlineId.ToString()==data.AnyItem);
                var userDto = mapper.Map<List<FlightTicketBookModelDto>>(allPosts);

                return Ok(userDto);
           }
            
            else
            {
                var userDto = mapper.Map<List<FlightTicketBookModelDto>>(events);
                return Ok(userDto);
            }


        }

        [HttpDelete]
        [Route("airline-flight-ticket/{id:guid}")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> DeleteAirlinesFlightTicketsAsync(Guid id)
        {
            // Get the region from the database
            var user = await airlineRepository.DeleteAirlineFlightTicketAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Hotel Does not Exist");
            }

            else
            {
                var userDto = mapper.Map<FlightTicketBookModelDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("airline-flight-ticket/{id:guid}")]
        [Authorize(Roles = "User,Manager,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateAirlinesFlightTicketAsync([FromRoute] Guid id, [FromBody] AddFlightTicket request)
        {
            var userDto1 = await airlineRepository.GetAirlineFlightTicketAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("Id Does not Exist");
            }

            // convert back to dto
            else
            {
                var user = new FlightTicketBookModel()
                {
                   TicketType = !string.IsNullOrWhiteSpace(request.TicketType) ? request.TicketType : userDto1.TicketType,
                    AvailabeTimeInterval = !string.IsNullOrWhiteSpace(request.AvailabeTimeInterval) ? request.AvailabeTimeInterval : userDto1.AvailabeTimeInterval,
                    DapatureTime = !string.IsNullOrWhiteSpace(request.DapatureTime) ? request.DapatureTime : userDto1.DapatureTime,
                    TicketPrice = (request.TicketPrice ?? userDto1.TicketPrice),
                    FlightSpeed = !string.IsNullOrWhiteSpace(request.FlightSpeed) ? request.FlightSpeed : userDto1.FlightSpeed,
                    Dapature = !string.IsNullOrWhiteSpace(request.Dapature) ? request.Dapature : userDto1.Dapature,
                    Destination = !string.IsNullOrWhiteSpace(request.Destination) ? request.Destination : userDto1.Destination,
                    OperatedBy = !string.IsNullOrWhiteSpace(request.OperatedBy) ? request.OperatedBy : userDto1.OperatedBy,
                    Available = (request.Available ?? userDto1.Available),
                    IsReturn = (request.IsReturn ?? userDto1.IsReturn),
                    BigLuggageKg = (request.BigLuggageKg ?? userDto1.BigLuggageKg),
                    SmallLuggageKg = (request.SmallLuggageKg ?? userDto1.SmallLuggageKg),
                    Stops= (request.Stops ?? userDto1.Stops),
                    StopPlaces= (request.StopPlaces ?? userDto1.StopPlaces),
                    
                };
                // Update detials to repository
                user = await airlineRepository.UpdateAirlineFlightTicketAsync(id, user);
                var contact = await airlineRepository.GetAirlineFlightTicketAsync(user.Id);
                var userDto = mapper.Map<FlightTicketBookModelDto>(contact);
                return Ok(userDto);
            }
        }

        [HttpPost("airline-flight-ticket")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> AddFlightTickeAsync([FromBody] AddFlightTicket request)
        {
            var check = ValidateFlightTicket(request);


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
                var flight = await airlineRepository.GetAsync(request.AirlineId);
                var contacts = new FlightTicketBookModel()
                {
                    AdminId = userGuid,
                    AirlineId=flight.Id,
                    AirlineName = flight.AirlineName,
                    OperatedBy=request.OperatedBy,
                    SmallLuggageKg= request.SmallLuggageKg,
                   Dapature = request.Dapature,
                    DapatureTime = request.DapatureTime,
                    Destination = request.Destination,
                    AvailabeTimeInterval = request.AvailabeTimeInterval,
                    TicketNum = CreateRandomTokenSix(),
                    FlightSpeed = request.FlightSpeed,
                    TicketPrice = request.TicketPrice,
                  TicketType = request.TicketType,
                    Available = true,
                    BigLuggageKg = request.BigLuggageKg,
                   IsReturn = request.IsReturn,
                   StopPlaces = request.StopPlaces,
                   Stops = request.Stops,
                    CreatedAt = DateTime.Now,

                };
                // Pass detials to repository
                contacts = await airlineRepository.AddAirlineFlightTicketAsync(contacts);
              
                // convert back to dto
                var contact = await airlineRepository.GetAirlineFlightTicketAsync(contacts.Id);
                var contactsDto = mapper.Map<FlightTicketBookModelDto>(contact);
                return CreatedAtAction(nameof(GetAirlinesAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }


        // This Is HOTEL SECTION
        [HttpGet]
        [Route("hotel")]
        public async Task<IActionResult> GetAllHotelAsync([FromQuery] FilterLocationData data)
        {
            var events = await hotelRepository.GetAllAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var allPosts = events.AsQueryable();

            if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<HotelDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.State))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State));
                var userDto = mapper.Map<List<HotelDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<HotelDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else
            {
                var userDto = mapper.Map<List<HotelDataModelDto>>(events);
                return Ok(userDto);
            }
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
        public async Task<IActionResult> UpdateHotelAsync([FromRoute] Guid id, [FromBody] AddHotel request)
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
                    Rating = userDto1.Rating,
                    BookingAmount = (double)(request.BookingAmount ?? userDto1.BookingAmount),
                    HotelAdvertType = !string.IsNullOrWhiteSpace(request.HotelAdvertType) ? request.HotelAdvertType : userDto1.HotelAdvertType,
                    HotelEmail = !string.IsNullOrWhiteSpace(request.HotelEmail) ? request.HotelEmail : userDto1.HotelEmail,
                    Available = (bool)(request.Available ?? userDto1.Available),
                    HotelMonday = new HotelMondayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Monday!.Start) ?
                        request.TimeSchedule?.Monday!.Start : userDto1.HotelMonday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Monday!.End) ?
                        request.TimeSchedule?.Monday!.End : userDto1.HotelMonday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Monday?.IsClosed ?? userDto1.HotelMonday!.IsClosed),
                    },
                    HotelTuesday = new HotelTuesdayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Tuesday!.Start) ?
                        request.TimeSchedule?.Tuesday!.Start : userDto1.HotelTuesday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Tuesday!.End) ?
                        request.TimeSchedule?.Tuesday!.End : userDto1.HotelTuesday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Tuesday?.IsClosed ?? userDto1.HotelTuesday!.IsClosed),
                    },
                    HotelWednesday = new HotelWednesdayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Wednesday!.Start) ?
                        request.TimeSchedule?.Wednesday!.Start : userDto1.HotelWednesday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Monday!.End) ?
                        request.TimeSchedule?.Wednesday!.End : userDto1.HotelWednesday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Wednesday?.IsClosed ?? userDto1.HotelWednesday!.IsClosed),
                    },
                    HotelThursday = new HotelThursdayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Thursday!.Start) ?
                        request.TimeSchedule?.Thursday!.Start : userDto1.HotelMonday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Thursday!.End) ?
                        request.TimeSchedule?.Thursday!.End : userDto1.HotelThursday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Thursday?.IsClosed ?? userDto1.HotelThursday!.IsClosed),
                    },
                    HotelFriday = new HotelFridayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Friday!.Start) ?
                        request.TimeSchedule?.Friday!.Start : userDto1.HotelFriday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Friday!.End) ?
                        request.TimeSchedule?.Friday!.End : userDto1.HotelFriday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Friday?.IsClosed ?? userDto1.HotelFriday!.IsClosed),
                    },
                    HotelSaturday = new HotelSaturdayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Saturday!.Start) ?
                        request.TimeSchedule?.Saturday!.Start : userDto1.HotelSaturday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Saturday!.End) ?
                        request.TimeSchedule?.Saturday!.End : userDto1.HotelSaturday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Saturday?.IsClosed ?? userDto1.HotelSaturday!.IsClosed),
                    },
                    HotelSunday = new HotelSundayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Sunday!.Start) ?
                        request.TimeSchedule?.Sunday!.Start : userDto1.HotelSunday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Sunday!.End) ?
                        request.TimeSchedule?.Sunday!.End : userDto1.HotelSunday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Sunday?.IsClosed ?? userDto1.HotelSunday!.IsClosed),
                    },
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
                    Latitude = (double)request.Latitude!,
                    Longitude = (double)request.Longitude!,
                    BookingAmount = (double)request.BookingAmount!,
                    Rating = 0,
                    NoOfRooms = (int)request.NoOfRooms!,
                    HotelAdvertType = "",
                    HotelLogo = request.HotelLogo,
                    Available = true,
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


        [HttpPut]
        [Route("update-hotel-images/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdateHotelImageAsync([FromRoute] Guid id, [FromBody] AddHotelImages request)
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

                for (var i = 0; i < request.HotelImages!.Count; i++)
                {
                    rtnlist.Add(request.HotelImages[i]);
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
                var receiver = await airlineRepository.GetAsync(userDto[i].ReceiverId);
                userDto[i].SenderProfileImage = sender.ImagePath;
                userDto[i].SenderUserName = $"{sender.FirstName} {sender.LastName}";
                userDto[i].ReceiverProfileImage = receiver.AirlineLogo;
                userDto[i].ReceiverUserName = $"{receiver.AirlineName}";
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
            var receiver = await airlineRepository.GetAsync(receiverId);
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
                    userDto[i].ReceiverProfileImage = receiver.AirlineLogo;
                    userDto[i].ReceiverUserName = $"{receiver.AirlineName}";
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

        private bool ValidatePlace(AddAirlines request)
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


        private bool ValidateFlightTicket(AddFlightTicket request)
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

        #endregion
    }
}
