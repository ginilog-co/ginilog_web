
using Ginilog_AdminWeb.GlobalConst;
using Ginilog_AdminWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;

namespace Ginilog_AdminWeb.Controllers
{
    public class AuthController : Controller
    {
        private string firstName = "";
        private string surName = "";
        private string email = "";
        private string sex = "";
        public string? imagePath = "";
        public async Task<IActionResult> UserProfile()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
            var token = HttpContext.Session.GetString("bt_token");
            var adminType = HttpContext.Session.GetString("bt_userType");
            if (userId != null)
            {
                using var httpClient = new HttpClient();
                AdminModelTable users = new();
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
                    imagePath = users.ImagePath!;
                    ViewBag.ProfilePics = users.ImagePath!;
                    ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                    ViewBag.UseType = adminType;
                    return View(users);
                }
                else
                {
                    ViewBag.StatusCode = response.StatusCode;
                    return RedirectToAction("SignIn", "Auth");
                }

            }
            else
            {
                return RedirectToAction("SignIn", "Home");
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
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminLoginUrl}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var body = JsonConvert.DeserializeObject<AdminLoginDto>(apiResponse)!;
                        ViewBag.Result = apiResponse;
                        HttpContext.Session.SetString("bt_userId", body.UserId.ToString());
                        HttpContext.Session.SetString("bt_token", body.Token!);
                        HttpContext.Session.SetString("bt_userType", body.UserType!);
                        HttpContext.Session.SetString("bt_userEmail", body.Email!);
                        HttpContext.Session.SetString("bt_userState", body.State!);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        if (apiResponse == "Not An Admin Account")
                        {
                            ViewBag.UserError = "Account Does Not Exist";
                            return View(requset);
                        }
                        else
                        {
                            ViewBag.UserError = "Account Does Not Exist";
                            return View(requset);
                        }
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
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.RessetPasswordUrl}", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.ForgetPasswordResult = apiResponse;
                        return RedirectToAction("SignIn", "Home");
                    }
                    else
                    {
                        ViewBag.ForgetPasswordError = "Invalid Token Entered";
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
