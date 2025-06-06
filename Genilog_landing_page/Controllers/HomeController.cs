using Genilog_landing_page.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Genilog_landing_page.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;

        public IActionResult Index()
        {
            return View();
        } 
        public IActionResult AboutUs()
        {
            return View();
        }  
        public IActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(FeedbackModel requset)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    FeedbackModel login = new()
                    {
                        Email = requset.Email,
                        Name = requset.Name,
                        PhoneNo = requset.PhoneNo,
                        Feedback = requset.Feedback,
                    };
                    using var httpClient = new HttpClient();
                    StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                    using var response = await httpClient.PostAsync("https://api-data.ginilog.com/api/Info/feedback", content);
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        ViewBag.Result = apiResponse;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
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
        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
