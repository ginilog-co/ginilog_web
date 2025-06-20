using Ginilog_AdminWeb.GlobalConst;
using Ginilog_AdminWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Ginilog_AdminWeb.Models.LogisticsModel;
using Ginilog_AdminWeb.Models.BookingsModel;
using Ginilog_AdminWeb.Models.UsersDataModel;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ginilog_AdminWeb.Models.WalletModel;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Ginilog_AdminWeb.Controllers
{
    public class UserDataController : Controller
    {
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
                    using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}profile");
                    if (response.StatusCode == HttpStatusCode.OK)
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
                        users = users.Where(x => x.AdminType == "Manager" && x.CompanyType!.Contains("Logistics")).ToList();
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
        public async Task<List<CompanyModelData>>? LogCompanyItems()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                List<CompanyModelData>? reservationList = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Logistics");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<CompanyModelData>>(apiResponse);
                    reservationList = [.. reservationList!.OrderByDescending(x => x.CreatedAt)];
                    _ = reservationList.ToList();
                    return reservationList;
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
        public async Task<OrderStatistics>? OrderStatistics(string id)
        {
            var datass = await PackageOrderItems()!;

            DateTime localTime = DateTime.Now;
            TimeZoneInfo nigeriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            DateTime nigeriaTime = TimeZoneInfo.ConvertTimeFromUtc(localTime.ToUniversalTime(), nigeriaTimeZone);
            var userDto = datass.ToList();
            // ✅ Filter by valid Guid if provided
            if (Guid.TryParse(id, out Guid userGuid))
            {
                userDto = userDto.Where(x => x.UserId == userGuid || x.CompanyId == userGuid).ToList();
            }
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

        // User Data
        public async Task<List<UserModelTable>>? UserItems()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                List<UserModelTable>? reservationList = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}AuthUsers");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<UserModelTable>>(apiResponse);
                    reservationList = [.. reservationList!.OrderByDescending(x => x.CreatedAt)];
                    _ = reservationList.ToList();
                    return reservationList;
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        // The View
        [HttpGet]
        public IActionResult AllUserList(string id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                var adminUser = UserItems()!.GetAwaiter().GetResult();
                adminUser = [.. adminUser!.OrderByDescending(x => x!.CreatedAt)];
                var search = from m in adminUser select m;
                if (!string.IsNullOrEmpty(id))
                {
                    search = search.Where(s => (s.FirstName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.LastName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.PhoneNo?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.Locality?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.State?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.Email?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false));

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
        public async Task<IActionResult> UserDetails(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                using var httpClient = new HttpClient();
                UserModelTable dataModel = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}AuthUsers/for-admin/{id}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dataModel = JsonConvert.DeserializeObject<UserModelTable>(apiResponse)!;
                    // orders
                    var orders = PackageOrderItems()!.GetAwaiter().GetResult();
                    orders = orders.Where(x => x.UserId == dataModel.Id).ToList();
                    var reservation = CustomerReservationItems()!.GetAwaiter().GetResult();
                    reservation= reservation.Where(x => x.UserId==dataModel.Id  ).ToList();
                    ViewBag.Id = dataModel.Id;

                    var details = new UserDetailsModel()
                    {
                        UserModelTable = dataModel,
                        OrderModelDatas = orders,
                        CustomerBookedReservations = reservation
                    };
                    return View(details);
                }
                else
                {
                    ViewBag.StatusCode = response.StatusCode;
                    return RedirectToAction("AllUserList", "UserData");
                }

            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }

        [HttpGet]
        public IActionResult DeleteUser(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUserConfirm(Guid id)
        {

            var token = HttpContext.Session.GetString("bt_token");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await httpClient.DeleteAsync($"{GlobalConstant.BaseUrl}AuthUsers/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction("AllUserList", "UserData");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("AllUserList", "UserData");
            }
        }

        [HttpGet]
        public IActionResult AddOrder(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;

                ViewBag.UserId = id;
                var companies = LogCompanyItems()!.GetAwaiter().GetResult();
                var company = companies.Select(m => new SelectListItem
                {
                    Text = $"{m.CompanyName}",       // What the user sees
                    Value = m.Id.ToString()  // What gets submitted
                }).ToList();
                ViewBag.Companies = company;
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
            var users = Data()!.GetAwaiter().GetResult();
            ViewBag.UserId = requset.UserId;
            var companies = LogCompanyItems()!.GetAwaiter().GetResult();
            var company = companies.Select(m => new SelectListItem
            {
                Text = $"{m.CompanyName}",       // What the user sees
                Value = m.Id.ToString()  // What gets submitted
            }).ToList();
            ViewBag.Companies = company;
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
                        PurchaseChannel = "Web Admin App",
                        UserType = "Registered",
                    };

                    using var httpClient = new HttpClient();
                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Logistics/add-package-orders", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                    {
                        var body = JsonConvert.DeserializeObject<OrderModelData>(apiResponse)!;
                        return RedirectToAction("CompleteOrder", "Logistics", new { id = body.Id, paymentChannel = login.PaymentChannel });
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

        public IActionResult AddCustomerReservation(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.UserId = id;
                var companies = BookAccomodationReservationItems()!.GetAwaiter().GetResult();
                var company = companies.Select(m => new SelectListItem
                {
                    Text = $"{m.AccomodationName} {m.RoomType}",       // What the user sees
                    Value = m.Id.ToString()  // What gets submitted
                }).ToList();
                ViewBag.Companies = company;
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
            ViewBag.UserId = requset.UserId;
            var companies = BookAccomodationReservationItems()!.GetAwaiter().GetResult();
            var company = companies.Select(m => new SelectListItem
            {
                Text = $"{m.AccomodationName} {m.RoomType}",       // What the user sees
                Value = m.Id.ToString()  // What gets submitted
            }).ToList();
            ViewBag.Companies = company;

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime startDate = DateTime.Parse(requset.ReservationStartDate!);
                    DateTime endDate = DateTime.Parse(requset.ReservationEndDate!);
                    int totalDays = (endDate - startDate).Days;
                    AddPaymentCustomerBookedReservation login = new()
                    {
                        UserId = requset.UserId,
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
                        PurchaseChannel = "Web Admin App",
                        UserType = "Registered",
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
                            ViewBag.UserId = login.UserId;
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
                            return RedirectToAction("CustomerReservationDetails", "Bookings", new { id = body.Id });
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

    }
}
