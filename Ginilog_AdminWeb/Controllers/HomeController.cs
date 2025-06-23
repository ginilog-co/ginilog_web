using Ginilog_AdminWeb.GlobalConst;
using Ginilog_AdminWeb.Models;
using Ginilog_AdminWeb.Models.BookingsModel;
using Ginilog_AdminWeb.Models.InfoModel;
using Ginilog_AdminWeb.Models.LogisticsModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Ginilog_AdminWeb.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;

        private string firstName = "";
        private string surName = "";
        private string email = "";
        private string sex = "";
        public string? imagePath = "";

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
        // Feedback list

        public async Task<List<FeedbackModel>>? FeedbackModelItems()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                List<FeedbackModel>? reservationList = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Info/feedback");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<FeedbackModel>>(apiResponse);
                    reservationList = [.. reservationList!.OrderByDescending(x => x.CreatedAt)];
                    return reservationList;
                }
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
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var stats = OrderStatistics()!.GetAwaiter().GetResult();
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = adminType;
                var data = PackageOrderItems()!.GetAwaiter().GetResult();
                data = data.Where(x => x.OrderStatus == "Open" || x.OrderStatus == "Cancelled").ToList();
                var reservations = CustomerReservationItems()!.GetAwaiter().GetResult();
                reservations = reservations.Where(x=> x.TicketClosed==false).ToList();
                var dataModel = new HomeModelData()
                {
                    OrderModelData = data,
                    OrderStatistics = stats,
                    CustomerBookedReservation = reservations,
                };
                return View(dataModel);
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }

        // Send Email

        [HttpPost]
        public async Task<IActionResult> SendEmail(HomeModelData requset)
        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {
                    SendMailModel login = new()
                    {
                        Name = requset.SendMailModel!.Name,
                        Email = requset.SendMailModel.Email,
                        Title = requset.SendMailModel.Title,
                        Message = requset.SendMailModel.Message,
                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Info/send-mail", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.UserError = "Error Sending Mail";
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SendAllEmail(HomeModelData requset)
        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {
                    SendMailModel login = new()
                    {

                        Link = requset.SendMailModel!.Link!,
                        Title = requset.SendMailModel!.Title,
                        Message = requset.SendMailModel!.Message,
                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Info/send-all-mail", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.UserError = "Account Does Not Exist";
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> FeedBackList(string id)
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
                List<FeedbackModel>? reservationList = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Info/feedback");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<FeedbackModel>>(apiResponse);
                    reservationList = [.. reservationList!.OrderByDescending(x => x.CreatedAt)];
                    var search = from m in reservationList select m;
                    if (!String.IsNullOrEmpty(id))
                    {
                        search = search.Where(s => s.Name!.Contains(id, StringComparison.CurrentCultureIgnoreCase) || s.PhoneNo!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.Email!.Contains(id, StringComparison.CurrentCultureIgnoreCase));
                        return View(search.ToList());
                    }
                    else
                    {
                        return View(reservationList);
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
        public async Task<IActionResult> DeleteFeedBack(Guid id)
        {
            var token = HttpContext.Session.GetString("bt_token");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await httpClient.DeleteAsync($"{GlobalConstant.BaseUrl}Info/feedback/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction("FeedBackList", "Home");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("FeedBackList", "Home");
            }
        }

        // All Admin

        [HttpGet]
        public async Task<IActionResult> AllAdminUser(string id)
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
                List<AdminModelTable> adminUser = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    adminUser = JsonConvert.DeserializeObject<List<AdminModelTable>>(apiResponse)!;
                    adminUser = [.. adminUser!.OrderByDescending(x => x!.CreatedAt)];
                    var search = from m in adminUser select m;
                    if (!String.IsNullOrEmpty(id))
                    {
                        search = search.Where(s => s.FirstName!.Contains(id, StringComparison.CurrentCultureIgnoreCase) || s.SurName!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.Email!.Contains(id, StringComparison.CurrentCultureIgnoreCase));
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

        public IActionResult AddAdminUser()
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
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }
        [HttpPost]
        public async Task<IActionResult> AddAdminUser(AddAdminRequest requset)
        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {
                    AddAdminRequest login = new()
                    {
                        Email = requset.Email,
                        Password = requset.Password,
                        FirstName = requset.FirstName,
                        SurName = requset.SurName,
                        PhoneNo = requset.PhoneNo,
                        Sex = requset.Sex,
                        StaffCode = requset.StaffCode,
                        AdminType = requset.AdminType,
                        State = requset.State,
                        Locality = requset.Locality,
                        Address = requset.Address,
                        Branch = requset.Branch,
                        CompanyName = requset.CompanyName,
                        CompanyType = requset.CompanyType,
                        CompanyUserName = requset.CompanyUserName,
                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        var body = JsonConvert.DeserializeObject<AdminModelTable>(apiResponse)!;
                        return RedirectToAction("AllAdminUser", "Home");
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
        public IActionResult DeleteAdmin(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAdminConfirm(Guid id)
        {

            var token = HttpContext.Session.GetString("bt_token");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await httpClient.DeleteAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction("AllAdminUser", "Home");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("AllAdminUser", "Home");
            }
        }

        // Staff Data
        [HttpGet]
        public async Task<IActionResult> AllStaffData(string id)
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
                List<AdminModelTable> adminUser = [];
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
                    AddStaffDataTable login = new()
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
                        var body =apiResponse;
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
        public IActionResult AllManagerList(string id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                var adminUser = ManagerDataList()!.GetAwaiter().GetResult();
                adminUser = [.. adminUser!.OrderByDescending(x => x!.CreatedAt)];
                var search = from m in adminUser select m;
                if (!string.IsNullOrEmpty(id))
                {
                    search = search.Where(s => (s.FirstName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.SurName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.PhoneNo?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.Locality?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.State?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.CompanyName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.CompanyUserName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
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
        public IActionResult ManagerDetails(Guid id)
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
                var managers = ManagerData(id)!.GetAwaiter().GetResult();
                return View(managers);
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }

        // Advert
        [HttpGet]
        public async Task<IActionResult> AllAdvertData(string id)
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
                List<AdvertHolderModel> adminUser = [];
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}advert");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    adminUser = JsonConvert.DeserializeObject<List<AdvertHolderModel>>(apiResponse)!;
                    adminUser = [.. adminUser!.OrderByDescending(x => x!.CreatedAt)];
                    var search = from m in adminUser select m;
                    if (!String.IsNullOrEmpty(id))
                    {
                        search = search.Where(s => s.AdvertName!.Contains(id, StringComparison.CurrentCultureIgnoreCase) || s.AdvertType!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.TransRef!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        || s.AdvertItemDescription!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
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
        public async Task<IActionResult> AdvertDetails(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var userDAta = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = userDAta.ImagePath!;
                ViewBag.AdminName = $"{userDAta.FirstName} {userDAta.SurName}";
                ViewBag.UseType = userDAta.AdminType;
                using var httpClient = new HttpClient();
                AdvertHolderModel users = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}advert/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<AdvertHolderModel>(apiResponse)!;
                    ViewBag.Id = id;
                    var managers = ManagerData(users.AdminId)!.GetAwaiter().GetResult();
                    var dataDetails = new MainAdvertModel
                    {
                        AdvertHolderModel = users,
                        AdminModelTable= managers,
                    };
                    return View(dataDetails);
                }
                else
                {
                    ViewBag.StatusCode = response.StatusCode;
                    return RedirectToAction("AllAdvertData", "Home");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }

        [HttpGet]
        public IActionResult DeleteAdvert(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAdvertConfirm(Guid id)
        {

            var token = HttpContext.Session.GetString("bt_token");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await httpClient.DeleteAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}advert/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction("AllAdvertData", "Home");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("AllAdvertData", "Home");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
