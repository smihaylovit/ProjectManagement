using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Web.Data;
using ProjectManagement.Web.Models;
using ProjectManagement.Web.Models.Entities;
using System.Collections;

namespace ProjectManagement.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext DbContext;
        private readonly ILogger<UsersController> Logger;

        public UsersController(ApplicationDbContext dbContext, ILogger<UsersController> logger)
        {
            DbContext = dbContext;
            Logger = logger;
        }

        [HttpGet]
        public IActionResult Get(DateOnly fromDate, DateOnly toDate, int pageNumber)
        {
            var cursor = (pageNumber - 1) * GlobalConstants.UsersPageSize;
            var users =
                (from user in DbContext.Users
                 join timeLog in DbContext.TimeLogs on user.Id equals timeLog.UserId
                 where user.Id >= cursor + 1 && user.Id <= cursor + GlobalConstants.UsersPageSize &&
                       timeLog.Date >= fromDate && timeLog.Date <= toDate
                 orderby user.Id
                 select new UserViewModel
                 {
                     Id = user.Id,
                     Email = user.Email
                 })
                 .Distinct()
                 .ToList();

            return Json(users);
        }

        [HttpGet]
        public IActionResult GetBarChart(DateOnly fromDate, DateOnly toDate, string userOrProject)
        {
            var chartData =
                (from timeLog in DbContext.TimeLogs
                 group timeLog by timeLog.UserId into tlg
                 select new ChartViewModel
                 {
                     Email = DbContext.Users.Single(u => u.Id == tlg.Key).Email,
                     Hours = tlg.Where(tl => tl.Date >= fromDate && tl.Date <= toDate).Sum(tl => tl.Hours)
                 })
                 .OrderByDescending(u => u.Hours)
                 .Take(10)
                 .ToList();

            return Json(chartData);
        }
    }
}
