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
using System.Net;
using System.Security.Claims;
using QRCoder;
using Genilog_WebApi.Key;
using Genilog_WebApi.Model.WalletModel;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Genilog_WebApi.Repository.UserRepo;
using Genilog_WebApi.Repository.AdminRepo;
using Genilog_WebApi.Repository.LogisticsRepo;

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(IHostEnvironment _env, IMapper mapper, IAccomodationRepository accomodationRepository
        , IGeneralUserRepository generalUserRepository, IUploadRepository uploadRepository, IAirlineRepository airlineRepository, IAdminRepository adminRepository) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IMapper mapper = mapper;
        private readonly IAccomodationRepository accomodationRepository = accomodationRepository;
        private readonly IGeneralUserRepository generalUserRepository = generalUserRepository;
        private readonly IUploadRepository uploadRepository = uploadRepository;
        private readonly IAirlineRepository airlineRepository = airlineRepository;
        private readonly IAdminRepository adminRepository = adminRepository;
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
                return BadRequest("Accomodation Does not Exist");
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
                    AirlineImages=request.AirlineImages?? userDto1.AirlineImages,
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
               
               
                var rtnlist = new List<string>();
                var contacts = new AirlineDataModel()
                {
                    AdminId = userGuid,
                    AirlineImages=request.AirlineImages,
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

                // convert back to dto
                var contact = await airlineRepository.GetAsync(contacts.Id);
                var contactsDto = mapper.Map<AirlineDataModelDto>(contact);
                return CreatedAtAction(nameof(GetAirlinesAsync), new { id = contactsDto.Id }, contactsDto);
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
                return BadRequest("Accomodation Does not Exist");
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
                    AdminId = flight.AdminId,
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
                   DepartureAirpot="",
                   ReturnAirpot= "",
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
        [Route("accomodation")]
        [Authorize]
        public async Task<IActionResult> GetAllAccomodationAsync([FromQuery] FilterLocationData data)
        {
            var events = await accomodationRepository.GetAllAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var allPosts = events.AsQueryable();

            if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<AccomodationDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.State))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State));
                var userDto = mapper.Map<List<AccomodationDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<AccomodationDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else
            {
                var userDto = mapper.Map<List<AccomodationDataModelDto>>(events);
                return Ok(userDto);
            }
        }

        [HttpGet]
        [Route("accomodation/{id:guid}")]
        [ActionName("GetAccomodationAsync")]
        [Authorize]
        public async Task<IActionResult> GetAccomodationAsync(Guid id)
        {
            var contacts = await accomodationRepository.GetAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {

                var contactsDto = mapper.Map<AccomodationDataModelDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("accomodation/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> DeleteAccomodationAsync(Guid id)
        {
            // Get the region from the database
            var user = await accomodationRepository.DeleteAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Accomodation Does not Exist");
            }

            else
            {
                var events = await accomodationRepository.GetAllBookAccomodationReservationAsync();
                events = events.Where(x => x.AccomodationId == user.Id);
                foreach (var item in events)
                {
                    await accomodationRepository.DeleteBookAccomodationReservationAsync(item.Id);
                }
                var userDto = mapper.Map<AccomodationDataModelDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("accomodation/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdateAccomodationAsync([FromRoute] Guid id, [FromBody] AddAccomodation request)
        {
            var userDto1 = await accomodationRepository.GetAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("Id Does not Exist");
            }

            // convert back to dto
            else
            {
                var user = new AccomodationDataModel()
                {
                    AccomodationName = !string.IsNullOrWhiteSpace(request.AccomodationName) ? request.AccomodationName : userDto1.AccomodationName,
                    AccomodationDescription = !string.IsNullOrWhiteSpace(request.AccomodationDescription) ? request.AccomodationDescription : userDto1.AccomodationDescription,
                    CheckInTime = !string.IsNullOrWhiteSpace(request.CheckInTime) ? request.CheckInTime : userDto1.CheckInTime,
                    CheckOutTime = !string.IsNullOrWhiteSpace(request.CheckOutTime) ? request.CheckOutTime : userDto1.CheckOutTime,
                    AccomodationWebsite = !string.IsNullOrWhiteSpace(request.AccomodationWebsite) ? request.AccomodationWebsite : userDto1.AccomodationWebsite,
                    AccomodationPhoneNo = !string.IsNullOrWhiteSpace(request.AccomodationPhoneNo) ? request.AccomodationPhoneNo : userDto1.AccomodationPhoneNo,
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
                    AccomodationAdvertType = !string.IsNullOrWhiteSpace(request.AccomodationAdvertType) ? request.AccomodationAdvertType : userDto1.AccomodationAdvertType,
                    AccomodationEmail = !string.IsNullOrWhiteSpace(request.AccomodationEmail) ? request.AccomodationEmail : userDto1.AccomodationEmail,
                    Available = (bool)(request.Available ?? userDto1.Available),
                    AccomodationFacilities=(request.AccomodationFacilities ?? userDto1.AccomodationFacilities),
                    AccomodationImages=(request.AccomodationImages ?? userDto1.AccomodationImages),
                    AccomodationLogo= !string.IsNullOrWhiteSpace(request.AccomodationLogo) ? request.AccomodationLogo : userDto1.AccomodationLogo,
                    AccomodationType= !string.IsNullOrWhiteSpace(request.AccomodationType) ? request.AccomodationType : userDto1.AccomodationType,
                    AccomodationMonday = new AccomodationMondayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Monday!.Start) ?
                        request.TimeSchedule?.Monday!.Start : userDto1.AccomodationMonday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Monday!.End) ?
                        request.TimeSchedule?.Monday!.End : userDto1.AccomodationMonday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Monday?.IsClosed ?? userDto1.AccomodationMonday!.IsClosed),
                    },
                    AccomodationTuesday = new AccomodationTuesdayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Tuesday!.Start) ?
                        request.TimeSchedule?.Tuesday!.Start : userDto1.AccomodationTuesday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Tuesday!.End) ?
                        request.TimeSchedule?.Tuesday!.End : userDto1.AccomodationTuesday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Tuesday?.IsClosed ?? userDto1.AccomodationTuesday!.IsClosed),
                    },
                    AccomodationWednesday = new AccomodationWednesdayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Wednesday!.Start) ?
                        request.TimeSchedule?.Wednesday!.Start : userDto1.AccomodationWednesday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Monday!.End) ?
                        request.TimeSchedule?.Wednesday!.End : userDto1.AccomodationWednesday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Wednesday?.IsClosed ?? userDto1.AccomodationWednesday!.IsClosed),
                    },
                    AccomodationThursday = new AccomodationThursdayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Thursday!.Start) ?
                        request.TimeSchedule?.Thursday!.Start : userDto1.AccomodationMonday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Thursday!.End) ?
                        request.TimeSchedule?.Thursday!.End : userDto1.AccomodationThursday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Thursday?.IsClosed ?? userDto1.AccomodationThursday!.IsClosed),
                    },
                    AccomodationFriday = new AccomodationFridayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Friday!.Start) ?
                        request.TimeSchedule?.Friday!.Start : userDto1.AccomodationFriday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Friday!.End) ?
                        request.TimeSchedule?.Friday!.End : userDto1.AccomodationFriday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Friday?.IsClosed ?? userDto1.AccomodationFriday!.IsClosed),
                    },
                    AccomodationSaturday = new AccomodationSaturdayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Saturday!.Start) ?
                        request.TimeSchedule?.Saturday!.Start : userDto1.AccomodationSaturday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Saturday!.End) ?
                        request.TimeSchedule?.Saturday!.End : userDto1.AccomodationSaturday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Saturday?.IsClosed ?? userDto1.AccomodationSaturday!.IsClosed),
                    },
                    AccomodationSunday = new AccomodationSundayModel()
                    {
                        HourStart = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Sunday!.Start) ?
                        request.TimeSchedule?.Sunday!.Start : userDto1.AccomodationSunday!.HourStart,
                        HourEnd = !string.IsNullOrWhiteSpace(request.TimeSchedule?.Sunday!.End) ?
                        request.TimeSchedule?.Sunday!.End : userDto1.AccomodationSunday!.HourEnd,
                        IsClosed = (request.TimeSchedule?.Sunday?.IsClosed ?? userDto1.AccomodationSunday!.IsClosed),
                    },
                };

                // Update detials to repository
                user = await accomodationRepository.UpdateAsync(id, user);


                var contacts = await accomodationRepository.GetAsync(user.Id);
                var userDto = mapper.Map<AccomodationDataModelDto>(contacts);
                return Ok(userDto);
            }
        }

        [HttpPost("accomodation")]
        [Authorize(Roles = "Manager,Admin,Super_Admin")]
        public async Task<IActionResult> AddAccomodationAsync([FromBody] AddAccomodation request)
        {
            var check = ValidateAccomodation(request);

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

                var admin = await adminRepository.GetAsync(userGuid);
                if (admin.AdminType == "Manager")
                {
                    var contacts = new AccomodationDataModel()
                    {
                        AccomodationMonday = new AccomodationMondayModel()
                        {
                            HourStart = request.TimeSchedule!.Monday!.Start,
                            HourEnd = request.TimeSchedule!.Monday!.End,
                            IsClosed = request.TimeSchedule!.Monday!.IsClosed,
                        },
                        AccomodationTuesday = new AccomodationTuesdayModel()
                        {
                            HourStart = request.TimeSchedule!.Tuesday!.Start,
                            HourEnd = request.TimeSchedule!.Tuesday!.End,
                            IsClosed = request.TimeSchedule!.Tuesday!.IsClosed,
                        },
                        AccomodationWednesday = new AccomodationWednesdayModel()
                        {
                            HourStart = request.TimeSchedule!.Wednesday!.Start,
                            HourEnd = request.TimeSchedule!.Wednesday!.End,
                            IsClosed = request.TimeSchedule!.Wednesday!.IsClosed,
                        },
                        AccomodationThursday = new AccomodationThursdayModel()
                        {
                            HourStart = request.TimeSchedule!.Thursday!.Start,
                            HourEnd = request.TimeSchedule!.Thursday!.End,
                            IsClosed = request.TimeSchedule!.Thursday!.IsClosed,
                        },
                        AccomodationFriday = new AccomodationFridayModel()
                        {
                            HourStart = request.TimeSchedule!.Friday!.Start,
                            HourEnd = request.TimeSchedule!.Friday!.End,
                            IsClosed = request.TimeSchedule!.Friday!.IsClosed,
                        },
                        AccomodationSaturday = new AccomodationSaturdayModel()
                        {
                            HourStart = request.TimeSchedule!.Saturday!.Start,
                            HourEnd = request.TimeSchedule!.Saturday!.End,
                            IsClosed = request.TimeSchedule!.Saturday!.IsClosed,
                        },
                        AccomodationSunday = new AccomodationSundayModel()
                        {
                            HourStart = request.TimeSchedule!.Sunday!.Start,
                            HourEnd = request.TimeSchedule!.Sunday!.End,
                            IsClosed = request.TimeSchedule!.Sunday!.IsClosed,
                        },
                        AdminId = admin.ManagerId,
                        AccomodationName = request.AccomodationName,
                        AccomodationEmail = request.AccomodationEmail,
                        AccomodationDescription = request.AccomodationDescription,
                        AccomodationType = request.AccomodationType,
                        CheckInTime = request.CheckInTime,
                        CheckOutTime = request.CheckOutTime,
                        AccomodationWebsite = request.AccomodationWebsite,
                        AccomodationPhoneNo = request.AccomodationPhoneNo,
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
                        AccomodationAdvertType = "",
                        AccomodationLogo = request.AccomodationLogo,
                        Available = true,
                        CreatedAt = DateTime.Now,
                        AccomodationImages = request.AccomodationImages,
                        AccomodationFacilities = request.AccomodationFacilities,

                    };
                    // Pass detials to repository
                    var isExist = await accomodationRepository.AdminIdExistAsync(admin.ManagerId);
                    if (isExist)
                    {
                        contacts = await accomodationRepository.UpdateAsync(admin.ManagerId, contacts);
                        // convert back to dto
                        var contact = await accomodationRepository.GetAsync(contacts.Id);
                        var contactsDto = mapper.Map<AccomodationDataModelDto>(contact);
                        return CreatedAtAction(nameof(GetAccomodationAsync), new { id = contactsDto.Id }, contactsDto);
                    }
                    else
                    {
                        contacts = await accomodationRepository.AddAsync(contacts);
                        // convert back to dto
                        var contact = await accomodationRepository.GetAsync(contacts.Id);
                        var contactsDto = mapper.Map<AccomodationDataModelDto>(contact);
                        return CreatedAtAction(nameof(GetAccomodationAsync), new { id = contactsDto.Id }, contactsDto);
                    }
                }
                else
                {
                    var contacts = new AccomodationDataModel()
                    {
                        AccomodationMonday = new AccomodationMondayModel()
                        {
                            HourStart = request.TimeSchedule!.Monday!.Start,
                            HourEnd = request.TimeSchedule!.Monday!.End,
                            IsClosed = request.TimeSchedule!.Monday!.IsClosed,
                        },
                        AccomodationTuesday = new AccomodationTuesdayModel()
                        {
                            HourStart = request.TimeSchedule!.Tuesday!.Start,
                            HourEnd = request.TimeSchedule!.Tuesday!.End,
                            IsClosed = request.TimeSchedule!.Tuesday!.IsClosed,
                        },
                        AccomodationWednesday = new AccomodationWednesdayModel()
                        {
                            HourStart = request.TimeSchedule!.Wednesday!.Start,
                            HourEnd = request.TimeSchedule!.Wednesday!.End,
                            IsClosed = request.TimeSchedule!.Wednesday!.IsClosed,
                        },
                        AccomodationThursday = new AccomodationThursdayModel()
                        {
                            HourStart = request.TimeSchedule!.Thursday!.Start,
                            HourEnd = request.TimeSchedule!.Thursday!.End,
                            IsClosed = request.TimeSchedule!.Thursday!.IsClosed,
                        },
                        AccomodationFriday = new AccomodationFridayModel()
                        {
                            HourStart = request.TimeSchedule!.Friday!.Start,
                            HourEnd = request.TimeSchedule!.Friday!.End,
                            IsClosed = request.TimeSchedule!.Friday!.IsClosed,
                        },
                        AccomodationSaturday = new AccomodationSaturdayModel()
                        {
                            HourStart = request.TimeSchedule!.Saturday!.Start,
                            HourEnd = request.TimeSchedule!.Saturday!.End,
                            IsClosed = request.TimeSchedule!.Saturday!.IsClosed,
                        },
                        AccomodationSunday = new AccomodationSundayModel()
                        {
                            HourStart = request.TimeSchedule!.Sunday!.Start,
                            HourEnd = request.TimeSchedule!.Sunday!.End,
                            IsClosed = request.TimeSchedule!.Sunday!.IsClosed,
                        },
                        AdminId = request.ManagerId,
                        AccomodationName = request.AccomodationName,
                        AccomodationEmail = request.AccomodationEmail,
                        AccomodationDescription = request.AccomodationDescription,
                        AccomodationType = request.AccomodationType,
                        CheckInTime = request.CheckInTime,
                        CheckOutTime = request.CheckOutTime,
                        AccomodationWebsite = request.AccomodationWebsite,
                        AccomodationPhoneNo = request.AccomodationPhoneNo,
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
                        AccomodationAdvertType = "",
                        AccomodationLogo = request.AccomodationLogo,
                        Available = true,
                        CreatedAt = DateTime.Now,
                        AccomodationImages = request.AccomodationImages,
                        AccomodationFacilities = request.AccomodationFacilities,

                    };
                    // Pass detials to repository
                    var isExist = await accomodationRepository.AdminIdExistAsync(request.ManagerId);
                    if (isExist)
                    {
                        contacts = await accomodationRepository.UpdateAsync(request.ManagerId, contacts);
                        // convert back to dto
                        var contact = await accomodationRepository.GetAsync(contacts.Id);
                        var contactsDto = mapper.Map<AccomodationDataModelDto>(contact);
                        return CreatedAtAction(nameof(GetAccomodationAsync), new { id = contactsDto.Id }, contactsDto);
                    }
                    else
                    {
                        contacts = await accomodationRepository.AddAsync(contacts);
                        // convert back to dto
                        var contact = await accomodationRepository.GetAsync(contacts.Id);
                        var contactsDto = mapper.Map<AccomodationDataModelDto>(contact);
                        return CreatedAtAction(nameof(GetAccomodationAsync), new { id = contactsDto.Id }, contactsDto);
                    }
                }
               
              
            }
        }


        [HttpPut]
        [Route("update-accomodation-review/{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateAccomodationReviewAsync([FromRoute] Guid id, [FromBody] AddAccomodationReview request)
        {
            var events = await accomodationRepository.GetAsync(id);
            // check the null value
            if (events == null)
            {
                return BadRequest("Accomodation Does not Exist");
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
                var interested = new AccomodationReviewModel()
                {
                    UserName = $"{user.FirstName} {user.LastName}",
                    ProfileImage = user.ImagePath,
                    AccomodationDataTableId = events.Id,
                    UserId = user.Id,
                    ReviewMessage = request.ReviewMessage,
                    RatingNum=request.RatingNum,
                    CreatedAt = DateTime.Now,
                };

                interested = await accomodationRepository.AddAccomodationReviewAsync(interested);
                var userDto = new ResponseModel()
                {
                    Message = "Update Facilities",
                    Status = true,
                };

                return Ok(userDto);
            }
        }

        //Reservations Book
        [HttpGet]
        [Route("accomodation-reservations")]
        [Authorize]
        public async Task<IActionResult> GetAllBookAccomodationReservationAsync([FromQuery] FilterData data)
        {
            var events = await accomodationRepository.GetAllBookAccomodationReservationAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var allPosts = events.AsQueryable();

            if (!string.IsNullOrEmpty(data.AnyItem))
            {
                allPosts = allPosts.Where(x =>(x.AdminId!.ToString()==data.AnyItem || x.AccomodationId!.ToString() == data.AnyItem));
                var userDto = mapper.Map<List<BookAccomodationReservatioModelDto>>(allPosts);
                return Ok(userDto);
            }
           
            else
            {
                events = [.. events.OrderByDescending(x => x.CreatedAt)];
                var userDto = mapper.Map<List<BookAccomodationReservatioModelDto>>(events);
                return Ok(userDto);
            }
        }

        [HttpGet]
        [Route("accomodation-reservations/{id:guid}")]
        [ActionName("GetBookAccomodationReservationAsync")]
        [Authorize]
        public async Task<IActionResult> GetBookAccomodationReservationAsync(Guid id)
        {
            var contacts = await accomodationRepository.GetBookAccomodationReservationAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {

                var contactsDto = mapper.Map<BookAccomodationReservatioModelDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("accomodation-reservations/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> DeleteBookAccomodationReservationAsync(Guid id)
        {
            // Get the region from the database
            var user = await accomodationRepository.DeleteBookAccomodationReservationAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Accomodation Does not Exist");
            }

            else
            {
                var userDto = mapper.Map<BookAccomodationReservatioModelDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("accomodation-reservations/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdateBookAccomodationReservationAsync([FromRoute] Guid id, [FromBody] AddBookAccomodationReservation request)
        {
            var userDto1 = await accomodationRepository.GetBookAccomodationReservationAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("Id Does not Exist");
            }

            // convert back to dto
            else
            {
                var user = new BookAccomodationReservatioModel()
                {
                    RoomType = !string.IsNullOrWhiteSpace(request.RoomType) ? request.RoomType : userDto1.RoomType,
                    QRCode =  userDto1.QRCode,
                     RoomNumber = (request.RoomNumber ?? userDto1.RoomNumber),
                    RoomPrice = (double)(request.RoomPrice ?? userDto1.RoomPrice),
                    RoomFeatures = (request.RoomFeatures ?? userDto1.RoomFeatures),
                    IsBooked = (request.IsBooked ?? userDto1.IsBooked),
                    RoomImages = (request.RoomImages ?? userDto1.RoomImages),
                };

                // Update detials to repository
                user = await accomodationRepository.UpdateBookAccomodationReservationAsync(id, user);
                var contacts = await accomodationRepository.GetBookAccomodationReservationAsync(user.Id);
                var userDto = mapper.Map<BookAccomodationReservatioModelDto>(contacts);
                return Ok(userDto);
            }
        }


        [HttpPost("accomodation-reservations")]
        [Authorize(Roles = "Manager,Admin,Super_Admin,StaffAdmin,Staff")]
        public async Task<IActionResult> AddBookAccomodationReservationAsync([FromHeader] Guid accomodationId, [FromBody] AddBookAccomodationReservation request)
        {
            var check = ValidateBookAccomodationReservation(request);

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
                var events = await accomodationRepository.GetAsync(accomodationId);
                var dataInfo = Guid.NewGuid();
                var td = CreateRandomTokenSix();
                var cD1 = GenerateQRCode($"{td}{events.AccomodationName}{request.RoomNumber}");
                var contacts = new BookAccomodationReservatioModel()
                {
                    Id = dataInfo,
                    AdminId = events.AdminId,
                    AccomodationId = events.Id,
                    AccomodationName = events.AccomodationName,
                    AccomodationImage = events.AccomodationImages!.Count==0?"": events.AccomodationImages![0],
                    AccomodationType=events.AccomodationType,
                    IsBooked = false,
                    QRCode = cD1,
                    MaximumNoOfGuest = request.MaximumNoOfGuest,
                    AccomodationLocality =events.Locality,
                    AccomodationState =events.State,
                    RoomFeatures = request.RoomFeatures,
                    RoomImages = request.RoomImages,
                    RoomNumber = (int)request.RoomNumber!,
                    RoomPrice =(double) request.RoomPrice!,
                    RoomType = request.RoomType,
                    TicketNum = td,
                    CreatedAt = DateTime.Now,
                    UpdateddAt = DateTime.Now,
                };


                var exist = await accomodationRepository.BookAccomodationReservationRoomExistAsync(contacts.RoomNumber!, events.Id, contacts);
                if (exist != null)
                {
                    var contacted = await accomodationRepository.GetBookAccomodationReservationAsync(exist!.Id);
                    var contactsDto = mapper.Map<BookAccomodationReservatioModelDto>(contacted);
                    return CreatedAtAction(nameof(GetBookAccomodationReservationAsync), new { id = contactsDto.Id }, contactsDto);
                }
                else
                {
                    // Pass detials to repository
                    contacts = await accomodationRepository.AddBookAccomodationReservationAsync(contacts);
                    var datra = await accomodationRepository.GetBookAccomodationReservationAsync(contacts.Id);
                    var contactsDto = mapper.Map<BookAccomodationReservatioModelDto>(datra);
                    return CreatedAtAction(nameof(GetBookAccomodationReservationAsync), new { id = contactsDto.Id }, contactsDto);
                }
               
            }
        }

        //Customer Book
        [HttpGet]
        [Route("accomodation-reservations-customer")]
        [Authorize]
        public async Task<IActionResult> GetAllCustomerBookedReservationAsync([FromQuery] FilterData data)
        {
            var events = await accomodationRepository.GetAllCustomerBookedReservationAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var allPosts = events.AsQueryable();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid User ID format.");
            }
            var user = await generalUserRepository.GetAsync(userGuid);
            if (user.UserType == "Super_Admin" || user.UserType == "Admin")
            {
                if (!string.IsNullOrEmpty(data.AnyItem))
                {
                    allPosts = allPosts.Where(x => x.ResevationId!.ToString() == data.AnyItem || x.AccomodationId!.ToString() == data.AnyItem
                    );
                    var userDto = mapper.Map<List<CustomerBookedReservationDto>>(allPosts);

                    return Ok(userDto);
                }

                else
                {
                    var userDto = mapper.Map<List<CustomerBookedReservationDto>>(events);
                    return Ok(userDto);
                }
            }
            else if (user.UserType == "Manager" || user.UserType == "Staff_Admin"|| user.UserType=="Staff")
            {
                var admin = await adminRepository.GetAsync(user.Id);
                events = [.. events.Where(x => x.AdminId == admin.ManagerId)];
                var userDto = mapper.Map<List<CustomerBookedReservationDto>>(events);
                return Ok(userDto);
            }

            else
            {
                events = [.. events.Where(x => x.UserId == user.Id)];
                var userDto = mapper.Map<List<CustomerBookedReservationDto>>(events);
                return Ok(userDto);
            }
        }

        [HttpGet]
        [Route("accomodation-reservations-customer/{id:guid}")]
        [ActionName("GetCustomerBookedReservationAsync")]
        [Authorize]
        public async Task<IActionResult> GetCustomerBookedReservationAsync(Guid id)
        {
            var contacts = await accomodationRepository.GetCustomerBookedReservationAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {

                var contactsDto = mapper.Map<CustomerBookedReservationDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("accomodation-reservations-customer/{id:guid}")]
        [Authorize(Roles = "Admin,User,Super_Admin")]
        public async Task<IActionResult> DeleteCustomerBookedReservationAsync(Guid id)
        {
            // Get the region from the database
            var user = await accomodationRepository.DeleteCustomerBookedReservationAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Accomodation Does not Exist");
            }

            else
            {
                var userDto = mapper.Map<CustomerBookedReservationDto>(user);
                return Ok(userDto);
            }
        }

        [HttpGet("delete-accomodation-reservations-customer")]
        public async Task<IActionResult> DeleteLinkCustomerBookedReservationAsync([FromQuery] Guid orderId)
        {
            // Get the region from the database
            var user = await accomodationRepository.DeleteCustomerBookedReservationAsync(orderId);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Accomodation Does not Exist");
            }

            else
            {
              
                return Ok("Deleted Successfully");
            }
        }


        [HttpPut]
        [Route("accomodation-reservations-customer/{id:guid}")]
        [Authorize(Roles = "Admin,User,Super_Admin")]
        public async Task<IActionResult> UpdateCustomerBookedReservationAsync([FromRoute] Guid id, [FromBody] AddCustomerBookedReservation request)
        {
            var userDto1 = await accomodationRepository.GetCustomerBookedReservationAsync(id);
            if (userDto1 == null)
            {
                return BadRequest("Id Does not Exist");
            }

            // convert back to dto
            else
            {
                var user = new CustomerBookedReservation()
                {
                    CustomerName = !string.IsNullOrWhiteSpace(request.CustomerName) ? request.CustomerName : userDto1.CustomerName,
                    CustomerPhoneNumber = !string.IsNullOrWhiteSpace(request.CustomerPhoneNumber) ? request.CustomerPhoneNumber : userDto1.CustomerPhoneNumber,
                    CustomerEmail = !string.IsNullOrWhiteSpace(request.CustomerEmail) ? request.CustomerEmail : userDto1.CustomerEmail,
                    TrnxReference = !string.IsNullOrWhiteSpace(request.TrnxReference) ? request.TrnxReference : userDto1.TrnxReference,
                    PaymentChannel = !string.IsNullOrWhiteSpace(request.PaymentChannel) ? request.PaymentChannel : userDto1.PaymentChannel,
                    Comment = !string.IsNullOrWhiteSpace(request.Comment) ? request.Comment : userDto1.Comment,
                    NumberOfGuests = (request.NumberOfGuests ?? userDto1.NumberOfGuests),
                    PaymentStatus = (request.PaymentStatus ?? userDto1.PaymentStatus),
                    TicketClosed = (request.TicketClosed ?? userDto1.TicketClosed),
                    UpdateddAt=DateTime.Now,
                };

                // Update detials to repository
                user = await accomodationRepository.UpdateCustomerBookedReservationAsync(id, user);
                var contacts = await accomodationRepository.GetCustomerBookedReservationAsync(user.Id);
                var userDto = mapper.Map<CustomerBookedReservationDto>(contacts);
                return Ok(userDto);
            }
        }


        [HttpPost("accomodation-reservations-customer")]
        [Authorize]
        public async Task<IActionResult> AddCustomerBookedReservationAsync([FromHeader] Guid reservationId, [FromBody] AddCustomerBookedReservation request)
        {
            if (!DateTime.TryParse(request.ReservationStartDate, out var startDate) ||
                    !DateTime.TryParse(request.ReservationEndDate, out var endDate))
            {
                return BadRequest("Invalid date format.");
            }
            var vat = await accomodationRepository.GetAllCustomerBookedReservationAsync();
            var vat2 = vat.Where(x=>x.ResevationId == reservationId).ToList();
            var check = ValidateCustomerBookedReservation(request,startDate,endDate,vat2!);

            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {

                var userD = await generalUserRepository.GetAsync(request.UserId);
                if (userD == null)
                {
                    var events = await accomodationRepository.GetBookAccomodationReservationAsync(reservationId);
                    var acc = await accomodationRepository.GetAsync(events.AccomodationId);
                    var td = CreateRandomTokenSix();
                    var cD1 = GenerateQRCode($"\nTicketNum:{td} \nName:{request.CustomerName} \nRoom:{events.RoomNumber}");

                    var contacts = new CustomerBookedReservation()
                    {
                        UserId = Guid.NewGuid(),
                        AdminId = events.AdminId,
                        AccomodationId = events.AccomodationId,
                        AccomodationType = events.AccomodationType,
                        AccomodationImage = events.AccomodationImage,
                        AccomodationName = events.AccomodationName,
                        AccomodationLocation = acc.Location,
                        ResevationId = events.Id,
                        CustomerName = request.CustomerName,
                        CustomerEmail = request.CustomerEmail,
                        CustomerPhoneNumber = request.CustomerPhoneNumber,
                        QRCode = cD1,
                        Comment = request.Comment,
                        PaymentChannel = request.PaymentChannel,
                        NumberOfGuests = (int)request.NumberOfGuests!,
                        PaymentStatus = (bool)request.PaymentStatus!,
                        TicketClosed = false,
                        RoomNumber = events.RoomNumber,
                        TrnxReference = request.TrnxReference,
                        TicketNum = td,
                        ReservationEndDate = endDate,
                        ReservationStartDate = startDate,
                        NoOfDays = request.NoOfDays,
                        TotalCost = events.RoomPrice * request.NoOfDays,
                        CreatedAt = DateTime.Now,
                        UpdateddAt = DateTime.Now,
                    };
                    // Pass detials to repository
                    contacts = await accomodationRepository.AddCustomerBookedReservationAsync(contacts);
                    var userDto1 = await accomodationRepository.GetBookAccomodationReservationAsync(contacts.ResevationId);
                    var user = new BookAccomodationReservatioModel()
                    {
                        RoomType = userDto1.RoomType,
                        QRCode = userDto1.QRCode,
                        RoomNumber = (userDto1.RoomNumber),
                        RoomPrice = (userDto1.RoomPrice),
                        RoomFeatures = (userDto1.RoomFeatures),
                        IsBooked = true,
                        RoomImages = (userDto1.RoomImages),
                    };

                    // Update detials to repository
                    await accomodationRepository.UpdateBookAccomodationReservationAsync(contacts.ResevationId, user);
                    // convert back to dto
                    var contact = await accomodationRepository.GetCustomerBookedReservationAsync(contacts.Id);
                    var contactsDto = mapper.Map<CustomerBookedReservationDto>(contact);
                    return CreatedAtAction(nameof(GetCustomerBookedReservationAsync), new { id = contactsDto.Id }, contactsDto);
                }
                else
                {
                    var events = await accomodationRepository.GetBookAccomodationReservationAsync(reservationId);
                    var acc = await accomodationRepository.GetAsync(events.AccomodationId);
                    var td = CreateRandomTokenSix();
                    var cD1 = GenerateQRCode($"\nTicketNum:{td} \nName:{request.CustomerName} \nRoom:{events.RoomNumber}");

                    var contacts = new CustomerBookedReservation()
                    {
                        UserId = userD.Id,
                        AdminId = events.AdminId,
                        AccomodationId = events.AccomodationId,
                        AccomodationType = events.AccomodationType,
                        AccomodationImage = events.AccomodationImage,
                        AccomodationName = events.AccomodationName,
                        AccomodationLocation = acc.Location,
                        ResevationId = events.Id,
                        CustomerName = request.CustomerName,
                        CustomerEmail = request.CustomerEmail,
                        CustomerPhoneNumber = request.CustomerPhoneNumber,
                        QRCode = cD1,
                        Comment = request.Comment,
                        PaymentChannel = request.PaymentChannel,
                        NumberOfGuests = (int)request.NumberOfGuests!,
                        PaymentStatus = (bool)request.PaymentStatus!,
                        TicketClosed = false,
                        RoomNumber = events.RoomNumber,
                        TrnxReference = request.TrnxReference,
                        TicketNum = td,
                        ReservationEndDate = endDate,
                        ReservationStartDate = startDate,
                        NoOfDays = request.NoOfDays,
                        TotalCost = events.RoomPrice * request.NoOfDays,
                        CreatedAt = DateTime.Now,
                        UpdateddAt = DateTime.Now,
                    };
                    // Pass detials to repository
                    contacts = await accomodationRepository.AddCustomerBookedReservationAsync(contacts);
                    var userDto1 = await accomodationRepository.GetBookAccomodationReservationAsync(contacts.ResevationId);
                    var user = new BookAccomodationReservatioModel()
                    {
                        RoomType = userDto1.RoomType,
                        QRCode = userDto1.QRCode,
                        RoomNumber = (userDto1.RoomNumber),
                        RoomPrice = (userDto1.RoomPrice),
                        RoomFeatures = (userDto1.RoomFeatures),
                        IsBooked = true,
                        RoomImages = (userDto1.RoomImages),
                    };

                    // Update detials to repository
                    await accomodationRepository.UpdateBookAccomodationReservationAsync(contacts.ResevationId, user);
                    // convert back to dto
                    var contact = await accomodationRepository.GetCustomerBookedReservationAsync(contacts.Id);
                    var contactsDto = mapper.Map<CustomerBookedReservationDto>(contact);
                    return CreatedAtAction(nameof(GetCustomerBookedReservationAsync), new { id = contactsDto.Id }, contactsDto);
                }

                  
            }
        }


        //paystack
        [HttpPost("initialize-paystack-accomodation-reservations-customer")]
        [Authorize]
        public async Task<IActionResult> InitializePayment([FromHeader] Guid reservationId, [FromBody] AddCustomerBookedReservation request)

        {
            if (!DateTime.TryParse(request.ReservationStartDate, out var startDate) ||
                    !DateTime.TryParse(request.ReservationEndDate, out var endDate))
            {
                return BadRequest("Invalid date format.");
            }
            var vat = await accomodationRepository.GetAllCustomerBookedReservationAsync();
            var vat2 = vat.Where(x => x.ResevationId == reservationId).ToList();
            var check = ValidateCustomerBookedReservation(request, startDate, endDate, vat2!);

            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var userD = await generalUserRepository.GetAsync(request.UserId);

                if (userD == null)
                {
                    var events = await accomodationRepository.GetBookAccomodationReservationAsync(reservationId);
                    var acc = await accomodationRepository.GetAsync(events.AccomodationId);
                    var td = CreateRandomTokenSix();
                    var cD1 = GenerateQRCode($"\nTicketNum:{td} \nName:{request.CustomerName} \nRoom:{events.RoomNumber}");
                    if (events == null)
                    {
                        return BadRequest("Accommodation Does not Exist");
                    }
                    else
                    {
                        var contacts = new CustomerBookedReservation()
                        {
                            UserId = Guid.NewGuid(),
                            AdminId = events.AdminId,
                            AccomodationId = events.AccomodationId,
                            AccomodationType = events.AccomodationType,
                            AccomodationImage = events.AccomodationImage,
                            AccomodationName = events.AccomodationName,
                            AccomodationLocation = acc.Location,
                            ResevationId = events.Id,
                            CustomerName = request.CustomerName,
                            CustomerEmail = request.CustomerEmail,
                            CustomerPhoneNumber = request.CustomerPhoneNumber,
                            QRCode = cD1,
                            Comment = request.Comment,
                            PaymentChannel = "Paystack",
                            NumberOfGuests = (int)request.NumberOfGuests!,
                            PaymentStatus = false,
                            TicketClosed = false,
                            RoomNumber = events.RoomNumber,
                            TrnxReference = "",
                            TicketNum = td,
                            ReservationEndDate = endDate,
                            ReservationStartDate = startDate,
                            NoOfDays = request.NoOfDays,
                            TotalCost = events.RoomPrice * request.NoOfDays,
                            CreatedAt = DateTime.Now,
                            UpdateddAt = DateTime.Now,
                        };
                        contacts = await accomodationRepository.AddCustomerBookedReservationAsync(contacts);


                        var url = "https://api.paystack.co/transaction/initialize";
                        var data = new
                        {
                            email = request.CustomerEmail,
                            amount = (events.RoomPrice * request.NoOfDays) * 100,  // Amount in Kobo (100 kobo = 1 Naira)
                            callback_url = $"{Cls_Keys.ServerURL}/api/Bookings/verify-paystack-accomodation-reservations-customer?orderId={contacts.Id}", // The URL to redirect after payment
                            channels = new[] { "card", "bank", "ussd", "mobile_money", "bank_transfer" },
                            metadata = new
                            {
                                cancel_action = $"{Cls_Keys.ServerURL}/api/Bookings/delete-accomodation-reservations-customer?orderId={contacts.Id}"
                            }
                        };

                        using var httpClient = new HttpClient();

                        StringContent content = new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.PaystackSecretKey);
                        using var response = await httpClient.PostAsync($"{url}", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var paystackResponse = JsonConvert.DeserializeObject<PaystackResponse>(apiResponse);


                            return Ok(paystackResponse);
                        }
                        else
                        {
                            await accomodationRepository.DeleteCustomerBookedReservationAsync(contacts.Id);
                            var error = new ErrorModel()
                            {
                                Message = $"{apiResponse}",
                                Status = true
                            };
                            return BadRequest(error);
                        }

                    }
                }
                else
                {
                    var events = await accomodationRepository.GetBookAccomodationReservationAsync(reservationId);
                    var acc = await accomodationRepository.GetAsync(events.AccomodationId);
                    var td = CreateRandomTokenSix();
                    var cD1 = GenerateQRCode($"\nTicketNum:{td} \nName:{request.CustomerName} \nRoom:{events.RoomNumber}");
                    if (events == null)
                    {
                        return BadRequest("Accommodation Does not Exist");
                    }
                    else
                    {
                        var contacts = new CustomerBookedReservation()
                        {
                            UserId = userD.Id,
                            AdminId = events.AdminId,
                            AccomodationId = events.AccomodationId,
                            AccomodationType = events.AccomodationType,
                            AccomodationImage = events.AccomodationImage,
                            AccomodationName = events.AccomodationName,
                            AccomodationLocation = acc.Location,
                            ResevationId = events.Id,
                            CustomerName = request.CustomerName,
                            CustomerEmail = request.CustomerEmail,
                            CustomerPhoneNumber = request.CustomerPhoneNumber,
                            QRCode = cD1,
                            Comment = request.Comment,
                            PaymentChannel = "Paystack",
                            NumberOfGuests = (int)request.NumberOfGuests!,
                            PaymentStatus = false,
                            TicketClosed = false,
                            RoomNumber = events.RoomNumber,
                            TrnxReference = "",
                            TicketNum = td,
                            ReservationEndDate = endDate,
                            ReservationStartDate = startDate,
                            NoOfDays = request.NoOfDays,
                            TotalCost = events.RoomPrice * request.NoOfDays,
                            CreatedAt = DateTime.Now,
                            UpdateddAt = DateTime.Now,
                        };
                        contacts = await accomodationRepository.AddCustomerBookedReservationAsync(contacts);


                        var url = "https://api.paystack.co/transaction/initialize";
                        var data = new
                        {
                            email = request.CustomerEmail,
                            amount = (events.RoomPrice * request.NoOfDays) * 100,  // Amount in Kobo (100 kobo = 1 Naira)
                            callback_url = $"{Cls_Keys.ServerURL}/api/Bookings/verify-paystack-accomodation-reservations-customer?orderId={contacts.Id}", // The URL to redirect after payment
                            channels = new[] { "card", "bank", "ussd", "mobile_money", "bank_transfer" },
                            metadata = new
                            {
                                cancel_action = $"{Cls_Keys.ServerURL}/api/Bookings/delete-accomodation-reservations-customer?orderId={contacts.Id}"
                            }
                        };

                        using var httpClient = new HttpClient();

                        StringContent content = new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.PaystackSecretKey);
                        using var response = await httpClient.PostAsync($"{url}", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var paystackResponse = JsonConvert.DeserializeObject<PaystackResponse>(apiResponse);


                            return Ok(paystackResponse);
                        }
                        else
                        {
                            await accomodationRepository.DeleteCustomerBookedReservationAsync(contacts.Id);
                            var error = new ErrorModel()
                            {
                                Message = $"{apiResponse}",
                                Status = true
                            };
                            return BadRequest(error);
                        }

                    }
                }

                 
            }

        }

        [HttpGet("verify-paystack-accomodation-reservations-customer")]
        public async Task<IActionResult> VerifyPharmacyProductOrderPayment([FromQuery] Guid orderId,[FromQuery] string trxref, [FromQuery] string reference)
        {

            var url = $"https://api.paystack.co/transaction/verify/{reference}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.PaystackSecretKey);
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

                var tronData = await accomodationRepository.GetCustomerBookedReservationAsync(orderId);
                var user = new CustomerBookedReservation()
                {
                    CustomerName = tronData.CustomerName,
                    CustomerPhoneNumber = tronData.CustomerPhoneNumber,
                    CustomerEmail = tronData.CustomerEmail,
                    TrnxReference = paystackResponse.Reference,
                    PaymentChannel = tronData.PaymentChannel,
                    Comment = tronData.Comment,
                    NumberOfGuests = tronData.NumberOfGuests,
                    PaymentStatus =paystackResponse.Status,
                    TicketClosed = tronData.TicketClosed,
                    UpdateddAt = DateTime.Now,
                };

                // Update detials to repository
                user = await accomodationRepository.UpdateCustomerBookedReservationAsync(tronData.Id, user);

                var userDto1 = await accomodationRepository.GetBookAccomodationReservationAsync(tronData.ResevationId);
                var book = new BookAccomodationReservatioModel()
                {
                    RoomType = userDto1.RoomType,
                    QRCode = userDto1.QRCode,
                    RoomNumber = (userDto1.RoomNumber),
                    RoomPrice = (userDto1.RoomPrice),
                    RoomFeatures = (userDto1.RoomFeatures),
                    IsBooked = true,
                    RoomImages = (userDto1.RoomImages),
                };

                // Update detials to repository
                await accomodationRepository.UpdateBookAccomodationReservationAsync(tronData.ResevationId, book);
                return Ok(paystackResponse);
            }
            else
            {
                await accomodationRepository.DeleteCustomerBookedReservationAsync(orderId);
                var error = new ErrorModel()
                {
                    Message = $"{apiResponse}",
                    Status = true
                };
                return BadRequest(error);
            }
        }


        //flutterwave
        [HttpPost("initialize-flutterwave-accomodation-reservations-customer")]
        [Authorize]
        public async Task<IActionResult> InitializeFlutterwavePayment([FromHeader] Guid reservationId, [FromBody] AddCustomerBookedReservation request)
        {
            if (!DateTime.TryParse(request.ReservationStartDate, out var startDate) ||
                   !DateTime.TryParse(request.ReservationEndDate, out var endDate))
            {
                return BadRequest("Invalid date format.");
            }
            var vat = await accomodationRepository.GetAllCustomerBookedReservationAsync();
            var vat2 = vat.Where(x => x.ResevationId == reservationId).ToList();
            var check = ValidateCustomerBookedReservation(request, startDate, endDate, vat2!);

            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var userD = await generalUserRepository.GetAsync(request.UserId);

                if (userD == null)
                {
                    var events = await accomodationRepository.GetBookAccomodationReservationAsync(reservationId);
                    var acc = await accomodationRepository.GetAsync(events.AccomodationId);
                    var td = CreateRandomTokenSix();
                    var cD1 = GenerateQRCode($"\nTicketNum:{td} \nName:{request.CustomerName} \nRoom:{events.RoomNumber}");
                    if (acc == null)
                    {
                        return BadRequest("Accommodation Does not Exist");
                    }
                    else
                    {
                        var contacts = new CustomerBookedReservation()
                        {
                            UserId = Guid.NewGuid(),
                            AdminId = events.AdminId,
                            AccomodationId = events.AccomodationId,
                            AccomodationType = events.AccomodationType,
                            AccomodationImage = events.AccomodationImage,
                            AccomodationName = events.AccomodationName,
                            AccomodationLocation = acc.Location,
                            ResevationId = events.Id,
                            CustomerName = request.CustomerName,
                            CustomerEmail = request.CustomerEmail,
                            CustomerPhoneNumber = request.CustomerPhoneNumber,
                            QRCode = cD1,
                            Comment = request.Comment,
                            PaymentChannel = "Flutterwave",
                            NumberOfGuests = (int)request.NumberOfGuests!,
                            PaymentStatus = false,
                            TicketClosed = false,
                            RoomNumber = events.RoomNumber,
                            TrnxReference = "",
                            TicketNum = td,
                            ReservationEndDate = endDate,
                            ReservationStartDate = startDate,
                            NoOfDays = request.NoOfDays,
                            TotalCost = events.RoomPrice * request.NoOfDays,
                            CreatedAt = DateTime.Now,
                            UpdateddAt = DateTime.Now,
                        };
                        contacts = await accomodationRepository.AddCustomerBookedReservationAsync(contacts);


                        var url = "https://api.flutterwave.com/v3/payments";
                        var data = new
                        {
                            tx_ref = CreateRandomTokenSix(),
                            amount = events.RoomPrice * request.NoOfDays,  // Amount in Kobo (100 kobo = 1 Naira)
                            customer = new
                            {
                                email = request.CustomerEmail,
                                name = request.CustomerName
                            },
                            currency = "NGN",
                            redirect_url = $"{Cls_Keys.ServerURL}/api/Bookings/verify-flutterwave-accomodation-reservations-customer?orderId={contacts.Id}", // The URL to redirect after payment
                            customizations = new
                            {
                                title = "My App Payment",
                                description = "Payment for items in cart"
                            }
                        };


                        using var httpClient = new HttpClient();

                        StringContent content = new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.FlutterwaveSecretKey);
                        using var response = await httpClient.PostAsync($"{url}", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var paystackResponse = JsonConvert.DeserializeObject<FlutterwaveResponse>(apiResponse);
                            return Ok(paystackResponse);
                        }
                        else
                        {
                            await accomodationRepository.DeleteCustomerBookedReservationAsync(contacts.Id);
                            var error = new ErrorModel()
                            {
                                Message = $"{apiResponse}",
                                Status = true
                            };
                            return BadRequest(error);
                        }

                    }
                }
                else
                {
                    var events = await accomodationRepository.GetBookAccomodationReservationAsync(reservationId);
                    var acc = await accomodationRepository.GetAsync(events.AccomodationId);
                    var td = CreateRandomTokenSix();
                    var cD1 = GenerateQRCode($"\nTicketNum:{td} \nName:{request.CustomerName} \nRoom:{events.RoomNumber}");
                    if (acc == null)
                    {
                        return BadRequest("Accommodation Does not Exist");
                    }
                    else
                    {
                        var contacts = new CustomerBookedReservation()
                        {
                            UserId = userD.Id,
                            AdminId = events.AdminId,
                            AccomodationId = events.AccomodationId,
                            AccomodationType = events.AccomodationType,
                            AccomodationImage = events.AccomodationImage,
                            AccomodationName = events.AccomodationName,
                            AccomodationLocation = acc.Location,
                            ResevationId = events.Id,
                            CustomerName = request.CustomerName,
                            CustomerEmail = request.CustomerEmail,
                            CustomerPhoneNumber = request.CustomerPhoneNumber,
                            QRCode = cD1,
                            Comment = request.Comment,
                            PaymentChannel = "Flutterwave",
                            NumberOfGuests = (int)request.NumberOfGuests!,
                            PaymentStatus = false,
                            TicketClosed = false,
                            RoomNumber = events.RoomNumber,
                            TrnxReference = "",
                            TicketNum = td,
                            ReservationEndDate = endDate,
                            ReservationStartDate = startDate,
                            NoOfDays = request.NoOfDays,
                            TotalCost = events.RoomPrice * request.NoOfDays,
                            CreatedAt = DateTime.Now,
                            UpdateddAt = DateTime.Now,
                        };
                        contacts = await accomodationRepository.AddCustomerBookedReservationAsync(contacts);


                        var url = "https://api.flutterwave.com/v3/payments";
                        var data = new
                        {
                            tx_ref = CreateRandomTokenSix(),
                            amount = events.RoomPrice * request.NoOfDays,  // Amount in Kobo (100 kobo = 1 Naira)
                            customer = new
                            {
                                email = request.CustomerEmail,
                                name = request.CustomerName
                            },
                            currency = "NGN",
                            redirect_url = $"{Cls_Keys.ServerURL}/api/Bookings/verify-flutterwave-accomodation-reservations-customer?orderId={contacts.Id}", // The URL to redirect after payment
                            customizations = new
                            {
                                title = "My App Payment",
                                description = "Payment for items in cart"
                            }
                        };


                        using var httpClient = new HttpClient();

                        StringContent content = new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.FlutterwaveSecretKey);
                        using var response = await httpClient.PostAsync($"{url}", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var paystackResponse = JsonConvert.DeserializeObject<FlutterwaveResponse>(apiResponse);
                            return Ok(paystackResponse);
                        }
                        else
                        {
                            await accomodationRepository.DeleteCustomerBookedReservationAsync(contacts.Id);
                            var error = new ErrorModel()
                            {
                                Message = $"{apiResponse}",
                                Status = true
                            };
                            return BadRequest(error);
                        }

                    }
                }
              
            }

        }


        [HttpGet("verify-flutterwave-accomodation-reservations-customer")]
        public async Task<IActionResult> VerifyFlutterwavePayment([FromQuery] Guid orderId, [FromQuery] string status, [FromQuery] string tx_ref, [FromQuery] string transaction_id)
        {
            var url = $"https://api.flutterwave.com/v3/transactions/{transaction_id}/verify";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.FlutterwaveSecretKey);
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

                if (paystackResponse.PaymentStatus == "completed"|| paystackResponse.PaymentStatus== "successful")
                {

                    var tronData = await accomodationRepository.GetCustomerBookedReservationAsync(orderId);
                    var user = new CustomerBookedReservation()
                    {
                        CustomerName = tronData.CustomerName,
                        CustomerPhoneNumber = tronData.CustomerPhoneNumber,
                        CustomerEmail = tronData.CustomerEmail,
                        TrnxReference = paystackResponse.Reference,
                        PaymentChannel = tronData.PaymentChannel,
                        Comment = tronData.Comment,
                        NumberOfGuests = tronData.NumberOfGuests,
                        PaymentStatus = paystackResponse.Status,
                        TicketClosed = tronData.TicketClosed,
                        UpdateddAt = DateTime.Now,
                    };

                    // Update detials to repository
                    user = await accomodationRepository.UpdateCustomerBookedReservationAsync(tronData.Id, user);

                    var userDto1 = await accomodationRepository.GetBookAccomodationReservationAsync(tronData.ResevationId);
                    var book = new BookAccomodationReservatioModel()
                    {
                        RoomType = userDto1.RoomType,
                        QRCode = userDto1.QRCode,
                        RoomNumber = (userDto1.RoomNumber),
                        RoomPrice = (userDto1.RoomPrice),
                        RoomFeatures = (userDto1.RoomFeatures),
                        IsBooked = true,
                        RoomImages = (userDto1.RoomImages),
                    };

                    // Update detials to repository
                    await accomodationRepository.UpdateBookAccomodationReservationAsync(tronData.ResevationId, book);
                    // TODO: Mark payment as successful in DB
                    return Redirect($"{Cls_Keys.ServerURL}/api/flutterwave-redirect?status={paystackResponse.PaymentStatus}");
                }
                else
                {

                    await accomodationRepository.DeleteCustomerBookedReservationAsync(orderId);
                    return Redirect($"{Cls_Keys.ServerURL}/api/flutterwave-redirect?status={paystackResponse.PaymentStatus}");
                }

            }
            else
            {
                await accomodationRepository.DeleteCustomerBookedReservationAsync(orderId);
                var error = new ErrorModel()
                {
                    Message = $"{apiResponse}",
                    Status = true
                };
                return BadRequest(error);
            }
        }



        // Accomodation Chat Message
        [HttpGet("all-accomodation-chat-messages")]
        [Authorize]
        public async Task<IActionResult> GetAllAccomodationChatAsync()
        {
            List<AccomodationChatModelDto> data = [];
            var events = await accomodationRepository.GetAllAccomodationChatAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var userDto = mapper.Map<List<AccomodationChatModelDto>>(events);
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
            var userDto2 = mapper.Map<List<AccomodationChatModelDto>>(lid);
            return Ok(userDto2);
        }

        [HttpGet("accomodation-chat-messages")]
        [Authorize]
        public async Task<IActionResult> GetAccomodationChatAsync([FromHeader] Guid senderId, [FromHeader] Guid receiverId)
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
                List<AccomodationChatModelDto> data = [];

                var groupChatId = $"{sender.Id}-{receiver.Id}";
                var events = await accomodationRepository.GetAllAccomodationChatAsync();
                var valid = events.Where(x => x.GroupChatId == groupChatId);
                events = [.. valid.OrderByDescending(x => x.CreatedAt)];
                var userDto = mapper.Map<List<AccomodationChatModelDto>>(events);
                for (var i = 0; i < userDto.Count; i++)
                {
                    userDto[i].SenderProfileImage = sender.ImagePath;
                    userDto[i].SenderUserName = $"{sender.FirstName} {sender.LastName}";
                    userDto[i].ReceiverProfileImage = receiver.AirlineLogo;
                    userDto[i].ReceiverUserName = $"{receiver.AirlineName}";
                    data.Add(userDto[i]);
                }
                var lid = data.OrderByDescending(x => x.CreatedAt).ToList();
                var userDto2 = mapper.Map<List<AccomodationChatModelDto>>(lid);
                return Ok(userDto2);
            }
        }

        [HttpPost]
        [Route("accomodation-chat-messages")]
        [Authorize]
        public async Task<IActionResult> AddAccomodationChatAsync([FromHeader] Guid senderId, [FromHeader] Guid receiverId, [FromBody] AddChatMessage message)
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
                    var interested = new AccomodationChatModel()
                    {
                        SenderId = sender.Id,
                        ReceiverId = receiver.Id,
                        Message = message.Message,
                        GroupChatId = $"{sender.Id}-{receiver.Id}",
                        MessageType = message.MessageType,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                    };

                    await accomodationRepository.AddAccomodationChatAsync(interested);

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

        private bool ValidateAccomodation(AddAccomodation request)
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
        private bool ValidateBookAccomodationReservation(AddBookAccomodationReservation request)
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
        private bool ValidateCustomerBookedReservation(AddCustomerBookedReservation request, DateTime startDate, DateTime endDate, List<CustomerBookedReservation> existingReservations)
        {
            if (request == null)
            {
                ModelState.AddModelError(nameof(request), $"Add  Data Is Required");
                return false;
            }
            var userPhone = accomodationRepository.CustomerBookedReservationDateExistAsync(startDate,endDate,existingReservations);

            if (!userPhone)
            {
                ModelState.AddModelError($"Reservation Time",  $" From {request.ReservationStartDate} To {request.ReservationEndDate}  Already Booked");
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

        private static string GenerateQRCode(string text)
        {
            using QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using Base64QRCode qrCode = new(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            return qrCodeImage;  // Convert the image to base64
        }

        #endregion
    }
}
