using Ginilog_AdminWeb.GlobalConst;
using Ginilog_AdminWeb.Models;
using Ginilog_AdminWeb.Models.BookingsModel;
using Ginilog_AdminWeb.Models.WalletModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Ginilog_AdminWeb.Controllers
{
    public class BookingsController : Controller
    {

        private string firstName = "";
        private string surName = "";
        private string email = "";
        private string sex = "";
        public string? imagePath = "";

        public async Task<string> UploadFile(IFormFile formFile, string token)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var multipartContent = new MultipartFormDataContent();

            // Copy the content of the IFormFile to a StreamContent
            using var fileStream = formFile.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

            // Add the file to the multipart content
            multipartContent.Add(fileContent, "file", formFile.FileName);

            // Send the request to the API endpoint
            using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}UploadFile/upload-web-server", multipartContent);
            string apiResponse = await response.Content.ReadAsStringAsync();

            // Deserialize the response to get the URL
            var responseObj = JsonConvert.DeserializeObject<dynamic>(apiResponse);
            string fileUrl = responseObj?.imageUrl!;
            return fileUrl!;
        }
        public async Task<LocationDataDetail>? LocationData(string id)
        {
            LocationDataDetail data = new();
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync($"https://maps.googleapis.com/maps/api/place/details/json?placeid={id}&key=AIzaSyCuU7j9XnHs31-I6NE7cz_SxOw3lzScFuo");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var googlePlaceResponse = JsonConvert.DeserializeObject<GooglePlaceDetailsResponse>(apiResponse)!;

                if (googlePlaceResponse?.Result != null)
                {
                    var place = googlePlaceResponse.Result;
                    string address = place.FormattedAddress!;
                    string postcode = "";
                    string country = "";
                    string state = "";
                    string city = "";

                    foreach (var component in place.AddressComponents!)
                    {
                        var types = component.Types;

                        if (types!.Contains("postal_code"))
                            postcode = component.LongName!;
                        if (types.Contains("country"))
                            country = component.LongName!;
                        if (types.Contains("administrative_area_level_1"))
                            state = component.LongName!;
                        if (types.Contains("locality"))
                            city = component.LongName!;
                        if (string.IsNullOrEmpty(city) && types.Contains("administrative_area_level_2"))
                            city = component.LongName!; // fallback
                    }

                    double lat = place.Geometry!.Location!.Lat;
                    double lng = place.Geometry.Location.Lng;

                    // Use address, city, state, etc. as needed
                    data = new LocationDataDetail()
                    {
                        Address = address,
                        Locality = city,
                        Postcode = postcode,
                        Country = country,
                        State = state,
                        Latitude = lat,
                        Longitude = lng,
                    };
                }
                return data!;
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }

        }
        public async Task<AdminModelTable>? Data()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                AdminModelTable users = new();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}profile");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        users = JsonConvert.DeserializeObject<AdminModelTable>(apiResponse)!;
                        firstName = users.FirstName!;
                        surName = users.SurName!;
                        email = users.Email!;
                        sex = users.Sex!;
                        return users!;
                    }
                    else
                    {
                        ViewBag.StatusCode = response.StatusCode;
                    }
                }
                return users;
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<List<AdminModelTable>>? ManagerDataList()
        {
            var token = HttpContext.Session.GetString("bt_token");
            if (token != null)
            {
                List<AdminModelTable> users = [];
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        users = JsonConvert.DeserializeObject<List<AdminModelTable>>(apiResponse)!;
                        users=users.Where(x=>x.AdminType=="Manager" && x.CompanyType!.Contains("Accommodation")).ToList();
                        return users!;
                    }
                    else
                    {
                        ViewBag.StatusCode = response.StatusCode;
                    }
                }
                return users;
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<List<AccomodationDataModel>>? AccommodationItems()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                List<AccomodationDataModel>? reservationList = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<AccomodationDataModel>>(apiResponse);
                    reservationList = [.. reservationList!.OrderByDescending(x => x.CreatedAt)];
                    _ = reservationList.ToList();
                    return reservationList;
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<List<CustomerBookedReservation>>? CustomerReservationItems()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                List<CustomerBookedReservation>? reservationList = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations-customer");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<CustomerBookedReservation>>(apiResponse);
                    reservationList = [.. reservationList!.OrderByDescending(x => x.CreatedAt)];
                    _ = reservationList.ToList();
                    return reservationList;
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<List<BookAccomodationReservatioModel>>? BookAccomodationReservationItems()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                List<BookAccomodationReservatioModel>? reservationList = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<BookAccomodationReservatioModel>>(apiResponse);
                    reservationList = [.. reservationList!.OrderByDescending(x => x.CreatedAt)];
                    _ = reservationList.ToList();
                    return reservationList;
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }



        // Accommodation Booking
        [HttpGet]
        public async Task<IActionResult> AllAccommodationList(string id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = adminType;
                List<AccomodationDataModel> adminUser = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    adminUser = JsonConvert.DeserializeObject<List<AccomodationDataModel>>(apiResponse)!;
                    adminUser = [.. adminUser!.OrderByDescending(x => x!.CreatedAt)];
                    var search = from m in adminUser select m;
                    if (!String.IsNullOrEmpty(id))
                    {
                        search = search.Where(s => s.AccomodationName!.Contains(id, StringComparison.CurrentCultureIgnoreCase) || s.Location!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.Locality!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.State!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.Country!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        );
                        return View(search.ToList());
                    }
                    else
                    {
                        return View(adminUser);
                    }
                }
                else
                {
                    ViewBag.StatusCode = response.StatusCode;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AccommodationDetails(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var userDAta = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = userDAta.ImagePath!;
                ViewBag.AdminName = $"{userDAta.FirstName} {userDAta.SurName}";
                ViewBag.UseType = adminType;
                using var httpClient = new HttpClient();
                AccomodationDataModel users = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<AccomodationDataModel>(apiResponse)!;
                    var reservatioModels = BookAccomodationReservationItems()!.GetAwaiter().GetResult();
                    reservatioModels = reservatioModels.Where(x => x.AccomodationId == users.Id).ToList();

                    var customer = CustomerReservationItems()!.GetAwaiter().GetResult()!;
                    customer= customer.Where(x => x.AccomodationId == users.Id).ToList();

                    var details = new AllAccomodationData()
                    {
                       AccomodationDataModel = users,
                        BookAccomodationReservatioModel = reservatioModels,
                        CustomerBookedReservation = customer,
                    };
                    return View(details);
                }
                else
                {
                    ViewBag.StatusCode = response.StatusCode;
                    return RedirectToAction("AllAccommodationList", "Bookings");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }

        public IActionResult AddAccommodation()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            // var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = adminType;
              var managers=  ManagerDataList()!.GetAwaiter().GetResult();
              var manager = managers.Select(m => new SelectListItem
                {
                    Text = $"{m.FirstName} {m.SurName}",       // What the user sees
                    Value = m.Id.ToString()  // What gets submitted
                }).ToList();
                ViewBag.Managers = manager;
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }
        [HttpPost]
        public async Task<IActionResult> AddAccommodation(AddAccomodation requset)
        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {
                    var place = await LocationData(requset.Location!)!;
                    var logo = await UploadFile(requset.LogoUpload!, token!);
                    List<string> accomodationImages = [];   
                    for(int i = 0; i < requset.ImageList!.Count; i++)
                    {
                        var image = await UploadFile(requset.ImageList![i], token!);
                        accomodationImages.Add(image);
                    }
                    string? checkInTime = requset!.HourStart.HasValue ? DateTime.Today.Add(requset.HourStart.Value).ToString("hh:mm tt") : null;
                    string? checkOutTime = requset.HourEnd.HasValue ? DateTime.Today.Add(requset.HourEnd.Value).ToString("hh:mm tt") : null;

                    string? monHourStart = requset.TimeSchedule!.Monday!.HourStart.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Monday.HourStart.Value).ToString("hh:mm tt") : null;
                    string? monHourEnd = requset.TimeSchedule.Monday.HourEnd.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Monday.HourEnd.Value).ToString("hh:mm tt") : null;

                    string? tuesHourStart = requset.TimeSchedule!.Tuesday!.HourStart.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Tuesday.HourStart.Value).ToString("hh:mm tt") : null; ;
                    string? tuesHourEnd = requset.TimeSchedule.Tuesday.HourEnd.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Tuesday.HourEnd.Value).ToString("hh:mm tt") : null;

                    string? wedHourStart = requset.TimeSchedule!.Wednesday!.HourStart.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Wednesday.HourStart.Value).ToString("hh:mm tt") : null;
                    string? wedHourEnd = requset.TimeSchedule.Wednesday.HourEnd.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Wednesday.HourEnd.Value).ToString("hh:mm tt") : null;

                    string? thurHourStart = requset.TimeSchedule!.Thursday!.HourStart.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Thursday.HourStart.Value).ToString("hh:mm tt") : null;
                    string? thurHourEnd = requset.TimeSchedule.Thursday.HourEnd.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Thursday.HourEnd.Value).ToString("hh:mm tt") : null;

                    string? friHourStart = requset.TimeSchedule!.Friday!.HourStart.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Friday.HourStart.Value).ToString("hh:mm tt") : null;
                    string? friHourEnd = requset.TimeSchedule.Friday.HourEnd.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Friday.HourEnd.Value).ToString("hh:mm tt") : null;

                    string? satHourStart = requset.TimeSchedule!.Saturday!.HourStart.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Saturday.HourStart.Value).ToString("hh:mm tt") : null;
                    string? satHourEnd = requset.TimeSchedule.Saturday.HourEnd.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Saturday.HourEnd.Value).ToString("hh:mm tt") : null;

                    string? sunHourStart = requset.TimeSchedule!.Sunday!.HourStart.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Sunday.HourStart.Value).ToString("hh:mm tt") : null;
                    string? sunHourEnd = requset.TimeSchedule.Sunday.HourEnd.HasValue ? DateTime.Today.Add(requset.TimeSchedule.Sunday.HourEnd.Value).ToString("hh:mm tt") : null;
                    MainAccomodation login = new()
                    {
                        ManagerId=requset.ManagerId,
                        AccomodationName = requset.AccomodationName,
                        AccomodationEmail = requset.AccomodationEmail,
                        AccomodationPhoneNo = requset.AccomodationPhoneNo,
                        AccomodationDescription = requset.AccomodationDescription,
                        AccomodationType = requset.AccomodationType,
                        AccomodationAdvertType ="Normal",
                        Location = place.Address,
                        Country = place.Country,
                        Locality = place.Locality,
                        Postcode = place.Postcode,
                        State = place.State,
                        Latitude= place.Latitude,
                        Longitude = place.Longitude,
                        AccomodationWebsite = requset.AccomodationWebsite,
                        BookingAmount = requset.BookingAmount,
                        NoOfRooms = requset.NoOfRooms,
                        Available = true,
                        CheckInTime = checkInTime,
                        CheckOutTime = checkOutTime,
                        AccomodationFacilities = requset.Facilities!.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList(),
                        AccomodationImages = accomodationImages,
                        AccomodationLogo = logo,
                        TimeSchedule = new()
                        {
                            Monday = new()
                            {
                                IsClosed = requset.TimeSchedule!.Monday!.IsClosed,
                                Start = monHourStart,
                                End = monHourEnd
                            },
                            Tuesday = new()
                            {
                                IsClosed = requset.TimeSchedule!.Tuesday!.IsClosed,
                                Start = tuesHourStart,
                                End = tuesHourEnd
                            },
                            Wednesday = new()
                            {
                                IsClosed = requset.TimeSchedule!.Wednesday!.IsClosed,
                                Start = wedHourStart,
                                End = wedHourEnd
                            },
                            Thursday = new()
                            {
                                IsClosed = requset.TimeSchedule!.Thursday!.IsClosed,
                                Start = thurHourStart,
                                End = thurHourEnd
                            },
                            Friday = new()
                            {
                                IsClosed = requset.TimeSchedule!.Friday!.IsClosed,
                                Start = friHourStart,
                                End = friHourEnd
                            },
                            Saturday = new()
                            {
                                IsClosed = requset.TimeSchedule!.Saturday!.IsClosed,
                                Start = satHourStart,
                                End = satHourEnd
                            },
                            Sunday = new ()
                            {
                                IsClosed = requset.TimeSchedule!.Sunday!.IsClosed,
                                Start = sunHourStart,
                                End = sunHourEnd
                            }
                        }
                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created|| response.StatusCode== HttpStatusCode.OK)
                    {
                        var body = apiResponse!;
                        return RedirectToAction("AllAccommodationList", "Bookings");
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return View(requset);
                    }
                }
                catch
                {
                    return View(requset);
                }
            }
            return View(requset);
        }
        [HttpGet]
        public IActionResult DeleteAccommodation(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAccommodationConfirm(Guid id)
        {

            var token = HttpContext.Session.GetString("bt_token");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await httpClient.DeleteAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction("AllAccommodationList", "Bookings");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("AllAccommodationList", "Bookings");
            }
        }



        // Reservation

        [HttpGet]
        public async Task<IActionResult> AllAccomodationReservationList(string id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = adminType;
                List<BookAccomodationReservatioModel> adminUser = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    adminUser = JsonConvert.DeserializeObject<List<BookAccomodationReservatioModel>>(apiResponse)!;
                    adminUser = [.. adminUser!.OrderByDescending(x => x!.CreatedAt)];
                    var search = from m in adminUser select m;
                    if (!String.IsNullOrEmpty(id))
                    {
                        search = search.Where(s => s.AccomodationName!.Contains(id, StringComparison.CurrentCultureIgnoreCase) || s.TicketNum!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.AccomodationLocality!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.AccomodationType!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.AccomodationState!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.AccomodationId!.ToString() == id||s.AdminId!.ToString() == id
                        );
                        return View(search.ToList());
                    }
                    else
                    {
                        return View(adminUser);
                    }
                }
                else
                {
                    ViewBag.StatusCode = response.StatusCode;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AccomodationReservationDetails(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var userDAta = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = userDAta.ImagePath!;
                ViewBag.AdminName = $"{userDAta.FirstName} {userDAta.SurName}";
                ViewBag.UseType = adminType;
                using var httpClient = new HttpClient();
                BookAccomodationReservatioModel users = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<BookAccomodationReservatioModel>(apiResponse)!;
                  

                    var customer = CustomerReservationItems()!.GetAwaiter().GetResult()!;
                    customer = customer.Where(x => x.AccomodationId == users.Id).ToList();

                    var details = new AllBookedReservationModel()
                    {
                        BookAccomodationReservatioModel = users,
                        CustomerBookedReservation = customer,
                    };
                    return View(details);
                }
                else
                {
                    ViewBag.StatusCode = response.StatusCode;
                    return RedirectToAction("AllAccomodationReservationList", "Bookings");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }

        public IActionResult AddBookReservation(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            // var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = adminType;

                ViewBag.AccommodationId = id;
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }
        [HttpPost]
        public async Task<IActionResult> AddBookReservation(AddBookAccomodationReservation requset)
        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {

                    
                    List<string> roomImages = [];
                    for (int i = 0; i < requset.RoomImages!.Count; i++)
                    {
                        var image = await UploadFile(requset.RoomImages![i], token!);
                        roomImages.Add(image);
                    }
                   MainBookAccomodationReservation login = new()
                    {
                       RoomNumber = requset.RoomNumber,
                       MaximumNoOfGuest = requset.MaximumNoOfGuest,
                       RoomPrice = requset.RoomPrice,
                       RoomType = requset.RoomType,
                       IsBooked = true,
                       RoomFeatures = requset.RoomFeatures!.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList(),
                       RoomImages = roomImages,
                       

                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Add("accomodationId", requset.AccommodationId.ToString());
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                    {
                        var body = apiResponse!;
                        return RedirectToAction("AllAccomodationReservationList", "Bookings");
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return View(requset);
                    }
                }
                catch
                {
                    return View(requset);
                }
            }
            return View(requset);
        }

        [HttpGet]
        public IActionResult DeleteBookReservation(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBookReservationConfirm(Guid id)
        {

            var token = HttpContext.Session.GetString("bt_token");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await httpClient.DeleteAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction("AllAccomodationReservationList", "Bookings");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("AllAccomodationReservationList", "Bookings");
            }
        }

        // Customer
        [HttpGet]
        public IActionResult CustomerReservationList(string id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = adminType;
                // orders

                var orders = CustomerReservationItems()!.GetAwaiter().GetResult();
                orders = [.. orders!.OrderByDescending(x => x!.CreatedAt)];

                var search = from m in orders select m;


                if (!String.IsNullOrEmpty(id))
                {
                    search = search.Where(s => s.CustomerEmail!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                    || s.CustomerName!.Contains(id, StringComparison.CurrentCultureIgnoreCase) || s.TrnxReference!.Contains(id, StringComparison.CurrentCultureIgnoreCase) ||
                    s.AccomodationName!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                    || s.TicketNum!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                    || s.AccomodationId.ToString() == id || s.ResevationId.ToString() == id || s.UserId.ToString() == id
                    );
                    var allOrdersData = search.ToList();
                    return View(allOrdersData);
                }
                else
                {
                    var allOrdersData =  search.ToList();
                    return View(allOrdersData);
                }

            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }

        [HttpGet]
        public async Task<IActionResult> CustomerReservationDetails(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var userDAta = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = userDAta.ImagePath!;
                ViewBag.AdminName = $"{userDAta.FirstName} {userDAta.SurName}";
                ViewBag.UseType = adminType;
                using var httpClient = new HttpClient();
                CustomerBookedReservation users = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations-customer/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<CustomerBookedReservation>(apiResponse)!;
                    return View(users);
                }
                else
                {
                    ViewBag.StatusCode = response.StatusCode;
                    return RedirectToAction("CustomerReservationList", "Home");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }

        [HttpGet]
        public IActionResult DeleteCustomerReservation(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCustomerReservationConfirm(Guid id)
        {

            var token = HttpContext.Session.GetString("bt_token");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await httpClient.DeleteAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations-customer/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction("CustomerReservationList", "Bookings");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("CustomerReservationList", "Bookings");
            }
        }

        public IActionResult AddCustomerReservation(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            // var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = adminType;
                ViewBag.ReservationId = id;
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomerReservation(AddPaymentCustomerBookedReservation requset)
        {
            var token = HttpContext.Session.GetString("bt_token");
            var users = Data()!.GetAwaiter().GetResult();
            ViewBag.ReservationId = requset.ReservationId;
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime startDate = DateTime.Parse(requset.ReservationStartDate!);
                    DateTime endDate = DateTime.Parse(requset.ReservationEndDate!);
                    int totalDays = (endDate - startDate).Days;
                    AddPaymentCustomerBookedReservation login = new()
                    {
                        UserId=Guid.NewGuid(),
                        PaymentChannel = requset.PaymentChannel,
                        CustomerName = requset.CustomerName,
                        CustomerPhoneNumber = requset.CustomerPhoneNumber,
                        CustomerEmail = requset.CustomerEmail,
                        NumberOfGuests = requset.NumberOfGuests,
                        Comment = requset.Comment,
                        ReservationStartDate =requset.ReservationStartDate,
                        ReservationEndDate =requset.ReservationEndDate,
                        NoOfDays = totalDays,
                        PaymentStatus =  true ,
                        TrnxReference= requset.PaymentChannel == "Cash" ? CreateRandomTokenSix():"",
                        StaffId = users.Id,
                        StaffName = $"{users.FirstName} {users.SurName}",
                        PurchaseChannel = "Web Admin App",
                        UserType = "Not Registered",
                    };

                    using var httpClient = new HttpClient();
                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    httpClient.DefaultRequestHeaders.Add("reservationId", requset.ReservationId.ToString());

                    if (requset.PaymentChannel == "Paystack")
                    {
                        using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Bookings/initialize-paystack-accomodation-reservations-customer", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                        {
                            var body =  JsonConvert.DeserializeObject<PaystackResponse>(apiResponse)!;
                            return Redirect(body.Data!.AuthorizationUrl!);
                        }
                        else
                        {
                            ViewBag.UserError = apiResponse;
                            return View(requset);
                        }
                    }
                    else if(requset.PaymentChannel == "Flutterwave")
                    {
                        using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Bookings/initialize-flutterwave-accomodation-reservations-customer", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                        {
                            var body = JsonConvert.DeserializeObject<FlutterwaveResponse>(apiResponse)!;
                            return Redirect(body.Data!.Link!);
                        }
                        else
                        {
                            ViewBag.UserError = apiResponse;
                            return View(requset);
                        }
                    }
                    else 
                    {
                        using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations-customer", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                        {
                            var body = JsonConvert.DeserializeObject<CustomerBookedReservation>(apiResponse)!;
                            return RedirectToAction("CustomerReservationDetails", "Bookings", new { id = body.Id  });
                        }
                        else
                        {
                            ViewBag.UserError = apiResponse;
                            return View(requset);
                        }
                    }
                   
                }
                catch
                {
                    return View(requset);
                }
            }
            return View(requset);
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

    }
}
