using Customer_Web_App.GlobalConst;
using Customer_Web_App.Models;
using Customer_Web_App.Models.BookingsModel;
using Customer_Web_App.Models.WalletModel;
using Customer_Web_App.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Customer_Web_App.Controllers
{
    public class HomeController(RepositoryService repositoryService) : Controller
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

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            ViewBag.Login = HttpContext.Session.GetString("bt_userLogin");
            if (userId != null)
            {
                var users = _repositoryService.UserData(token!)!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ProfilePicture!;
                ViewBag.FullName = $"{users.FirstName} {users.LastName}"; ;
                ViewBag.Email = users.Email;
            }
            // Package Orders
            var company = (await _repositoryService.LogisticsCompanyItems()!).OrderBy(x => Guid.NewGuid()).Take(4).ToList();

            var accomodation = (await _repositoryService.AccomodationListItems()!).OrderBy(x => Guid.NewGuid()).Take(4).ToList();

            var reservation = (await _repositoryService.ReservationListItems("")!).OrderBy(x => Guid.NewGuid()).Take(4).ToList();

            var dataModel = new HomeModelData()
            {
                CompanyModelData = company,
                AccomodationDataModel = accomodation,
                BookAccomodationReservatioModel = reservation,
            };

            return View(dataModel);
            
        }


        [HttpGet]
        public async Task<IActionResult> AccomodationList(string id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            ViewBag.Login = HttpContext.Session.GetString("bt_userLogin");
            if (userId != null)
            {
                var users = await _repositoryService.UserData(token!)!;
                ViewBag.ProfilePics = users.ProfilePicture!;
                ViewBag.FullName = $"{users.FirstName} {users.LastName}"; ;
                ViewBag.Email = users.Email;
                var notify = await _repositoryService.NotificationListItems(token!)!;
                ViewBag.AllNotificationCount = notify.Count(n => n.IsRead == false);
            }
            
            List<AccomodationDataModel>? reservationList = await _repositoryService.AccomodationListItems()!;
            reservationList = [.. reservationList!.OrderByDescending(x => x!.CreatedAt)];
            var search = from m in reservationList select m;
            if (!String.IsNullOrEmpty(id))
            {
                search = search.Where(s => s.AccomodationName!.Contains(id, StringComparison.CurrentCultureIgnoreCase) || s.Location!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                || s.Locality!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                || s.State!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                || s.AccomodationAdvertType!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                || s.AccomodationType!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                || s.Country!.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                );
                return View(search.ToList());
            }
            else
            {
                return View(search);
            }
        }


        [HttpGet]
        public async Task<IActionResult> AccomodationDetails(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            ViewBag.Login = HttpContext.Session.GetString("bt_userLogin");
            if (userId != null)
            {
                var users = await _repositoryService.UserData(token!)!;
                ViewBag.ProfilePics = users.ProfilePicture!;
                ViewBag.FullName = $"{users.FirstName} {users.LastName}"; ;
                ViewBag.Email = users.Email;
                var notify = await _repositoryService.NotificationListItems(token!)!;
                ViewBag.AllNotificationCount = notify.Count(n => n.IsRead == false);
            }

            AccomodationDataModel? reservationList = await _repositoryService.AccomodationData(id)!;
           return View(reservationList);
        }


        [HttpGet]
        public async Task<IActionResult> AccomodationReservationList(string id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            ViewBag.Login = HttpContext.Session.GetString("bt_userLogin");
            if (userId != null)
            {
                var users = await _repositoryService.UserData(token!)!;
                ViewBag.ProfilePics = users.ProfilePicture!;
                ViewBag.FullName = $"{users.FirstName} {users.LastName}"; ;
                ViewBag.Email = users.Email;
                var notify = await _repositoryService.NotificationListItems(token!)!;
                ViewBag.AllNotificationCount = notify.Count(n => n.IsRead == false);
            }

            List<BookAccomodationReservatioModel>? reservationList = await _repositoryService.ReservationListItems(id==""|| id==null?"":id)!;
            reservationList = [.. reservationList!.OrderByDescending(x => x!.CreatedAt)];
            var search = from m in reservationList select m;
     
                return View(search);
            
        }

        // Customer
        public async Task<IActionResult> AddCustomerReservation(Guid id)
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {
                var users = await _repositoryService.UserData(token!)!;
                ViewBag.ProfilePics = users.ProfilePicture!;
                ViewBag.FullName = $"{users.FirstName} {users.LastName}"; 
                ViewBag.Email = users.Email;
                ViewBag.PhoneNo = users.PhoneNo;
                var notify = await _repositoryService.NotificationListItems(token!)!;
                ViewBag.AllNotificationCount = notify.Count(n => n.IsRead == false);
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
            var users = await _repositoryService.UserData(token!)!;
            ViewBag.FullName = $"{users.FirstName} {users.LastName}";
            ViewBag.Email = users.Email;
            ViewBag.PhoneNo = users.PhoneNo;
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
                        StaffName = $"{users.FirstName} {users.LastName}",
                        PurchaseChannel = "Web Customer App",
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
                    else 
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
               

                }
                catch
                {
                    return View(requset);
                }
            }
            return View(requset);
        }


        public IActionResult Privacy()
        {
            ViewBag.Login = HttpContext.Session.GetString("bt_userLogin");
            return View();
        }
    }
}
