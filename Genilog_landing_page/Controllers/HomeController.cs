using Genilog_landing_page.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
