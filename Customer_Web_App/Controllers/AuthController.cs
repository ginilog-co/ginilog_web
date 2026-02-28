using Customer_Web_App.GlobalConst;
using Customer_Web_App.Models;
using Customer_Web_App.Models.BookingsModel;
using Customer_Web_App.Models.UsersDataModel;
using Customer_Web_App.Repository;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Customer_Web_App.Controllers
{
    public class AuthController(RepositoryService repositoryService) : Controller
    {
        private readonly RepositoryService _repositoryService = repositoryService;

        public async Task<IActionResult> UserProfile()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
           
            if (userId != null)
            {
                using var httpClient = new HttpClient();
                UserModelTable users = await _repositoryService.UserData(token!)!;
                var userBookings = await _repositoryService.BookedReservationListItems(token!)!;
                var userOrders = await _repositoryService.PackageOrderItems(token!)!;
                var notify = await _repositoryService.NotificationListItems(token!)!;
                ViewBag.FirstName = users.FirstName!;
                ViewBag.SurName = users.LastName!;
                ViewBag.Email = users.Email!;
                ViewBag.Sex = users.Sex!;
                ViewBag.ProfilePics = users.ProfilePicture!;
                ViewBag.UserName = $"{users.FirstName} {users.LastName}";
                ViewBag.Login = HttpContext.Session.GetString("bt_userLogin");

                UserDetailsModel userDetails = new()
                {
                    UserModelTable = users,
                    CustomerBookedReservations = userBookings,
                    OrderModelDatas = userOrders,
                   NotificationModels = notify,
                };

                return View(userDetails);
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");
            }

        }


        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(LoginRequset requset)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    LoginRequset login = new()
                    {
                        Email_PhoneNo = requset.Email_PhoneNo,
                        Password = requset.Password,
                    };
                    HttpContext.Session.SetString("bt_userPassword", requset.Password!);
                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.UserLoginUrl}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                    {
                        var body = JsonConvert.DeserializeObject<LoginDto>(apiResponse)!;
                        ViewBag.Result = apiResponse;
                        HttpContext.Session.SetString("bt_userId", body.UserId.ToString());
                        HttpContext.Session.SetString("bt_token", body.Token!);
                        HttpContext.Session.SetString("bt_userType", body.UserType!);
                        HttpContext.Session.SetString("bt_userEmail", body.Email!);
                        HttpContext.Session.SetString("bt_userLogin", "Login");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return View(requset);
                    }
                }
                catch(Exception ex)
                {
                    ViewBag.UserError =ex.Message;
                    return View(requset);
                }
            }
            return View(requset);
        }

        [HttpPost]
        public async Task<IActionResult> FirebaseSignIn(LoginRequset req)
        {
            try
            {

                FirebaseToken decoded;
                decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(req.IdToken);

                // Firebase user info (claims)
                var uid = decoded.Uid;

                // email may be in decoded.Claims depending on provider
                var email = decoded.Claims.TryGetValue("email", out var e) ? e?.ToString() : null;

                var name = decoded.Claims.TryGetValue("name", out var n) ? n?.ToString() : null;
                var picture = decoded.Claims.TryGetValue("picture", out var p) ? p?.ToString() : null;
                var phone = decoded.Claims.TryGetValue("phone_number", out var ph) ? ph?.ToString() : null;

                string? firstName = null;
                string? surname = null;

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 1)
                    {
                        firstName = parts[0];
                    }
                    else if (parts.Length >= 2)
                    {
                        firstName = parts[0];
                        surname = parts[^1]; // last word
                    }
                }


                // OPTION A: Just store firebase identity in session
                HttpContext.Session.SetString("bt_firebase_uid", uid);
                if (!string.IsNullOrEmpty(email))
                    HttpContext.Session.SetString("bt_userEmail", email);

                // OPTION B (Recommended): send token to your API to login/register and return your own JWT
                // Example: /auth/firebase-login on your API (you create it)
                // It should verify the same token again server-side, create user, return AdminLoginDto.

                LoginExternalRequset login = new()
                {
                    Email = email,
                    IdToken = req.IdToken,
                    ExternalId=uid,
                    FirstName = firstName,
                    LastName = surname,
                    ProfilePicture = picture,
                    PhoneNo = phone,
                };

                using var httpClient = new HttpClient();

                StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.UserUrl}/auth-login", content);
                var apiResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK|| response.StatusCode == HttpStatusCode.Created)
                {
                    var body = JsonConvert.DeserializeObject<LoginDto>(apiResponse)!;

                    HttpContext.Session.SetString("bt_userId", body.UserId.ToString());
                    HttpContext.Session.SetString("bt_token", body.Token!);
                    HttpContext.Session.SetString("bt_userType", body.UserType!);
                    HttpContext.Session.SetString("bt_userEmail", body.Email!);
                    HttpContext.Session.SetString("bt_userLogin", "Login");
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ViewBag.UserError = apiResponse;
                    return View(req);
                }


            }
            catch (Exception ex)
            {
                ViewBag.UserError = ex.Message;
                return View(req);
            }
        }

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(AddUserRequest requset)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    AddMainUserRequest login = new()
                    {
                        FirstName =requset.FirstName,
                        LastName =requset.LastName,
                        Email = requset.Email,
                        PhoneNo = requset.PhoneNo,
                        Password = requset.Password,
                    };
                    HttpContext.Session.SetString("bt_userPassword", requset.Password!);
                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.UserUrl}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode==HttpStatusCode.Created)
                    {
                        var body = JsonConvert.DeserializeObject<UserModelTable>(apiResponse)!;
                        ViewBag.Result = apiResponse;
                        HttpContext.Session.SetString("bt_userId", body.Id.ToString());
                        HttpContext.Session.SetString("bt_userEmail", body.Email!);
                        HttpContext.Session.SetString("bt_userState", body.State!);
                        return RedirectToAction("EmailVerication", "Auth");
                    }
                    else
                    {
                        ViewBag.UserError = apiResponse;
                        return View(requset);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.UserError = ex.Message;
                    return View(requset);
                }
            }
            return View(requset);
        }


        public IActionResult EmailVerication()
        {
            var email = HttpContext.Session.GetString("bt_userEmail");
            ViewBag.Email = email;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EmailVerication(EmailVerification requset)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = HttpContext.Session.GetString("bt_userEmail");
                    ViewBag.Email = email;
                    var password = HttpContext.Session.GetString("bt_userPassword");

                    EmailVerification login = new()
                    {
                       Token = requset.Token,
                       Password = password,
                    };
                    using var httpClient = new HttpClient();
                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.EmailVerificationUrl}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.ForgetPasswordResult = apiResponse;
                        return RedirectToAction("SignIn", "Auth");
                    }
                    else
                    {
                        ViewBag.ForgetPasswordError = "Email Does Not Exist";
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

        [HttpPost]
        public async Task<IActionResult> ResendEmailVerication()
        {

                try
                {
                    var email = HttpContext.Session.GetString("bt_userEmail");
                      ViewBag.Email= email;

                ResendEmailVerification login = new()
                    {
                        Email =email,
                    };
                    using var httpClient = new HttpClient();
                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.ResendEmailVerificationTokenUrl}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.ForgetPasswordResult = apiResponse;
                        return RedirectToAction("EmailVerication", "Auth");
                    }
                    else
                    {
                        ViewBag.ForgetPasswordError = "Email Does Not Exist";
                        return RedirectToAction("EmailVerication", "Auth");
                    }
                }
                catch
                {
                    return RedirectToAction("EmailVerication", "Auth");
                }

        }

        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest requset)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ForgetPasswordRequestDto login = new()
                    {
                        Email = requset.Email,
                    };
                    using var httpClient = new HttpClient();
                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.ForgetPasswordUrl}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.ForgetPasswordResult = apiResponse;
                        return RedirectToAction("RessetPassword", "Auth");
                    }
                    else
                    {
                        ViewBag.ForgetPasswordError = apiResponse;
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


        public IActionResult RessetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RessetPassword(ResetPasswordRequest requset)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ResetPasswordRequest login = new()
                    {
                        Token = requset.Token,
                        Password = requset.Password

                    };
                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.ResetPasswordUrl}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.ForgetPasswordResult = apiResponse;
                        return RedirectToAction("SignIn", "Auth");
                    }
                    else
                    {
                        ViewBag.ForgetPasswordError = apiResponse;
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

        public IActionResult LogOut()
        {
            return View();
        }
        public IActionResult LogOutConfirm()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn", "Auth");
        }
    }
}
