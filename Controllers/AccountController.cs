using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using smartmetercms.Data;

namespace smartmetercms.Controllers
{
    public class AccountController : Controller
    {
        private readonly smartmetercmsContext _context;

        public AccountController(smartmetercmsContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string meterID, string password)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.MeterID == meterID && u.Password == password);

            if (user != null && user.MeterID != null)
            {
                HttpContext.Session.SetString("MeterID", user.MeterID);
                HttpContext.Session.SetString("Role", user.Role ?? "");

                if (user.Role == "Admin")
                    return RedirectToAction("AdminDashboard", "Home");
                else if (user.Role == "Customer")
                    return RedirectToAction("CustomerDashboard", "Home");
            }

            ViewBag.ErrorMessage = "Invalid MeterID or Password";
            return View();
        }
    }
}
