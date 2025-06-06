using Ginilog_AdminWeb.GlobalConst;
using Ginilog_AdminWeb.Models;
using Ginilog_AdminWeb.Models.LogisticsModel;
using Ginilog_AdminWeb.Models.WalletModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
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
                        Locality=city,
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
                userDto = userDto.Where(x => x.UserId == userGuid|| x.CompanyId==userGuid).ToList();
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

        // Logistics
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
                var managers = ManagerDataList()!.GetAwaiter().GetResult();
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
        public async Task<IActionResult> AddLogCompany(AddCompany requset)
        {
            var token = HttpContext.Session.GetString("bt_token");
            var managers = ManagerDataList()!.GetAwaiter().GetResult();
            var manager = managers.Select(m => new SelectListItem
            {
                Text = $"{m.FirstName} {m.SurName}",       // What the user sees
                Value = m.Id.ToString()  // What gets submitted
            }).ToList();
            ViewBag.Managers = manager;
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
                        CompanyAddress =place.Address,
                        State = place.State,
                        Latitude = place.Latitude,
                        Longitude = place.Longitude,
                        PostCodes = place.Postcode,
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

        // Orders Items

        [HttpGet]
        public IActionResult OrderList(string id, string date)
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
                var stats = OrderStatistics(id)!.GetAwaiter().GetResult();
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
                             s.UserId.ToString()== id|| s.CompanyId.ToString()== id
                            );
                    var allOrdersData = new AllOrdersDataModel()
                    {
                        OrderStatistics = stats,
                        OrderModelData = search.ToList(),

                    };

                    return View(allOrdersData);
                }
                else if (!String.IsNullOrEmpty(date))
                {
                    search = search.Where(s => s.CreatedAt.ToString("yyyy-MM-dd") == date);
                    var allOrdersData = new AllOrdersDataModel()
                    {
                        OrderStatistics = stats,
                        OrderModelData = search.ToList(),

                    };

                    return View(allOrdersData);
                }
                else
                {
                    var allOrdersData = new AllOrdersDataModel()
                    {
                        OrderStatistics = stats,
                        OrderModelData = search.ToList(),
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
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                var userDAta = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = userDAta.ImagePath!;
                ViewBag.AdminName = $"{userDAta.FirstName} {userDAta.SurName}";
                ViewBag.UseType = adminType;
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
                    ViewBag.Id= id;
                    var dataDetails = new OrdersDetailsDataModel
                    {
                        OrderModelData = users,
                    };
                    return View(dataDetails);
                }
                else
                {
                    ViewBag.StatusCode = response.StatusCode;
                    return RedirectToAction("OrderList", "Logistics");
                }
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }

        [HttpGet]
        public IActionResult AddOrder(Guid id)
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

                ViewBag.CompanyId = id;
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
                        PackageType=requset.PackageType,
                        RecieverAddress= recieverAddress.Address,
                        RecieverCountry= recieverAddress.Country,
                        RecieverLatitude= recieverAddress.Latitude,
                        RecieverLocality= recieverAddress.Locality,
                        RecieverLongitude= recieverAddress.Longitude,
                        RecieverPostalCode = recieverAddress.Postcode,
                        RecieverState = recieverAddress.State,
                        RecieverEmail = requset.RecieverEmail,
                        RecieverName =requset.RecieverName,
                        RecieverPhoneNo=requset.RecieverPhoneNo,
                        RiderType=requset.RiderType,
                        SenderAddress= senderAddress.Address,
                        SenderCountry= senderAddress.Country,
                        SenderState = senderAddress.State,
                        SenderLatitude = senderAddress.Latitude,
                        SenderLocality= senderAddress.Locality,
                        SenderLongitude= senderAddress.Longitude,
                        SenderPostalCode = senderAddress.Postcode,
                        SenderName =requset.SenderName,
                        SenderPhoneNo=requset.SenderPhoneNo,
                        SenderEmail = requset.SenderEmail,
                        ShippingType=requset.ShippingType,
                        PackageImageLists=packageImages,
                        PaymentChannel=requset.PaymentChannel,

                    };

                    using var httpClient = new HttpClient();
                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}Logistics/add-package-orders", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                    {
                        var body = JsonConvert.DeserializeObject<OrderModelData>(apiResponse)!;
                        return RedirectToAction("CompleteOrder", "Logistics", new { id = body.Id, paymentChannel=login.PaymentChannel });
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
        public async Task<IActionResult> CompleteOrder(string id,string paymentChannel)
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
                        TrnxReference =  CreateRandomTokenSix() 
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
                            return RedirectToAction("OrderDetails", "Logistics", new { id });
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
                            return RedirectToAction("OrderDetails", "Logistics", new { id });
                        }
                    }
                    else
                    {

                        using var response = await httpClient.PutAsync($"{GlobalConstant.BaseUrl}Logistics/package-orders/{id}", content);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                        {
                            var body = JsonConvert.DeserializeObject<OrderModelData>(apiResponse)!;
                            return RedirectToAction("OrderDetails", "Logistics", new { id = body.Id });
                        }
                        else
                        {
                            ViewBag.UserError = apiResponse;
                            return RedirectToAction("OrderDetails", "Logistics", new { id });
                        }
                    }
                }
                catch
                {
                    return RedirectToAction("OrderDetails", "Logistics", new { id });
                }
            }
            return RedirectToAction("OrderDetails", "Logistics", new { id });
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
                        Id=requset.UpdateShippingCost.Id
                    };

                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = await httpClient.PutAsync($"{GlobalConstant.BaseUrl}Logistics/package-orders/{login.Id}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        var body = JsonConvert.DeserializeObject<OrderModelData>(apiResponse)!;
                        return RedirectToAction("OrderDetails", "Logistics", new { id = body.Id });
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return RedirectToAction("OrderDetails", "Logistics", new { id = login.Id });
                    }
                }
                catch
                {
                    return RedirectToAction("OrderDetails", "Logistics", new { id = requset.UpdateShippingCost!.Id });
                }
            }
            return RedirectToAction("OrderDetails", "Logistics", new { id = requset.UpdateShippingCost!.Id });
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
                        return RedirectToAction("OrderDetails", "Logistics", new { id = body.Id });
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return RedirectToAction("OrderDetails", "Logistics", new { id = login.Id });
                    }
                }
                catch
                {
                    return RedirectToAction("OrderDetails", "Logistics", new { id = requset.UpdateExpectedDeliveryTime!.Id });
                }
            }
            return RedirectToAction("OrderDetails", "Logistics", new { id = requset.UpdateExpectedDeliveryTime!.Id });
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
                        return RedirectToAction("OrderDetails", "Logistics", new { id = body.Id });
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return RedirectToAction("OrderDetails", "Logistics", new { id = login.Id });
                    }
                }
                catch
                {
                    return RedirectToAction("OrderDetails", "Logistics", new { id = requset.UpdateOrderStatus!.Id });
                }
            }
            return RedirectToAction("OrderDetails", "Logistics", new { id = requset.UpdateOrderStatus!.Id });
        }


        [HttpGet]
        public IActionResult DeleteOrder(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteOrderConfirm(Guid id)
        {

            var token = HttpContext.Session.GetString("bt_token");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await httpClient.DeleteAsync($"{GlobalConstant.BaseUrl}Logistics/package-orders/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return RedirectToAction("OrderList", "Logistics");
            }
            else
            {
                ViewBag.StatusCode = response.StatusCode;
                return RedirectToAction("OrderList", "Home");
            }
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
