using Ginilog_AdminWeb.GlobalConst;
using Ginilog_AdminWeb.Models;
using Ginilog_AdminWeb.Models.BookingsModel;
using Ginilog_AdminWeb.Models.LogisticsModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace Ginilog_AdminWeb.Controllers
{
    public class LogisticsController : Controller
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
        public async Task<OrderStatistics>? OrderStatistics(Guid id)
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
        [HttpGet]
        public IActionResult AllLogCompanyList(string id)
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
                var adminUser = LogCompanyItems()!.GetAwaiter().GetResult();
                adminUser = [.. adminUser!.OrderByDescending(x => x!.CreatedAt)];
                var search = from m in adminUser select m;
                if (!string.IsNullOrEmpty(id))
                {
                    search = search.Where(s => (s.CompanyName?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.CompanyAddress?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.State?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             (s.Locality?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false));

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

        public IActionResult AddLogCompany()
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
        public async Task<IActionResult> AddLogCompany(AddCompany requset)
        {
            var token = HttpContext.Session.GetString("bt_token");
            if (ModelState.IsValid)
            {
                try
                {
                    var logo = await UploadFile(requset.LogoUpload!, token!);
                   
                    AddCompany login = new()
                    {
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
                        Locality = requset.Locality,
                        CompanyAddress = requset.CompanyAddress,
                        State = requset.State,
                        Latitude = requset.Latitude,
                        Longitude = requset.Longitude,
                        PostCodes = requset.PostCodes,
                        DeliveryTypes = requset.DeliveryTypes,
                        ServiceAreas = requset.ServiceArea!.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList(),
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
                        return RedirectToAction("AllLogCompanyList", "Logistics");
                    }
                    else
                    {
                        ViewBag.UserError = "Account Does Not Exist";
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

        [HttpGet]
        public async Task<IActionResult> LogCompanyDetails(Guid id)
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
                using var httpClient = new HttpClient();
                CompanyModelData dataModel = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await httpClient.GetAsync($"{GlobalConstant.BaseUrl}Logistics/{id}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dataModel = JsonConvert.DeserializeObject<CompanyModelData>(apiResponse)!;
                    // orders
                    var orders = PackageOrderItems()!.GetAwaiter().GetResult();
                    orders = orders.Where(x => x.CompanyId == dataModel.Id).ToList();
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
                    return RedirectToAction("AllLogCompanyList", "Logistics");
                }

            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }

        [HttpGet]
        public IActionResult LogCompanyOrderList(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                ViewBag.Id = id;
                // orders
                var stats = OrderStatistics(id)!.GetAwaiter().GetResult();
                var orders = PackageOrderItems()!.GetAwaiter().GetResult();
                orders = orders.Where(x => x.CompanyId == id).ToList();
                var allOrdersData = new AllOrdersDataModel()
                {
                    OrderStatistics = stats,
                    OrderModelData = orders,
                };
                return View(allOrdersData);

            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }


        [HttpGet]
        public IActionResult DeleteLogCompany(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteLogCompanyConfirm(Guid id)
        {

            var token = HttpContext.Session.GetString("bt_token");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await httpClient.DeleteAsync($"{GlobalConstant.BaseUrl}Logistics/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction("AllLogCompanyList", "Logistics");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("AllLogCompanyList", "Logistics");
            }
        }
    }
}
