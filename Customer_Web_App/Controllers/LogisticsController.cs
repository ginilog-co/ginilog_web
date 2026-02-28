using Customer_Web_App.GlobalConst;
using Customer_Web_App.Models.BookingsModel;
using Customer_Web_App.Models.LogisticsModel;
using Customer_Web_App.Models.WalletModel;
using Customer_Web_App.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Customer_Web_App.Controllers
{
    public class LogisticsController(RepositoryService repositoryService) : Controller
    {
        private readonly RepositoryService _repositoryService = repositoryService;

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
        public IActionResult Index()
        {
            return View();
        }


        // Orders Items
        [HttpGet]
        public async Task<IActionResult> AddOrder(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = await _repositoryService.UserData(token!)!;
                ViewBag.ProfilePics = users.ProfilePicture!;
                ViewBag.FullName = $"{users.FirstName} {users.LastName}"; ;
                ViewBag.Email = users.Email;
                var notify = await _repositoryService.NotificationListItems(token!)!;
                ViewBag.AllNotificationCount = notify.Count(n => n.IsRead == false);
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
            var users = await _repositoryService.UserData(token!)!;
            if (ModelState.IsValid)
            {
                try
                {
                    var senderAddress = await _repositoryService.LocationData(requset.SenderAddress!)!;
                    var recieverAddress = await _repositoryService.LocationData(requset.RecieverAddress!)!;
                    List<string> packageImages = [];
                    for (int i = 0; i < requset.ImageList!.Count; i++)
                    {
                        var image = await _repositoryService.UploadFile(requset.ImageList![i], token!);
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
                        StaffName = $"{users.FirstName} {users.LastName}",
                        PurchaseChannel = "Web Customer App",
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
                            return RedirectToAction("OrderDetails", "Logistics", new { id });
                        }
                    }
                    else 
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
                }
                catch
                {
                    return RedirectToAction("OrderDetails", "Logistics", new { id });
                }
            }
            return RedirectToAction("OrderDetails", "Logistics", new { id });
        }
        [HttpGet]
        public async Task<IActionResult> OrderList(string id, string date)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = await _repositoryService.UserData(token!)!;
                ViewBag.ProfilePics = users.ProfilePicture!;
                ViewBag.FullName = $"{users.FirstName} {users.LastName}"; ;
                ViewBag.Email = users.Email;
                var notify = await _repositoryService.NotificationListItems(token!)!;
                ViewBag.AllNotificationCount = notify.Count(n => n.IsRead == false);
                // orders
                var orders = await _repositoryService.PackageOrderItems(token!)!;
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
                            (s.TrackingNum?.Contains(id, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                             s.UserId.ToString() == id || s.CompanyId.ToString() == id || s.StaffId.ToString() == id
                            );
               

                    return View(search);
                }
                else if (!String.IsNullOrEmpty(date))
                {
                    search = search.Where(s => s.CreatedAt.ToString("yyyy-MM-dd") == date);
            

                    return View(search);
                }
                else
                {
 
                    return View(search);
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
                var users = await _repositoryService.UserData(token!)!;
                ViewBag.ProfilePics = users.ProfilePicture!;
                ViewBag.FullName = $"{users.FirstName} {users.LastName}"; ;
                ViewBag.Email = users.Email;
                var notify = await _repositoryService.NotificationListItems(token!)!;
                ViewBag.AllNotificationCount = notify.Count(n => n.IsRead == false);
                OrderModelData data = await _repositoryService.PackageOrderData(id,token!)!;
                string time = data.UpdatedAt.ToString("hh:mm tt");
                ViewBag.Time = time;
                ViewBag.Id = id;
                var dataDetails = new OrdersDetailsDataModel
                {
                    OrderModelData = data,
                };
                return View(dataDetails);
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }

    }
}
