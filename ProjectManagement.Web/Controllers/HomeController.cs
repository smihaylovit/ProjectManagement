using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Web.Data;
using ProjectManagement.Web.Models;
using System.Diagnostics;

namespace ProjectManagement.Web.Controllers
{
    public class HomeController : Controller
    {
		private readonly ApplicationDbContext DbContext;
		private readonly ILogger<HomeController> Logger;

		public HomeController(ApplicationDbContext dbContext, ILogger<HomeController> logger)
		{
			DbContext = dbContext;
			Logger = logger;
		}

		public IActionResult Index()
        {
            var viewModel = new HomeIndexViewModel
            {
                MinDate = GlobalConstants.TimeLogMinDate.ToString("yyyy-MM-dd"),
                MaxDate = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
                Projects = DbContext.Projects.Select(p => new ProjectViewModel { Id = p.Id, Name = p.Name }).ToList()
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
