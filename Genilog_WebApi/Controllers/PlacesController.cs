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
namespace Genilog_WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlacesController(IHostEnvironment _env, IMapper mapper, IHotelRepository hotelRepository
        , IGeneralUserRepository generalUserRepository, IUploadRepository uploadRepository, IPlacesRepository placesRepository) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IMapper mapper = mapper;
        private readonly IHotelRepository hotelRepository = hotelRepository;
        private readonly IGeneralUserRepository generalUserRepository = generalUserRepository;
        private readonly IUploadRepository uploadRepository = uploadRepository;
        private readonly IPlacesRepository placesRepository = placesRepository;
        readonly string keyPath = Path.Combine(_env.ContentRootPath, "Key\\ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2.json");


        // This Is PLACES SECTION
        [HttpGet]
       [Authorize]
        public async Task<IActionResult> GetAllPlacesAsync()
        {
            var contacts = await placesRepository.GetAllAsync();
            var contactsDto = mapper.Map<List<PlacesDataModelDto>>(contacts);
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
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> DeletePlacesAsync(Guid id)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
            // Get the region from the database
            var user = await placesRepository.DeleteAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Hotel Does not Exist");
            }

            else
            {
                DocumentReference usrRef = firestoreDb!.Collection("PlacesCollections").Document(user.Id.ToString());
                await usrRef.DeleteAsync();
                var userDto = mapper.Map<PlacesDataModelDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdatePlacesAsync([FromRoute] Guid id, [FromBody] UpdatePlaces request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                DocumentReference usrRef = firestoreDb!.Collection("PlacesCollections").Document(user.Id.ToString());
                Dictionary<string, object> user3 = new()
                {
                    {"PlaceName",user.PlaceName!},
                    {"PlaceEmail",user.PlaceEmail!},
                    {"CheckInTime",user.CheckInTime!},
                    {"CheckOutTime",user.CheckOutTime!},
                    {"PlaceWebsite",user.PlaceWebsite!},
                    {"PlacePhoneNo",user.PlacePhoneNo!},
                    {"Location",user.Location!},
                    {"Locality",user.Locality!},
                    {"Postcode",user.Postcode!},
                    {"Latitude",user.Latitude!},
                    {"Longitude",user.Longitude!},
                    {"Rating",user.Rating!},
                    {"BookingAmount",user.BookingAmount!},
                    {"PlaceAdvertType",user.PlaceAdvertType!},
                    {"PlaceOverview",user.PlaceOverview!},
                    {"PlacesAdditionalInfo",user.PlacesAdditionalInfo!},
                    {"CancelationPolicy",user.CancelationPolicy!},
                    {"PlacesHighlights",user.PlacesHighlights!},
                    {"PlaceLogo",user.PlaceLogo!},
                    {"Available",user.Available!},
                };
                await usrRef.UpdateAsync(user3);
                var contact = await placesRepository.GetAsync(user.Id);
                var userDto = mapper.Map<PlacesDataModelDto>(contact);
                return Ok(userDto);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> AddPlacesAsync( [FromBody] AddPlaces request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                DocumentReference usrRef = firestoreDb!.Collection("PlacesCollections").Document(contacts.Id.ToString());
                Dictionary<string, object> monday = new()
                {
                  {  "HourStart", contacts.PlacesMonday!.HourStart!},
                  {  "End", contacts.PlacesMonday!.HourEnd! },
                  { "IsClosed", contacts.PlacesMonday.IsClosed! },
                };
                Dictionary<string, object> tuesday = new()
                {
                  {  "HourStart", contacts.PlacesTuesday!.HourStart!},
                  {  "HourEnd", contacts.PlacesTuesday!.HourEnd! },
                  { "IsClosed", contacts.PlacesTuesday.IsClosed! },
                };
                Dictionary<string, object> wednesday = new()
                {
                  {  "HourStart", contacts.PlacesWednesday!.HourStart!},
                  {  "HourEnd", contacts.PlacesWednesday!.HourEnd! },
                  { "IsClosed", contacts.PlacesWednesday.IsClosed! },
                };
                Dictionary<string, object> thursday = new()
                {
                  {  "HourStart", contacts.PlacesThursday!.HourStart!},
                  {  "HourEnd", contacts.PlacesThursday!.HourEnd! },
                  { "IsClosed", contacts.PlacesThursday!.IsClosed! },
                };
                Dictionary<string, object> friday = new()
                {
                  {  "HourStart", contacts.PlacesFriday!.HourStart!},
                  {  "HourEnd", contacts.PlacesFriday!.HourEnd! },
                  { "IsClosed", contacts.PlacesFriday.IsClosed! },
                };
                Dictionary<string, object> saturday = new()
                {
                  {  "HourStart", contacts.PlacesSaturday!.HourStart!},
                  {  "HourEnd", contacts.PlacesSaturday!.HourEnd! },
                  { "IsClosed", contacts.PlacesSaturday.IsClosed! },
                };
                Dictionary<string, object> sunday = new()
                {
                  {  "HourStart", contacts.PlacesSunday!.HourStart!},
                  {  "HourEnd", contacts.PlacesSunday!.HourEnd!},
                  { "HourIsClosed", contacts.PlacesSunday.IsClosed! },
                };

                Dictionary<string, object> daysShedules = new()
                {
                  {  "Monday", monday},
                  {  "Tuesday", tuesday },
                  { "Wednesday", wednesday },
                  { "Thursday", thursday},
                  { "Friday", friday },
                  { "Saturday", saturday },
                  { "Sunday", sunday },
                };
                Dictionary<string, object> user3 = new()
                {
                    {"Id",contacts.Id.ToString()},
                    {"AdminId",contacts.AdminId.ToString()},
                    {"PlaceName" ,contacts.PlaceName!},
                    {"PlaceEmail",contacts.PlaceEmail! },
                    {"PlaceOverview",contacts.PlaceOverview!},
                    {"PlacesHighlights",contacts.PlacesHighlights!},
                    {"PlacesAdditionalInfo",contacts.PlacesAdditionalInfo!},
                    {"CancelationPolicy",contacts.CancelationPolicy! },
                    {"PlaceType",contacts.PlaceType!},
                    {"CheckInTime",contacts.CheckInTime!},
                    {"CheckOutTime",contacts.CheckOutTime!},
                    {"PlaceWebsite",contacts.PlaceWebsite!},
                    {"PlacePhoneNo",contacts.PlacePhoneNo!},
                    {"Location",contacts.Location!},
                    {"State",contacts.State!},
                    {"Country",contacts.Country!},
                    {"Locality",contacts.Locality!},
                    {"Postcode",contacts.Postcode!},
                    {"Latitude",contacts.Latitude!},
                    {"Longitude",contacts.Longitude!},
                    {"BookingAmountt",contacts.BookingAmount},
                    {"Rating",contacts.Rating},
                    {"IsPayment",contacts.IsPayment},
                    {"TicketType",contacts.TicketType!},
                    {"PlaceAdvertType",contacts.PlaceAdvertType!},
                    {"PlaceLogo",contacts.PlaceLogo!},
                    {"Available",contacts.Available!},
                    {"ListPlacesImages",rtnlist!},
                    {"TimeSchedules",daysShedules },
                    {"DatePublished",date!},
                    {"Timestamp",timeStamp!}

                };
                await usrRef.SetAsync(user3);
                // convert back to dto
                var contact = await placesRepository.GetAsync(contacts.Id);
                var contactsDto = mapper.Map<PlacesDataModelDto>(contact);
                return CreatedAtAction(nameof(GetPlacesAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }

        [HttpPut("update-places-time-schedule/{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdatePlacesTimeScheduleAsync([FromRoute] Guid id, AddTimeSchedule request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);

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
                DocumentReference usrRef = firestoreDb!.Collection("PlacesCollections").Document(contacts.Id.ToString());
                Dictionary<string, object> monday = new()
                {
                  {  "HourStart", contacts.PlacesMonday!.HourStart!},
                  {  "End", contacts.PlacesMonday!.HourEnd! },
                  { "IsClosed", contacts.PlacesMonday.IsClosed! },
                };
                Dictionary<string, object> tuesday = new()
                {
                  {  "HourStart", contacts.PlacesTuesday!.HourStart!},
                  {  "HourEnd", contacts.PlacesTuesday!.HourEnd! },
                  { "IsClosed", contacts.PlacesTuesday.IsClosed! },
                };
                Dictionary<string, object> wednesday = new()
                {
                  {  "HourStart", contacts.PlacesWednesday!.HourStart!},
                  {  "HourEnd", contacts.PlacesWednesday!.HourEnd! },
                  { "IsClosed", contacts.PlacesWednesday.IsClosed! },
                };
                Dictionary<string, object> thursday = new()
                {
                  {  "HourStart", contacts.PlacesThursday!.HourStart!},
                  {  "HourEnd", contacts.PlacesThursday!.HourEnd! },
                  { "IsClosed", contacts.PlacesThursday!.IsClosed! },
                };
                Dictionary<string, object> friday = new()
                {
                  {  "HourStart", contacts.PlacesFriday!.HourStart!},
                  {  "HourEnd", contacts.PlacesFriday!.HourEnd! },
                  { "IsClosed", contacts.PlacesFriday.IsClosed! },
                };
                Dictionary<string, object> saturday = new()
                {
                  {  "HourStart", contacts.PlacesSaturday!.HourStart!},
                  {  "HourEnd", contacts.PlacesSaturday!.HourEnd! },
                  { "IsClosed", contacts.PlacesSaturday.IsClosed! },
                };
                Dictionary<string, object> sunday = new()
                {
                  {  "HourStart", contacts.PlacesSunday!.HourStart!},
                  {  "HourEnd", contacts.PlacesSunday!.HourEnd!},
                  { "HourIsClosed", contacts.PlacesSunday.IsClosed! },
                };

                Dictionary<string, object> daysShedules = new()
                {
                  {  "Monday", monday},
                  {  "Tuesday", tuesday },
                  { "Wednesday", wednesday },
                  { "Thursday", thursday},
                  { "Friday", friday },
                  { "Saturday", saturday },
                  { "Sunday", sunday },
                };
                Dictionary<string, object> user3 = new()
                {
                    {"TimeSchedules",daysShedules },
                };
                await usrRef.UpdateAsync(user3);
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
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);

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
                DocumentReference usrRef = firestoreDb!.Collection("PlacesCollections").Document(events.Id.ToString());
                Dictionary<string, object> user3 = new()
                {
                    {"ListPlacesImages",rtnlist!},
                };
                await usrRef.UpdateAsync(user3);
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
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdatePlacesFacilitiesAsync([FromRoute] Guid id, AddPlacesFacilities request)
        {

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);

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

                interested = await placesRepository.AddPlaceFacilitiesAsync(interested);
                var date = DateTime.Now.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();
                DocumentReference usrRef = firestoreDb!.Collection("PlacesCollections").Document(events.Id.ToString()).Collection("Facilities")
                    .Document(interested.Id.ToString());
                Dictionary<string, object> user3 = new()
                    {
                      {"Id",interested.Id.ToString()!},
                      {"Facilities",interested.Facilities!},
                      {"PlacesDataModelId",interested.PlacesDataModelId.ToString()!},
                      {"DatePublished",date!},
                      {"Timestamp",timeStamp!}

                    };
                await usrRef.SetAsync(user3);
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
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdatePlaceWhatToExpectAsync([FromRoute] Guid id, AddPlaceWhatToExpect request)
        {

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);

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

                interested = await placesRepository.AddPlaceWhatToExpectAsync(interested);
                var date = DateTime.Now.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();
                DocumentReference usrRef = firestoreDb!.Collection("PlacesCollections").Document(events.Id.ToString()).Collection("PlaceWhatToExpect")
                    .Document(interested.Id.ToString());
                Dictionary<string, object> user3 = new()
                    {
                      {"Id",interested.Id.ToString()!},
                      {"Titles",interested.Titles !},
                      {"Description",interested.Description !},
                      {"SubTitle ",interested.SubTitle !},
                      {"PlacesDataModelId",interested.PlacesDataModelId.ToString()!},
                      {"DatePublished",date!},
                      {"Timestamp",timeStamp!}

                    };
                await usrRef.SetAsync(user3);
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

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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

                interested = await placesRepository.AddPlaceReviewAsync(interested);
                var date = DateTime.Now.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();
                DocumentReference usrRef = firestoreDb!.Collection("PlacesCollections").Document(events.Id.ToString()).Collection("Reviews")
                    .Document(interested.Id.ToString());
                Dictionary<string, object> user3 = new()
                    {
                      {"UserName",interested.UserName!},
                      {"ProfileImage",interested.ProfileImage!},
                      {"PlacesDataModelId",interested.PlacesDataModelId.ToString()!},
                      {"UserId",interested.UserId.ToString()!},
                      {"ReviewMessage",interested.ReviewMessage!},
                      {"DatePublished",date!},
                      {"Timestamp",timeStamp!}

                    };
                await usrRef.SetAsync(user3);
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
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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

                    interested = await placesRepository.AddPlacesChatAsync(interested);
                    var date = DateTime.Now.ToString("ddd,MMM d,yyyy");
                    var timeStamp = Timestamp.GetCurrentTimestamp();
                    DocumentReference usrRef = firestoreDb!.Collection("PlacesChatCollection")
                        .Document(interested.GroupChatId);

                    // Chat
                    DocumentReference documentReference = firestoreDb!.Collection("PlacesMessageCollections").Document(interested.GroupChatId).
                        Collection(interested.GroupChatId).Document(DateTime.Now.Millisecond.ToString());

                    Dictionary<string, object> user3 = new()
                    {
                         {"Id",interested.Id.ToString()!},
                         {"SenderId",interested.SenderId.ToString()!},
                         {"ReceiverId",interested.ReceiverId.ToString()!},
                         {"Message",interested.Message!},
                         {"GroupChatId",interested.GroupChatId!},
                         {"MessageType",interested.MessageType!},
                         {"IsRead",interested.IsRead!},
                         {"DatePublished",date},
                         {"Timestamp",timeStamp}

                    };
                    await usrRef.SetAsync(user3);
                    await firestoreDb.RunTransactionAsync(async transaction =>
                    {
                        DocumentSnapshot currentSnapshot = await transaction.GetSnapshotAsync(usrRef);
                        string id = currentSnapshot.GetValue<string>("Id");
                        string senderId = currentSnapshot.GetValue<string>("SenderId");
                        string receiverId = currentSnapshot.GetValue<string>("ReceiverId");
                        string message = currentSnapshot.GetValue<string>("Message");
                        string groupChatId = currentSnapshot.GetValue<string>("GroupChatId");
                        string messageType = currentSnapshot.GetValue<string>("MessageType");
                        string itemImageURL = currentSnapshot.GetValue<string>("ItemImageURL");
                        bool isRead = currentSnapshot.GetValue<bool>("IsRead");
                        string datePublished = currentSnapshot.GetValue<string>("DatePublished");
                        Timestamp timestamp = currentSnapshot.GetValue<Timestamp>("Timestamp");
                        Dictionary<string, object> transact = new()
                            {
                            {"Id",id},
                            {"SenderId",senderId},
                            {"ReceiverId",receiverId},
                            {"Message",message!},
                            {"GroupChatId",groupChatId},
                            {"MessageType",messageType},
                            {"ItemImageURL",itemImageURL!},
                            {"IsRead",isRead},
                            {"DatePublished",datePublished},
                            {"Timestamp",timeStamp}
                            };
                        transaction.Set(documentReference, transact);
                    });
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
            var contactsDto = mapper.Map<List<HotelDataTableDto>>(contacts);
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

                var contactsDto = mapper.Map<HotelDataTableDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("hotel/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> DeleteHotelAsync(Guid id)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
            // Get the region from the database
            var user = await hotelRepository.DeleteAsync(id);
            // if null NotFound
            if (user == null)
            {
                return BadRequest("Hotel Does not Exist");
            }

            else
            {
                DocumentReference usrRef = firestoreDb!.Collection("HotelCollections").Document(user.Id.ToString());
                await usrRef.DeleteAsync();
                var userDto = mapper.Map<HotelDataTableDto>(user);
                return Ok(userDto);
            }
        }

        [HttpPut]
        [Route("hotel/{id:guid}")]
        [Authorize(Roles = "Admin,Super_Admin")]
        public async Task<IActionResult> UpdateHotelAsync([FromRoute] Guid id, [FromBody] UpdateHotel request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                DocumentReference usrRef = firestoreDb!.Collection("HotelCollections").Document(user.Id.ToString());
                Dictionary<string, object> user3 = new()
                {
                    {"HotelName",user.HotelName!},
                    {"HotelDescription",user.HotelDescription!},
                    {"CheckInTime",user.CheckInTime!},
                    {"CheckOutTime",user.CheckOutTime!},
                    {"HotelWebsite",user.HotelWebsite!},
                    {"HotelPhoneNo",user.HotelPhoneNo!},
                    {"Location",user.Location!},
                    {"NoOfRooms",user.NoOfRooms!},
                    {"Locality",user.Locality!},
                    {"Postcode",user.Postcode!},
                    {"Latitude",user.Latitude!},
                    {"Longitude",user.Longitude!},
                    {"HotelLogo",user.HotelLogo!},
                    {"Available",user.Available!},
                    {"Rating",user.Rating!},
                    {"BookingAmount",user.BookingAmount!},
                    {"HotelAdvertType",user.HotelAdvertType!},
                };
                await usrRef.UpdateAsync(user3);
                var contacts = await hotelRepository.GetAsync(user.Id);
                var userDto = mapper.Map<HotelDataTableDto>(contacts);
                return Ok(userDto);
            }
        }

        [HttpPost("hotel")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> AddHotelAsync([FromBody] AddHotel request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                var date = DateTime.Now.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();
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
                DocumentReference usrRef = firestoreDb!.Collection("HotelCollections").Document(contacts.Id.ToString());
                Dictionary<string, object> monday = new()
                {
                  {  "HourStart", contacts.HotelMonday!.HourStart!},
                  {  "End", contacts.HotelMonday!.HourEnd! },
                  { "IsClosed", contacts.HotelMonday.IsClosed! },
                };
                Dictionary<string, object> tuesday = new()
                {
                  {  "HourStart", contacts.HotelTuesday!.HourStart!},
                  {  "HourEnd", contacts.HotelTuesday!.HourEnd! },
                  { "IsClosed", contacts.HotelTuesday.IsClosed! },
                };
                Dictionary<string, object> wednesday = new()
                {
                  {  "HourStart", contacts.HotelWednesday!.HourStart!},
                  {  "HourEnd", contacts.HotelWednesday!.HourEnd! },
                  { "IsClosed", contacts.HotelWednesday.IsClosed! },
                };
                Dictionary<string, object> thursday = new()
                {
                  {  "HourStart", contacts.HotelThursday!.HourStart!},
                  {  "HourEnd", contacts.HotelThursday!.HourEnd! },
                  { "IsClosed", contacts.HotelThursday!.IsClosed! },
                };
                Dictionary<string, object> friday = new()
                {
                  {  "HourStart", contacts.HotelFriday!.HourStart!},
                  {  "HourEnd", contacts.HotelFriday!.HourEnd! },
                  { "IsClosed", contacts.HotelFriday.IsClosed! },
                };
                Dictionary<string, object> saturday = new()
                {
                  {  "HourStart", contacts.HotelSaturday!.HourStart!},
                  {  "HourEnd", contacts.HotelSaturday!.HourEnd! },
                  { "IsClosed", contacts.HotelSaturday.IsClosed! },
                };
                Dictionary<string, object> sunday = new()
                {
                  {  "HourStart", contacts.HotelSunday!.HourStart!},
                  {  "HourEnd", contacts.HotelSunday!.HourEnd!},
                  { "HourIsClosed", contacts.HotelSunday.IsClosed! },
                };

                Dictionary<string, object> daysShedules = new()
                {
                  {  "Monday", monday},
                  {  "Tuesday", tuesday },
                  { "Wednesday", wednesday },
                  { "Thursday", thursday},
                  { "Friday", friday },
                  { "Saturday", saturday },
                  { "Sunday", sunday },
                };
                Dictionary<string, object> user3 = new()
                {
                    {"Id",contacts.Id.ToString()},
                    {"AdminId",contacts.AdminId.ToString()},
                    {"HotelName",contacts.HotelName!},
                    {"HotelEmail",contacts.HotelEmail!},
                    {"HotelDescription",contacts.HotelDescription!},
                    {"HotelType",contacts.HotelType!},
                    {"CheckInTime",contacts.CheckInTime!},
                    {"CheckOutTime",contacts.CheckOutTime!},
                    {"HotelWebsite",contacts.HotelWebsite!},
                    {"HotelPhoneNo",contacts.HotelPhoneNo!},
                    {"Location",contacts.Location!},
                    {"State",contacts.State!},
                    {"Country",contacts.Country!},
                    {"Locality",contacts.Locality!},
                    {"Postcode",contacts.Postcode!},
                    {"Latitude",contacts.Latitude!},
                    {"Longitude",contacts.Longitude!},
                    {"BookingAmountt",contacts.BookingAmount},
                    {"Rating",contacts.Rating},
                    {"NoOfRooms",contacts.NoOfRooms},
                    {"HotelAdvertType",contacts.HotelAdvertType!},
                    {"HotelLogo",contacts.HotelLogo!},
                    {"Available",contacts.Available!},
                    {"ListHotelImages",rtnlist!},
                    {"TimeSchedules",daysShedules },
                    {"DatePublished",date!},
                    {"Timestamp",timeStamp!}
                };
                await usrRef.SetAsync(user3);
                // convert back to dto
                var contact = await hotelRepository.GetAsync(contacts.Id);
                var contactsDto = mapper.Map<HotelDataTableDto>(contact);
                return CreatedAtAction(nameof(GetHotelAsync), new { id = contactsDto.Id }, contactsDto);
            }
        }

        [HttpPut("update-hotel-time-schedule/{id:guid}")]
        [Authorize(Roles = "User,Admin,Super_Admin")]
        public async Task<IActionResult> UpdateHotelTimeScheduleAsync([FromRoute] Guid id, AddTimeSchedule request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);

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
                DocumentReference usrRef = firestoreDb!.Collection("HotelCollections").Document(contacts.Id.ToString());
                Dictionary<string, object> monday = new()
                {
                  {  "HourStart", contacts.HotelMonday!.HourStart!},
                  {  "End", contacts.HotelMonday!.HourEnd! },
                  { "IsClosed", contacts.HotelMonday.IsClosed! },
                };
                Dictionary<string, object> tuesday = new()
                {
                  {  "HourStart", contacts.HotelTuesday!.HourStart!},
                  {  "HourEnd", contacts.HotelTuesday!.HourEnd! },
                  { "IsClosed", contacts.HotelTuesday.IsClosed! },
                };
                Dictionary<string, object> wednesday = new()
                {
                  {  "HourStart", contacts.HotelWednesday!.HourStart!},
                  {  "HourEnd", contacts.HotelWednesday!.HourEnd! },
                  { "IsClosed", contacts.HotelWednesday.IsClosed! },
                };
                Dictionary<string, object> thursday = new()
                {
                  {  "HourStart", contacts.HotelThursday!.HourStart!},
                  {  "HourEnd", contacts.HotelThursday!.HourEnd! },
                  { "IsClosed", contacts.HotelThursday!.IsClosed! },
                };
                Dictionary<string, object> friday = new()
                {
                  {  "HourStart", contacts.HotelFriday!.HourStart!},
                  {  "HourEnd", contacts.HotelFriday!.HourEnd! },
                  { "IsClosed", contacts.HotelFriday.IsClosed! },
                };
                Dictionary<string, object> saturday = new()
                {
                  {  "HourStart", contacts.HotelSaturday!.HourStart!},
                  {  "HourEnd", contacts.HotelSaturday!.HourEnd! },
                  { "IsClosed", contacts.HotelSaturday.IsClosed! },
                };
                Dictionary<string, object> sunday = new()
                {
                  {  "HourStart", contacts.HotelSunday!.HourStart!},
                  {  "HourEnd", contacts.HotelSunday!.HourEnd!},
                  { "HourIsClosed", contacts.HotelSunday.IsClosed! },
                };

                Dictionary<string, object> daysShedules = new()
                {
                  {  "Monday", monday},
                  {  "Tuesday", tuesday },
                  { "Wednesday", wednesday },
                  { "Thursday", thursday},
                  { "Friday", friday },
                  { "Saturday", saturday },
                  { "Sunday", sunday },
                };
                Dictionary<string, object> user3 = new()
                {
                    {"TimeSchedules",daysShedules },
                };
                await usrRef.UpdateAsync(user3);
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
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);

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
                DocumentReference usrRef = firestoreDb!.Collection("HotelCollections").Document(events.Id.ToString());
                Dictionary<string, object> user3 = new()
                {
                    {"ListHotelImages",rtnlist!},
                };
                await usrRef.UpdateAsync(user3);
                var userDto = new HotelDataTableDto()
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

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);

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

                interested = await hotelRepository.AddHotelFacilitiesAsync(interested);
                var date = DateTime.Now.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();
                DocumentReference usrRef = firestoreDb!.Collection("HotelCollections").Document(events.Id.ToString()).Collection("Facilities")
                    .Document(interested.Id.ToString());
                Dictionary<string, object> user3 = new()
                    {
                      {"Id",interested.Id.ToString()!},
                      {"Facilities",interested.Facilities!},
                      {"HotelDataTableId",interested.HotelDataTableId.ToString()!},
                      {"DatePublished",date!},
                      {"Timestamp",timeStamp!}

                    };
                await usrRef.SetAsync(user3);
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

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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
                var date = DateTime.Now.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();
                DocumentReference usrRef = firestoreDb!.Collection("HotelCollections").Document(events.Id.ToString()).Collection("Reviews")
                    .Document(interested.Id.ToString());
                Dictionary<string, object> user3 = new()
                    {
                      {"UserName",interested.UserName!},
                      {"ProfileImage",interested.ProfileImage!},
                      {"HotelDataTableId",interested.HotelDataTableId.ToString()!},
                      {"UserId",interested.UserId.ToString()!},
                      {"ReviewMessage",interested.ReviewMessage!},
                      {"DatePublished",date!},
                      {"Timestamp",timeStamp!}

                    };
                await usrRef.SetAsync(user3);
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
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
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

                    interested = await hotelRepository.AddHotelChatAsync(interested);
                    var date = DateTime.Now.ToString("ddd,MMM d,yyyy");
                    var timeStamp = Timestamp.GetCurrentTimestamp();
                    DocumentReference usrRef = firestoreDb!.Collection("HotelChatCollection")
                        .Document(interested.GroupChatId);

                    // Chat
                    DocumentReference documentReference = firestoreDb!.Collection("HotelMessageCollections").Document(interested.GroupChatId).
                        Collection(interested.GroupChatId).Document(DateTime.Now.Millisecond.ToString());

                    Dictionary<string, object> user3 = new()
                    {
                         {"Id",interested.Id.ToString()!},
                         {"SenderId",interested.SenderId.ToString()!},
                         {"ReceiverId",interested.ReceiverId.ToString()!},
                         {"Message",interested.Message!},
                         {"GroupChatId",interested.GroupChatId!},
                         {"MessageType",interested.MessageType!},
                         {"IsRead",interested.IsRead!},
                         {"DatePublished",date},
                         {"Timestamp",timeStamp}

                    };
                    await usrRef.SetAsync(user3);
                    await firestoreDb.RunTransactionAsync(async transaction =>
                    {
                        DocumentSnapshot currentSnapshot = await transaction.GetSnapshotAsync(usrRef);
                        string id = currentSnapshot.GetValue<string>("Id");
                        string senderId = currentSnapshot.GetValue<string>("SenderId");
                        string receiverId = currentSnapshot.GetValue<string>("ReceiverId");
                        string message = currentSnapshot.GetValue<string>("Message");
                        string groupChatId = currentSnapshot.GetValue<string>("GroupChatId");
                        string messageType = currentSnapshot.GetValue<string>("MessageType");
                        bool isRead = currentSnapshot.GetValue<bool>("IsRead");
                        string datePublished = currentSnapshot.GetValue<string>("DatePublished");
                        Timestamp timestamp = currentSnapshot.GetValue<Timestamp>("Timestamp");
                        Dictionary<string, object> transact = new()
                            {
                            {"Id",id},
                            {"SenderId",senderId},
                            {"ReceiverId",receiverId},
                            {"Message",message!},
                            {"GroupChatId",groupChatId},
                            {"MessageType",messageType},
                            {"IsRead",isRead},
                            {"DatePublished",datePublished},
                            {"Timestamp",timeStamp}
                            };
                        transaction.Set(documentReference, transact);
                    });
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
