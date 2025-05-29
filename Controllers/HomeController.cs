using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using smartmetercms.Data;
using smartmetercms.Models;

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

        public async Task<IActionResult> AdminDashboard()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var powerQualityData = await _context.PowerQuality
                .Where(pq => pq.Timestamp >= DateTime.Now.AddDays(-7))
                .ToListAsync();

            return View(powerQualityData);
        }

        [HttpGet]
        public async Task<IActionResult> LoadDemandChartData(string date, string range)
        {
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Now.Date : DateTime.Parse(date).Date;
            var start = selectedDate;
            var end = selectedDate.AddDays(1);

            var query = _context.PowerQuality
                .Where(pq => pq.Timestamp >= start && pq.Timestamp < end)
                .OrderBy(pq => pq.Timestamp);

            var data = await query
                .GroupBy(pq => new { pq.Timestamp.Year, pq.Timestamp.Month, pq.Timestamp.Day, pq.Timestamp.Hour })
                .Select(g => new
                {
                    Time = $"{g.Key.Year}-{g.Key.Month:02}-{g.Key.Day:02} {g.Key.Hour:02}:00",
                    TotalPower = g.Sum(x => x.InstantaneousPower)
                })
                .ToListAsync();

            if (!data.Any())
            {
                return new JsonResult(new[] { new { Time = selectedDate.ToString("yyyy-MM-dd"), TotalPower = 0.0 } });
            }

            return new JsonResult(data);
        }

        [HttpGet]
        public async Task<IActionResult> ExportLoadDemandCsv(string date, string range = "hour")
        {
            var data = await LoadDemandChartData(date, range) as JsonResult;
            var list = data?.Value as IEnumerable<dynamic>;

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Time,TotalPower");

            foreach (var entry in list!)
            {
                csv.AppendLine($"{entry.Time},{entry.TotalPower}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"LoadDemand_hourly_{DateTime.Now:yyyyMMddHHmmss}.csv");
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