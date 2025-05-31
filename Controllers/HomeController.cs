using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using smartmetercms.Data;
using smartmetercms.Models;
using Microsoft.Extensions.Logging;

namespace smartmetercms.Controllers
{
    public class HomeController : Controller
    {
        private readonly smartmetercmsContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(smartmetercmsContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
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

            var users = await _context.User
                .Include(u => u.Bills)
                .ToListAsync();
            var meterStatuses = await _context.MeterStatus.ToListAsync();

            ViewBag.Users = users;
            ViewBag.MeterStatuses = meterStatuses;

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
                    Time = $"{g.Key.Year}-{g.Key.Month:00}-{g.Key.Day:00} {g.Key.Hour:00}:00",
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

        [HttpGet]
        public async Task<IActionResult> GetUnpaidCosts()
        {
            var users = await _context.User
                .Include(u => u.Bills)
                .ToListAsync();

            var unpaidCosts = users.Select(u => new
            {
                MeterID = u.MeterID,
                UnpaidAmount = u.Bills
                    .Where(b => !b.PaidStatus && b.AmountDue > 0)
                    .Sum(b => b.AmountDue)
            }).ToList();

            return Json(unpaidCosts);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleMeterStatuses()
        {
            try
            {
                var users = await _context.User
                    .Include(u => u.Bills)
                    .ToListAsync();

                _logger.LogInformation($"Found {users.Count} users to process at {DateTime.Now}.");

                bool anyChanges = false;
                foreach (var user in users)
                {
                    if (string.IsNullOrEmpty(user.MeterID))
                    {
                        _logger.LogWarning($"User has no MeterID, skipping.");
                        continue;
                    }

                    var hasUnpaidBills = user.Bills.Any(b => !b.PaidStatus && b.AmountDue > 0);
                    _logger.LogInformation($"Processing MeterID: {user.MeterID}, Has Unpaid Bills: {hasUnpaidBills}");

                    var meterStatus = await _context.MeterStatus
                        .FirstOrDefaultAsync(ms => ms.MeterID == user.MeterID);

                    if (meterStatus == null)
                    {
                        meterStatus = new MeterStatus
                        {
                            MeterID = user.MeterID,
                            Status = hasUnpaidBills ? "Disconnected" : "Connected",
                            LastUpdated = DateTime.Now
                        };
                        _context.MeterStatus.Add(meterStatus);
                        _logger.LogInformation($"Created new MeterStatus for MeterID: {user.MeterID}, Status: {meterStatus.Status}");
                        anyChanges = true;
                    }
                    else
                    {
                        var previousStatus = meterStatus.Status;
                        var newStatus = hasUnpaidBills ? "Disconnected" : "Connected";
                        if (meterStatus.Status != newStatus)
                        {
                            meterStatus.Status = newStatus;
                            meterStatus.LastUpdated = DateTime.Now;
                            _context.Entry(meterStatus).State = EntityState.Modified;
                            _logger.LogInformation($"Updated MeterID: {user.MeterID}, Status changed from {previousStatus} to {meterStatus.Status}");
                            anyChanges = true;
                        }
                        else
                        {
                            _logger.LogInformation($"No change needed for MeterID: {user.MeterID}, Status remains: {meterStatus.Status}");
                        }
                    }
                }

                if (anyChanges)
                {
                    var changes = await _context.SaveChangesAsync();
                    _logger.LogInformation($"Saved {changes} changes to the database at {DateTime.Now}.");
                }
                else
                {
                    _logger.LogInformation($"No changes to save at {DateTime.Now}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while toggling meter statuses at {DateTime.Now}.");
                return StatusCode(500, "An error occurred while updating meter statuses. Check logs for details.");
            }

            return RedirectToAction("AdminDashboard");
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