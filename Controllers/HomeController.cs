using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string meterID, string password)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.MeterID == meterID && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("MeterID", user.MeterID ?? "");
                HttpContext.Session.SetString("Role", user.Role ?? "");

                if (user.Role == "Admin")
                {
                    return RedirectToAction("AdminDashboard");
                }
                return RedirectToAction("CustomerDashboard");
            }

            ViewData["Error"] = "Invalid Meter ID or password.";
            return View();
        }

        public async Task<IActionResult> CustomerDashboard()
        {
            var meterID = HttpContext.Session.GetString("MeterID");
            if (string.IsNullOrEmpty(meterID))
            {
                return RedirectToAction("Login");
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

        public IActionResult AdminDashboard()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login");
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