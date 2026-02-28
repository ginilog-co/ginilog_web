using Ginilog_Company_WebDasboard.GlobalConst;
using Ginilog_Company_WebDasboard.Models;
using Ginilog_Company_WebDasboard.Models.BookingsModel;
using Ginilog_Company_WebDasboard.Models.LogisticsModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Ginilog_Company_WebDasboard.Controllers
{
    public class AuthController : Controller
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
            using var response = await httpClient.GetAsync($"https://maps.googleapis.com/maps/api/place/details/json?placeid={id}&key=AIzaSyA1WkH5DbnyUVLhPtqo_qj3Bmr0uKPolSw");
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

        public IActionResult UserProfile()
        {
            var userId = HttpContext.Session.GetString("bt_userId");
          //  var token = HttpContext.Session.GetString("bt_token");
            if (userId != null)
            {

                var users = Data()!.GetAwaiter().GetResult();
                ViewBag.ProfilePics = users.ImagePath!;
                ViewBag.AdminName = $"{users.FirstName} {users.SurName}";
                ViewBag.UseType = users.AdminType;
                ViewBag.CompanyType = users.CompanyType;
                ViewBag.ManagerId = users.ManagerId;
                ViewBag.CompanyName = users.CompanyName;
                _ = new ProfileData();
                ProfileData? dataModel;
                if (users.CompanyType!.Contains("Logistics") && users.CompanyType!.Contains("Accommodation"))
                {

                    var logistics = LogCompany(users.ManagerId)!.GetAwaiter().GetResult();
                    ViewBag.LogisticId = logistics.Id;
                    var accommodation = AccommodationItems(users.ManagerId)!.GetAwaiter().GetResult();
                    ViewBag.AccommodationId = accommodation.Id;
                    if (logistics == null && accommodation == null && users.AdminType == "Manager")
                    {
                        ViewBag.Logistics = "NotRegistered";
                        ViewBag.Accommodation = "NotRegistered";
                    }

                    dataModel = new ProfileData()
                    {
                        AdminModelTable = users,
                        CompanyModelData = logistics,
                        AccomodationDataModel = accommodation,
                    };
                }
                else if (users.CompanyType!.Contains("Logistics"))
                {
                    var logistics = LogCompany(users.ManagerId)!.GetAwaiter().GetResult();
                    ViewBag.LogisticId = logistics.Id;
                    if (logistics == null && users.AdminType == "Manager")
                    {
                        ViewBag.Logistics = "NotRegistered";
                    }
                    dataModel = new ProfileData()
                    {
                        AdminModelTable = users,
                        CompanyModelData = logistics,
                    };
                }
                else
                {
                    var accommodation = AccommodationItems(users.ManagerId)!.GetAwaiter().GetResult();
                    ViewBag.AccommodationId = accommodation.Id;
                    if (accommodation == null && users.AdminType == "Manager")
                    {
                        ViewBag.Accommodation = "NotRegistered";
                    }
                    dataModel = new ProfileData()
                    {
                        AdminModelTable = users,
                        AccomodationDataModel = accommodation,
                    };

                }

                return View(dataModel);
            }
            else
            {
                return RedirectToAction("SignIn", "Auth");

            }
        }

        public async Task<IActionResult> UserProfile1()
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

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(AddAdminRequest requset)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<string> errors = [];
                    errors.Add(requset.CompanyType!);
                    AddMainAdminRequest login = new()
                    {
                        AdminType = "Manager",
                        FirstName =requset.FirstName,
                        SurName =requset.SurName,
                        StaffCode ="Staff",
                        CompanyName =requset.CompanyName,
                        CompanyUserName =requset.CompanyName,
                        CompanyType=errors,
                        Branch=requset.Branch,
                        Locality="No Locality",
                        State="No State",
                        Address=requset.Address,
                        Email = requset.Email,
                        PhoneNo = requset.PhoneNo,
                        Sex = requset.Sex,
                        Password = requset.Password,
                    };
                    HttpContext.Session.SetString("bt_userPassword", requset.Password!);
                    using var httpClient = new HttpClient();

                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    using var response = await httpClient.PostAsync($"{GlobalConstant.BaseUrl}{GlobalConstant.AdminUrl}add-manager", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode==HttpStatusCode.Created)
                    {
                        var body = JsonConvert.DeserializeObject<AdminModelTable>(apiResponse)!;
                        ViewBag.Result = apiResponse;
                        HttpContext.Session.SetString("bt_userId", body.Id.ToString());
                        HttpContext.Session.SetString("bt_userType", body.AdminType!);
                        HttpContext.Session.SetString("bt_userEmail", body.Email!);
                        HttpContext.Session.SetString("bt_userState", body.State!);
                        return RedirectToAction("SignIn", "Auth");
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
                        return RedirectToAction("SignIn", "Auth");
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
