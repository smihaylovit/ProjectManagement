using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Web.Data;
using ProjectManagement.Web.Models;

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
        //[ResponseCache(Duration = 5 * 60, VaryByQueryKeys = ["*"])]
        public async Task<IActionResult> Get(DateOnly? fromDate, DateOnly? toDate, int pageNumber)
        {
            if (fromDate == null)
            {
                fromDate = DateOnly.MinValue;
            }

            if (toDate == null)
            {
                toDate = DateOnly.MaxValue;
            }

            var users = await
                DbContext.Users
                .Include(u => u.TimeLogs)
                .Where(u => u.TimeLogs.Any(tl => tl.Date >= fromDate & tl.Date <= toDate))
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Email = u.Email
                })
                .OrderBy(u => u.Id)
                .ToListAsync();

            var numberOfPages =
                users.Count / GlobalConstants.UsersPageSize +
                (users.Count % GlobalConstants.UsersPageSize == 0 ? 0 : 1);

            if (pageNumber < 1 || pageNumber > numberOfPages)
            {
                pageNumber = 1;
            }

            users =
                users
                .Skip((pageNumber - 1) * GlobalConstants.UsersPageSize)
                .Take(GlobalConstants.UsersPageSize)
                .ToList();

            return Json(new { Users = users, Pages = numberOfPages, SelectedPage = pageNumber });
        }

        [HttpGet]
        public async Task<IActionResult> GetPerformance(DateOnly? fromDate, DateOnly? toDate, int projectId)
        {
            if (fromDate == null)
            {
                fromDate = DateOnly.MinValue;
            }

            if (toDate == null)
            {
                toDate = DateOnly.MaxValue;
            }

            var projects = await
                DbContext.Projects
                .Include(p => p.TimeLogs)
                .Where(p => p.TimeLogs.Any(tl => tl.Date >= fromDate & tl.Date <= toDate))
                .Select(p => new ProjectViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .OrderBy(p => p.Id)
                .ToListAsync();

            if (projectId < 1 || projectId > projects.Count)
            {
                projectId = 0;
            }

            IEnumerable<ChartViewModel> chartData;

            if (projectId == 0)
            {
                chartData = await
                    (from timeLog in DbContext.TimeLogs
                     group timeLog by timeLog.UserId into tlg
                     select new ChartViewModel
                     {
                         Email = DbContext.Users.First(u => u.Id == tlg.Key).Email,
                         Hours = tlg
                            .Where(tl => tl.Date >= fromDate && tl.Date <= toDate)
                            .Sum(tl => tl.Hours)
                     })
                    .ToListAsync();
            }
            else
            {
                chartData = await
                    (from timeLog in DbContext.TimeLogs
                     group timeLog by new { timeLog.UserId, timeLog.ProjectId } into tlg
                     select new ChartViewModel
                     {
                         Email = DbContext.Users.First(u => u.Id == tlg.Key.UserId).Email,
                         Hours = tlg
                            .Where(tl =>
                               tl.Date >= fromDate && tl.Date <= toDate &&
                               tl.ProjectId == projectId)
                            .Sum(tl => tl.Hours)
                     })
                    .ToListAsync();
            }

            chartData =
                chartData
                .OrderByDescending(u => u.Hours)
                .Take(GlobalConstants.UsersCountForPerformanceChart)
                .ToList();

            return Json(new
            { 
                ChartData = chartData,
                Projects = projects,
                SelectedProjectId = projectId,
                SelectedProjectName = projectId == 0 ? "All Projects" : projects[projectId - 1].Name
            });
        }
    }
}
