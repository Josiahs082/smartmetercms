using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using smartmetercms.Data;
using smartmetercms.Models;
using System.Globalization;
namespace smartmetercms.Controllers
{
    public class HomeController : Controller
    {
        private readonly smartmetercmsContext _context;

        public HomeController(smartmetercmsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CustomerDashboard()
        {
            var meterID = HttpContext.Session.GetString("MeterID");
            if (string.IsNullOrEmpty(meterID))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.User
                .Include(u => u.IntervalEnergyUsages)
                .FirstOrDefaultAsync(u => u.MeterID == meterID);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Bills = await _context.Bill
                .Where(b => b.MeterID == meterID)
                .ToListAsync();

            return View(user);
        }

        //admin powerqualit display stuff
       
        [HttpGet]
        public async Task<IActionResult> LoadDemandChartData(string range = "day")
        {
            IQueryable<PowerQuality> data = _context.PowerQuality;

            List<object> grouped = new List<object>();

            if (range == "day")
            {
                grouped = await data
                    .GroupBy(p => new { p.Timestamp.Year, p.Timestamp.Month, p.Timestamp.Day })
                    .Select(g => new
                    {
                        time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        totalPower = g.Sum(p => p.InstantaneousPower)
                    })
                    .OrderBy(g => g.time)
                    .ToListAsync<object>();
            }
            else if (range == "week")
            {
                grouped = await data
                    .GroupBy(p => EF.Functions.DateDiffWeek(DateTime.UnixEpoch, p.Timestamp))
                    .Select(g => new
                    {
                        time = DateTime.UnixEpoch.AddDays(g.Key * 7),
                        totalPower = g.Sum(p => p.InstantaneousPower)
                    })
                    .OrderBy(g => g.time)
                    .ToListAsync<object>();
            }
            else if (range == "month")
            {
                grouped = await data
                    .GroupBy(p => new { p.Timestamp.Year, p.Timestamp.Month })
                    .Select(g => new
                    {
                        time = new DateTime(g.Key.Year, g.Key.Month, 1),
                        totalPower = g.Sum(p => p.InstantaneousPower)
                    })
                    .OrderBy(g => g.time)
                    .ToListAsync<object>();
            }

            return Json(grouped);
        }




        [HttpGet]
        public async Task<IActionResult> ExportLoadDemandCsv(string range = "day")
        {
            var data = await LoadDemandChartData(range) as JsonResult;
            var list = data?.Value as IEnumerable<dynamic>;

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Time,TotalPower");

            foreach (var entry in list!)
            {
                csv.AppendLine($"{entry.Time},{entry.TotalPower}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"LoadDemand_{range}_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }


        


        public IActionResult AdminDashboard()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
