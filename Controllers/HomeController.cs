using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using smartmetercms.Models;
using smartmetercms.Data;
using Microsoft.EntityFrameworkCore;

namespace smartmetercms.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly smartmetercmsContext _context;

    public HomeController(ILogger<HomeController> logger, smartmetercmsContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult AdminDashboard()
    {
        if (HttpContext.Session.GetString("MeterID") == null || 
            _context.User.FirstOrDefault(u => u.MeterID == HttpContext.Session.GetString("MeterID"))?.Role != "Admin")
        {
            return RedirectToAction("Login", "Account");
        }
        return View();
    }

    public async Task<IActionResult> CustomerDashboard()
    {
        var meterID = HttpContext.Session.GetString("MeterID");
        if (meterID == null || 
            (await _context.User.FirstOrDefaultAsync(u => u.MeterID == meterID))?.Role != "Customer")
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.User
            .Where(u => u.MeterID == meterID)
            .Include(u => u.Bills)
            .Include(u => u.EnergyUsages)
            .Include(u => u.IntervalEnergyUsages)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    public IActionResult Logout()
    {
        // Clear the session
        HttpContext.Session.Clear();
        // Redirect to Index action
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}