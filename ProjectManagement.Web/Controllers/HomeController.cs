using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Web.Data;
using ProjectManagement.Web.Data.Initialization;
using ProjectManagement.Web.Models;
using System.Diagnostics;

namespace ProjectManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext DbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime fromDate, DateTime toDate)
        {
            if (fromDate <= toDate && 
                toDate <= DateTime.Today.Date)
            {
                var gen = new DbInitializationSqlGenerator();
                var createSpSql = gen.CreateDbInitializationStoredProcedureSql(fromDate, toDate);
                var execSpSql = gen.CreateDbInitializationStoredProcedureExecutionSql();
                var createSp = await DbContext.Database.ExecuteSqlRawAsync(createSpSql);
                var execSp = await DbContext.Database.ExecuteSqlRawAsync(execSpSql);

                ViewBag.FromDate = DateOnly.FromDateTime(fromDate).ToString("dd-MMM-yyyy");
                ViewBag.ToDate = DateOnly.FromDateTime(toDate).ToString("dd-MMM-yyyy");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
