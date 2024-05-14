using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
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
        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = ["pageNumber"])]
        public IActionResult Get(DateOnly fromDate, DateOnly toDate, int pageNumber)
        {
            var users =
                DbContext.Users.Include(u => u.TimeLogs)
                .Where(u => u.TimeLogs.Any(tl => tl.Date >= fromDate & tl.Date <= toDate))
                .OrderBy(u => u.Id)
                .Skip((pageNumber - 1) * GlobalConstants.UsersPageSize)
                .Take(GlobalConstants.UsersPageSize)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Email = u.Email
                })
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
                     Email = DbContext.Users.First(u => u.Id == tlg.Key).Email,
                     Hours = tlg.Where(tl => tl.Date >= fromDate && tl.Date <= toDate).Sum(tl => tl.Hours)
                 })
                 .OrderByDescending(u => u.Hours)
                 .Take(10)
                 .ToList();

            return Json(chartData);
        }
    }
}
