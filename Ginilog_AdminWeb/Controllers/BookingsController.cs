using Microsoft.AspNetCore.Mvc;

namespace Ginilog_AdminWeb.Controllers
{
    public class BookingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
