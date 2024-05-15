using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Web.Models;
using System.Diagnostics;

namespace ProjectManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> Logger;

        public HomeController(ILogger<HomeController> logger)
        {
            Logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.MinDate = GlobalConstants.TimeLogMinDate.ToString("yyyy-MM-dd");
            ViewBag.MaxDate = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
