using Ginilog_Company_WebDasboard.GlobalConst;
using Ginilog_Company_WebDasboard.Models;
using Ginilog_Company_WebDasboard.Models.BookingsModel;
using Ginilog_Company_WebDasboard.Models.LogisticsModel;
using Ginilog_Company_WebDasboard.Models.WalletModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Ginilog_Company_WebDasboard.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        ILogger<HomeController> logger = logger;
        private string firstName = "";
        private string surName = "";
        private string email = "";
        private string sex = "";
        public string? imagePath = "";
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
                    using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}profile/{userId}");
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

        public async Task<AdminModelTable>? ManagerData(Guid id)
        {
            var token = HttpContext.Session.GetString("bt_token");
            if (token != null)
            {
                AdminModelTable users = new();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}profile/{id}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        users = JsonConvert.DeserializeObject<AdminModelTable>(apiResponse)!; ;
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
    
        public async Task<CompanyModelData>? LogCompany(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                CompanyModelData users = new();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Logistics/{id}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        users = JsonConvert.DeserializeObject<CompanyModelData>(apiResponse)!;

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

        public async Task<AccomodationDataModel>? AccommodationItems(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                _ = new AccomodationDataModel();
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    AccomodationDataModel reservationList = JsonConvert.DeserializeObject<AccomodationDataModel>(apiResponse)!;
                    return reservationList;
                }
                else
                { 
                    ViewBag.StatusCode = response.StatusCode;
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<List<OrderModelData>>? PackageOrderItems()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                List<OrderModelData>? reservationList = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Logistics/package-orders");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<OrderModelData>>(apiResponse);
                    reservationList = [.. reservationList!.OrderByDescending(x => x.CreatedAt)];
                    _ = reservationList.ToList();
                    return reservationList;
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<OrderStatistics>? OrderStatistics()
        {
            var datass = await PackageOrderItems()!;
            DateTime localTime = DateTime.Now;
            TimeZoneInfo nigeriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            DateTime nigeriaTime = TimeZoneInfo.ConvertTimeFromUtc(localTime.ToUniversalTime(), nigeriaTimeZone);
            var userDto = datass.ToList();
            var totalRevenue = userDto.Sum(x => x.ShippingCost);
            var todaysOrder = userDto.Where(x => x.CreatedAt.ToString("yyyy-MM-dd") == nigeriaTime.ToString("yyyy-MM-dd")).ToList();
            var todaysAmountOrder = todaysOrder.Sum(x => x.ShippingCost);
            var completedOrders = userDto.Where(x => x.OrderStatus == "Completed" || x.OrderStatus == "Delivered" ||
            x.OrderStatus == "Closed").ToList();
            var completedAmountOrder = completedOrders.Sum(x => x.ShippingCost);
            var pendingOrders = userDto.Where(x => x.OrderStatus == "Picked" || x.OrderStatus == "Ongoing" ||
             x.OrderStatus == "Accepted").ToList();
            var pendingAmountOrder = pendingOrders.Sum(x => x.ShippingCost);
            var openOrders = userDto.Where(x => x.OrderStatus == "Open").ToList();
            var openAmountOrder = openOrders.Sum(x => x.ShippingCost);
            var rejectedOrders = userDto.Where(x => x.OrderStatus == "Rejected").ToList();
            var rejectedAmountOrder = rejectedOrders.Sum(x => x.ShippingCost);
            var totalVatAmount = userDto.Sum(x => (x.VatCost));
            var dataModel = new OrderStatistics()
            {
                TotalRevenue = totalRevenue,
                TotalOrders = userDto.Count,
                TotalAmountOrders = totalRevenue,
                TodaysOrder = todaysOrder.Count,
                TodayAmountOrders = todaysAmountOrder,
                CompletedOrders = completedOrders.Count,
                CompletedAmountOrders = completedAmountOrder,
                PendingOrders = pendingOrders.Count,
                PendingAmountOrders = pendingAmountOrder,
                OpenOrders = openOrders.Count,
                OpenAmountOrders = openAmountOrder,
                RejectedOrders = rejectedOrders.Count,
                RejectedAmountOrders = rejectedAmountOrder,
            };
            return dataModel;
        }

        public async Task<List<BookAccomodationReservatioModel>>? BookAccomodationReservationItems(string id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                List<BookAccomodationReservatioModel>? reservationList = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations?AnyItem={id}");
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

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
               
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.CompanyType = users.CompanyType;
                ViewBag.ManagerId = users.ManagerId;
                ViewBag.CompanyName = users.CompanyName;
              var  dataModel = new HomeModelData();
               
                if (users.CompanyType!.Contains("Logistics") && users.CompanyType!.Contains("Accommodation"))
                {

                    var logistics = LogCompany(users.ManagerId)!.GetAwaiter().GetResult();
                    ViewBag.LogisticId = logistics.Id;
                    var accommodation= AccommodationItems(users.ManagerId)!.GetAwaiter().GetResult(); 
                    ViewBag.AccommodationId = accommodation.Id;
                    if (logistics.CompanyName == null && accommodation.AccomodationName == null && users.AdminType=="Manager")
                    {
                        ViewBag.Logistics = "NotRegistered";
                        ViewBag.Accommodation = "NotRegistered";
                    }

                    var stats = OrderStatistics()!.GetAwaiter().GetResult();
                    var data = PackageOrderItems()!.GetAwaiter().GetResult();
                    data = [.. data.Where(x => x.OrderStatus == "Open" || x.OrderStatus == "Cancelled")];
                    var reservations = CustomerReservationItems()!.GetAwaiter().GetResult();
                    reservations = [.. reservations.Where(x => x.TicketClosed == false)];
                    dataModel = new HomeModelData()
                    {
                        OrderModelData = data,
                        OrderStatistics = stats,
                        CustomerBookedReservation = reservations,
                    };
                }
                else if (users.CompanyType!.Contains("Logistics"))
                {
                    var logistics = LogCompany(users.ManagerId)!.GetAwaiter().GetResult();
                    ViewBag.LogisticId = logistics.Id;
                    if (logistics.CompanyName == null  && users.AdminType == "Manager")
                    {
                        ViewBag.Logistics = "NotRegistered";
                    }
                    var stats = OrderStatistics()!.GetAwaiter().GetResult();
                    var data = PackageOrderItems()!.GetAwaiter().GetResult();
                    data = [.. data.Where(x => x.OrderStatus == "Open" || x.OrderStatus == "Cancelled")];
                    dataModel = new HomeModelData()
                    {
                        OrderModelData = data,
                        OrderStatistics = stats,
                    };
                }
                else
                {
                    var accommodation = AccommodationItems(users.ManagerId)!.GetAwaiter().GetResult();
                    ViewBag.AccommodationId = accommodation.Id;
                    if ( accommodation.AccomodationName == null && users.AdminType == "Manager")
                    {
                        ViewBag.Accommodation = "NotRegistered";
                    }
                    var reservations = CustomerReservationItems()!.GetAwaiter().GetResult();
                        reservations = [.. reservations.Where(x => x.TicketClosed == false)];
                        dataModel = new HomeModelData()
                        {
                            CustomerBookedReservation = reservations,
                        };
                    
                }

                    return View(dataModel);
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }


        // Staff Data
        [HttpGet]
        public async Task<IActionResult> AllStaffData(string id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.CompanyType = users.CompanyType;
                ViewBag.ManagerId = users.ManagerId;
                List<AdminModelTable> adminUser = [];

                if (users.AdminType== "Manager")
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}staff-users");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        adminUser = JsonConvert.DeserializeObject<List<AdminModelTable>>(apiResponse)!;
                        adminUser = [.. adminUser!.OrderByDescending(x => x!.CreatedAt)];
                        var search = from m in adminUser select m;
                        if (!String.IsNullOrEmpty(id))
                        {
                            search = search.Where(s => s.FirstName!.Contains(id, StringComparison.CurrentCultureIgnoreCase) || s.SurName!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                            || s.Email!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                            || s.CompanyName!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
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
                    return View(adminUser);

                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }
        }

        public IActionResult AddStaff()
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
                ViewBag.CompanyType = users.CompanyType;
                ViewBag.ManagerId = users.ManagerId;
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }
        [HttpPost]
        public async Task<IActionResult> AddStaff(AddStaffDataTable requset)
        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {
                    AddMainStaffDataTable login = new()
                    {
                        Email = requset.Email,
                        Password = requset.Password,
                        FirstName = requset.FirstName,
                        SurName = requset.SurName,
                        PhoneNo = requset.PhoneNo,
                        Sex = requset.Sex,
                        StaffCode = requset.StaffCode,
                        AdminType = requset.AdminType,
                        Address = requset.Address,
                        Locality = requset.Locality,
                        Branch = requset.Branch,
                        State = requset.State,
                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}staff-admin", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        var body = apiResponse;
                        return RedirectToAction("AllStaffData", "Home");
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
        public IActionResult DeleteStaff(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteStaffConfirm(Guid id)
        {

            var token = HttpContext.Session.GetString("bt_token");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await httpClient.DeleteAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}staff/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction("AllStaffData", "Home");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("AllStaffData", "Home");
            }
        }


        [HttpGet]
        public IActionResult StaffDetails(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.CompanyType = users.CompanyType;
                ViewBag.ManagerId = users.ManagerId;
                var managers = ManagerData(id)!.GetAwaiter().GetResult();
                return View(managers);
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LogCompanyDetails()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.CompanyType = users.CompanyType;
                ViewBag.Managers = users.ManagerId;
                using var httpClient = new HttpClient();
                CompanyModelData dataModel = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Logistics/{users.ManagerId}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dataModel = JsonConvert.DeserializeObject<CompanyModelData>(apiResponse)!;
                    // orders
                    var orders = PackageOrderItems()!.GetAwaiter().GetResult();
                    orders = [.. orders.Where(x => x.CompanyId == dataModel.Id)];
                    ViewBag.Id = dataModel.Id;

                    var details = new AllCompanyModelData()
                    {
                        CompanyModelData = dataModel,
                        OrderModelData = orders,
                    };
                    return View(details);
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
        public IActionResult AddLogCompany()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            // var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.Managers = users.ManagerId;
                ViewBag.CompanyNamme = users.CompanyName;
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }
        [HttpPost]
        public async Task<IActionResult> AddLogCompany(AddCompany requset)
        {
            var token = HttpContext.Session.GetString("bt_token");

            ViewBag.Managers = requset.ManagerId;
            if (ModelState.IsValid)
            {
                try
                {
                    var place = await LocationData(requset.CompanyAddress!)!;
                    var logo = await UploadFile(requset.LogoUpload!, token!);

                    AddMainCompany login = new()
                    {
                        ManagerId = requset.ManagerId,
                        CompanyEmail = requset.CompanyEmail,
                        CompanyName = requset.CompanyName,
                        PhoneNumber = requset.PhoneNumber,
                        CompanyRegNo = requset.CompanyRegNo,
                        CompanyInfo = requset.CompanyInfo,
                        Rating = 0,
                        ValueCharge = requset.ValueCharge,
                        NoOfTrucks = requset.NoOfTrucks,
                        NofOfBikes = requset.NofOfBikes,
                        AccountName = requset.AccountName,
                        AccountNumber = requset.AccountNumber,
                        BankName = requset.BankName,
                        Locality = place.Locality,
                        CompanyAddress = place.Address,
                        State = place.State,
                        Latitude = place.Latitude,
                        Longitude = place.Longitude,
                        PostCodes = place.Postcode,
                        DeliveryTypes = requset.DeliveryTypes,
                        ServiceAreas = [.. requset.ServiceArea!.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t))],
                        CompanyLogo = logo,
                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Logistics", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                    {
                        var body = JsonConvert.DeserializeObject<dynamic>(apiResponse)!;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return View(requset);
                    }
                }
                catch (Exception e)
                {
                    ViewBag.UserError = e.Message;
                    return View(requset);
                }
            }
            return View(requset);
        }

        // Orders Items
        [HttpGet]
        public IActionResult AddOrder()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                var company = LogCompany(users.ManagerId)!.GetAwaiter().GetResult(); 
                ViewBag.CompanyId = company.Id;
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOrder(AddOrder requset)
        {
            var token = HttpContext.Session.GetString("bt_token");
            ViewBag.CompanyId = requset.CompanyId;
            var users = Data()!.GetAwaiter().GetResult();
            if (ModelState.IsValid)
            {
                try
                {
                    var senderAddress = await LocationData(requset.SenderAddress!)!;
                    var recieverAddress = await LocationData(requset.RecieverAddress!)!;
                    List<string> packageImages = [];
                    for (int i = 0; i < requset.ImageList!.Count; i++)
                    {
                        var image = await UploadFile(requset.ImageList![i], token!);
                        packageImages.Add(image);
                    }
                    AddMainOrder login = new()
                    {
                        CompanyId = requset.CompanyId,
                        UserId = requset.UserId,
                        ExpectedDeliveryTime = requset.ExpectedDeliveryTime,
                        ShippingCost = requset.ShippingCost,
                        VatCost = requset.VatCost,
                        ItemName = requset.ItemName,
                        ItemDescription = requset.ItemDescription,
                        ItemModelNumber = requset.ItemModelNumber,
                        ItemCost = requset.ItemCost,
                        ItemQuantity = requset.ItemQuantity,
                        ItemWeight = requset.ItemWeight,
                        PackageType = requset.PackageType,
                        RecieverAddress = recieverAddress.Address,
                        RecieverCountry = recieverAddress.Country,
                        RecieverLatitude = recieverAddress.Latitude,
                        RecieverLocality = recieverAddress.Locality,
                        RecieverLongitude = recieverAddress.Longitude,
                        RecieverPostalCode = recieverAddress.Postcode,
                        RecieverState = recieverAddress.State,
                        RecieverEmail = requset.RecieverEmail,
                        RecieverName = requset.RecieverName,
                        RecieverPhoneNo = requset.RecieverPhoneNo,
                        RiderType = requset.RiderType,
                        SenderAddress = senderAddress.Address,
                        SenderCountry = senderAddress.Country,
                        SenderState = senderAddress.State,
                        SenderLatitude = senderAddress.Latitude,
                        SenderLocality = senderAddress.Locality,
                        SenderLongitude = senderAddress.Longitude,
                        SenderPostalCode = senderAddress.Postcode,
                        SenderName = requset.SenderName,
                        SenderPhoneNo = requset.SenderPhoneNo,
                        SenderEmail = requset.SenderEmail,
                        ShippingType = requset.ShippingType,
                        PackageImageLists = packageImages,
                        PaymentChannel = requset.PaymentChannel,
                        StaffId = users.Id,
                        StaffName = $"{users.FirstName} {users.SurName}",
                        PurchaseChannel = "Web Manager App",
                        UserType = "Not Registered",

                    };

                    using var httpClient = new HttpClient();
                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Logistics/add-package-orders", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                    {
                        var body = JsonConvert.DeserializeObject<OrderModelData>(apiResponse)!;
                        return RedirectToAction("CompleteOrder", "Home", new { id = body.Id, paymentChannel = login.PaymentChannel });
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
        public async Task<IActionResult> CompleteOrder(string id, string paymentChannel)
        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {

                    UpdateOrder login = new()
                    {
                        PaymentChannel = paymentChannel,
                        PaymentStatus = true,
                        TrnxReference = CreateRandomTokenSix()
                    };

                    using var httpClient = new HttpClient();
                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    if (paymentChannel == "Paystack")
                    {
                        StringContent content2 = new(JsonConvert.SerializeObject(""), Encoding.UTF8, "application/json");
                        using var response = await httpClient.PutAsync($"{GlobalConstant.BaseUrl}Logistics/initialize-paystack-package-orders/{id}", content2);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                        {
                            var body = JsonConvert.DeserializeObject<PaystackResponse>(apiResponse)!;
                            return Redirect(body.Data!.AuthorizationUrl!);
                        }
                        else
                        {
                            ViewBag.UserError = apiResponse;
                            return RedirectToAction("OrderDetails", "Home", new { id });
                        }
                    }
                    else if (paymentChannel == "Flutterwave")
                    {
                        StringContent content2 = new(JsonConvert.SerializeObject(""), Encoding.UTF8, "application/json");
                        using var response = await httpClient.PutAsync($"{GlobalConstant.BaseUrl}Logistics/initialize-flutterwave-package-orders/{id}", content2);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                        {
                            var body = JsonConvert.DeserializeObject<FlutterwaveResponse>(apiResponse)!;
                            return Redirect(body.Data!.Link!);
                        }
                        else
                        {
                            ViewBag.UserError = apiResponse;
                            return RedirectToAction("OrderDetails", "Home", new { id });
                        }
                    }
                    else
                    {

                        using var response = await httpClient.PutAsync($"{GlobalConstant.BaseUrl}Logistics/package-orders/{id}", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                        {
                            var body = JsonConvert.DeserializeObject<OrderModelData>(apiResponse)!;
                            return RedirectToAction("OrderDetails", "Home", new { id = body.Id });
                        }
                        else
                        {
                            ViewBag.UserError = apiResponse;
                            return RedirectToAction("OrderDetails", "Home", new { id });
                        }
                    }
                }
                catch
                {
                    return RedirectToAction("OrderDetails", "Home", new { id });
                }
            }
            return RedirectToAction("OrderDetails", "Home", new { id });
        }
        [HttpGet]
        public IActionResult OrderList(string id, string date)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.CompanyType = users.CompanyType;
                ViewBag.ManagerId = users.ManagerId;
                // orders
                var stats = OrderStatistics()!.GetAwaiter().GetResult();
                var orders = PackageOrderItems()!.GetAwaiter().GetResult();
                orders = [.. orders!.OrderByDescending(x => x!.CreatedAt)];

                var search = from m in orders select m;

                if (!string.IsNullOrEmpty(id))
                {
                    search = search.Where(s =>
                            (s.SenderName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                            (s.RiderName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                            (s.CompanyName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                            (s.RecieverName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                            (s.OrderStatus?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                            (s.TrnxReference?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                            (s.ItemName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                            (s.RecieverEmail?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                            (s.PurchaseChannel?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                            (s.PaymentChannel?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             s.UserId.ToString() == id || s.CompanyId.ToString() == id || s.StaffId.ToString() == id
                            );
                    var allOrdersData = new AllOrdersDataModel()
                    {
                        OrderStatistics = stats,
                        OrderModelData = [.. search],

                    };

                    return View(allOrdersData);
                }
                else if (!String.IsNullOrEmpty(date))
                {
                    search = search.Where(s => s.CreatedAt.ToString("yyyy-MM-dd") == date);
                    var allOrdersData = new AllOrdersDataModel()
                    {
                        OrderStatistics = stats,
                        OrderModelData = [.. search],

                    };

                    return View(allOrdersData);
                }
                else
                {
                    var allOrdersData = new AllOrdersDataModel()
                    {
                        OrderStatistics = stats,
                        OrderModelData = [.. search],
                    };
                    return View(allOrdersData);
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var userDAta = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = userDAta.ImagePath!;
                ViewBag.AdminName = $"{userDAta.FirstName} {userDAta.SurName}";
                ViewBag.UseType = userDAta.AdminType;
                ViewBag.CompanyType = userDAta.CompanyType;
                ViewBag.ManagerId = userDAta.ManagerId;
                using var httpClient = new HttpClient();
                OrderModelData users = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Logistics/package-orders/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<OrderModelData>(apiResponse)!;
                    string time = users.UpdatedAt.ToString("hh:mm tt");
                    ViewBag.Time = time;
                    ViewBag.Id = id;
                    var dataDetails = new OrdersDetailsDataModel
                    {
                        OrderModelData = users,
                    };
                    return View(dataDetails);
                }
                else
                {
                    ViewBag.StatusCode = response.StatusCode;
                    return RedirectToAction("OrderList", "Home");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }

  
        [HttpPost]
        public async Task<IActionResult> UpdateShippingCost(OrdersDetailsDataModel requset)

        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {
                    UpdateShippingCost login = new()
                    {
                        ShippingCost = requset.UpdateShippingCost!.ShippingCost,
                        VatCost = requset.UpdateShippingCost.VatCost,
                        Id = requset.UpdateShippingCost.Id
                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PutAsync($"{GlobalConstant.BaseUrl}Logistics/package-orders/{login.Id}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        var body = JsonConvert.DeserializeObject<OrderModelData>(apiResponse)!;
                        return RedirectToAction("OrderDetails", "Home", new { id = body.Id });
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return RedirectToAction("OrderDetails", "Home", new { id = login.Id });
                    }
                }
                catch
                {
                    return RedirectToAction("OrderDetails", "Home", new { id = requset.UpdateShippingCost!.Id });
                }
            }
            return RedirectToAction("OrderDetails", "Home", new { id = requset.UpdateShippingCost!.Id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateExpectedDeliveryTime(OrdersDetailsDataModel requset)

        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {
                    UpdateExpectedDeliveryTime login = new()
                    {
                        ExpectedDeliveryTime = requset.UpdateExpectedDeliveryTime!.ExpectedDeliveryTime,
                        Id = requset.UpdateExpectedDeliveryTime!.Id
                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PutAsync($"{GlobalConstant.BaseUrl}Logistics/package-orders/{login.Id}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        var body = JsonConvert.DeserializeObject<OrderModelData>(apiResponse)!;
                        return RedirectToAction("OrderDetails", "Home", new { id = body.Id });
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return RedirectToAction("OrderDetails", "Home", new { id = login.Id });
                    }
                }
                catch
                {
                    return RedirectToAction("OrderDetails", "Home", new { id = requset.UpdateExpectedDeliveryTime!.Id });
                }
            }
            return RedirectToAction("OrderDetails", "Home", new { id = requset.UpdateExpectedDeliveryTime!.Id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(OrdersDetailsDataModel requset)

        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {
                    UpdateOrderStatus login = new()
                    {
                        OrderStatus = requset.UpdateOrderStatus!.OrderStatus,
                        Id = requset.UpdateOrderStatus!.Id
                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PutAsync($"{GlobalConstant.BaseUrl}Logistics/package-orders/{login.Id}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        var body = JsonConvert.DeserializeObject<OrderModelData>(apiResponse)!;
                        return RedirectToAction("OrderDetails", "Home", new { id = body.Id });
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return RedirectToAction("OrderDetails", "Home", new { id = login.Id });
                    }
                }
                catch
                {
                    return RedirectToAction("OrderDetails", "Home", new { id = requset.UpdateOrderStatus!.Id });
                }
            }
            return RedirectToAction("OrderDetails", "Home", new { id = requset.UpdateOrderStatus!.Id });
        }


      

        // Accommodation
        public IActionResult AddAccommodation(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            // var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.Managers = id;
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
                    for (int i = 0; i < requset.ImageList!.Count; i++)
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
                        ManagerId = requset.ManagerId,
                        AccomodationName = requset.AccomodationName,
                        AccomodationEmail = requset.AccomodationEmail,
                        AccomodationPhoneNo = requset.AccomodationPhoneNo,
                        AccomodationDescription = requset.AccomodationDescription,
                        AccomodationType = requset.AccomodationType,
                        AccomodationAdvertType = "Normal",
                        Location = place.Address,
                        Country = place.Country,
                        Locality = place.Locality,
                        Postcode = place.Postcode,
                        State = place.State,
                        Latitude = place.Latitude,
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
                            Sunday = new()
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
                    if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
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
        public async Task<IActionResult> AccommodationDetails()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var userDAta = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = userDAta.ImagePath!;
                ViewBag.AdminName = $"{userDAta.FirstName} {userDAta.SurName}";
                ViewBag.UseType = userDAta.AdminType;
                ViewBag.CompanyType = userDAta.CompanyType;
                ViewBag.Managers = userDAta.ManagerId;
                using var httpClient = new HttpClient();
                AccomodationDataModel users = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation/{userDAta.ManagerId}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<AccomodationDataModel>(apiResponse)!;
                    var reservatioModels = BookAccomodationReservationItems(userDAta.ManagerId.ToString())!.GetAwaiter().GetResult();
                    reservatioModels = [.. reservatioModels.Where(x => x.AccomodationId == users.Id)];

                    var customer = CustomerReservationItems()!.GetAwaiter().GetResult()!;
                    customer = [.. customer.Where(x => x.AccomodationId == users.Id)];

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
                    ViewBag.StatusCode = response;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }
        
        // Reservation
        public IActionResult AddBookReservation(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.CompanyType = users.CompanyType;
                ViewBag.Managers = users.ManagerId;
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
                        return RedirectToAction("AllAccomodationReservationList", "Home");
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
        public IActionResult AllAccomodationReservationList(string id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.CompanyType = users.CompanyType;
                ViewBag.Managers = users.ManagerId;

              var  adminUser = BookAccomodationReservationItems(users.ManagerId.ToString())!.GetAwaiter().GetResult();
                adminUser = [.. adminUser!.OrderByDescending(x => x!.CreatedAt)];
                var search = from m in adminUser select m;
                if (!String.IsNullOrEmpty(id))
                {
                    search = search.Where(s => s.AccomodationName!.Contains(id, StringComparison.CurrentCultureIgnoreCase) || s.TicketNum!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                    || s.AccomodationLocality!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                    || s.AccomodationType!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                    || s.AccomodationState!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                    || s.AccomodationId!.ToString() == id || s.AdminId!.ToString() == id
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
                return RedirectToAction("SignIn", "Auth");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AccomodationReservationDetails(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var userDAta = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = userDAta.ImagePath!;
                ViewBag.AdminName = $"{userDAta.FirstName} {userDAta.SurName}";
                ViewBag.UseType = userDAta.AdminType;
                ViewBag.CompanyType = userDAta.CompanyType;
                ViewBag.Managers = userDAta.ManagerId;
                using var httpClient = new HttpClient();
                BookAccomodationReservatioModel users = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Bookings/accomodation-reservations/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<BookAccomodationReservatioModel>(apiResponse)!;

                    var customer = CustomerReservationItems()!.GetAwaiter().GetResult()!;
                    customer = [.. customer.Where(x => x.AccomodationId == users.Id)];

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
                    return RedirectToAction("AllAccomodationReservationList", "Home");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

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
                return RedirectToAction("AllAccomodationReservationList", "Home");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("AllAccomodationReservationList", "Home");
            }
        }

        // Customer
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
                        UserId = Guid.NewGuid(),
                        PaymentChannel = requset.PaymentChannel,
                        CustomerName = requset.CustomerName,
                        CustomerPhoneNumber = requset.CustomerPhoneNumber,
                        CustomerEmail = requset.CustomerEmail,
                        NumberOfGuests = requset.NumberOfGuests,
                        Comment = requset.Comment,
                        ReservationStartDate = requset.ReservationStartDate,
                        ReservationEndDate = requset.ReservationEndDate,
                        NoOfDays = totalDays,
                        PaymentStatus = true,
                        TrnxReference = requset.PaymentChannel == "Cash" ? CreateRandomTokenSix() : "",
                        StaffId = users.Id,
                        StaffName = $"{users.FirstName} {users.SurName}",
                        PurchaseChannel = "Web Manager App",
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
                            var body = JsonConvert.DeserializeObject<PaystackResponse>(apiResponse)!;
                            return Redirect(body.Data!.AuthorizationUrl!);
                        }
                        else
                        {
                            ViewBag.UserError = apiResponse;
                            return View(requset);
                        }
                    }
                    else if (requset.PaymentChannel == "Flutterwave")
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
                            return RedirectToAction("CustomerReservationDetails", "Home", new { id = body.Id });
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
                ViewBag.UseType = users.AdminType;
                ViewBag.CompanyType = users.CompanyType;
                ViewBag.Managers = users.ManagerId;
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
                    var allOrdersData = search.ToList();
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
                ViewBag.UseType = userDAta.AdminType;
                ViewBag.CompanyType = userDAta.CompanyType;
                ViewBag.Managers = userDAta.ManagerId;
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

    }
}
