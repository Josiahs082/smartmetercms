using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using smartmetercms.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using smartmetercms.Data; // <-- This is the missing using directive for your DbContext


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

        if (user != null && user.MeterID != null)  // Add null check for MeterID
        {
            // Store user in session or cookie (this is just an example)
            HttpContext.Session.SetString("MeterID", user.MeterID);

            // Redirect based on user role
            if (user.Role == "Admin")
            {
                return RedirectToAction("AdminDashboard", "Home");
            }
            else if (user.Role == "Customer")
            {
                return RedirectToAction("CustomerDashboard", "Home");
            }
        }

        // If login fails, show an error message
        ViewBag.ErrorMessage = "Invalid MeterID or Password";
        return View();
    }
}
}