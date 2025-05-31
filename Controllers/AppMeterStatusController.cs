using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartmetercms.Data;
using smartmetercms.Models;
using Microsoft.Extensions.Logging;

namespace smartmetercms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppMeterStatusController : ControllerBase
    {
        private readonly smartmetercmsContext _context;
        private readonly ILogger<AppMeterStatusController> _logger;

        public AppMeterStatusController(smartmetercmsContext context, ILogger<AppMeterStatusController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMeterStatuses()
        {
            try
            {
                _logger.LogInformation($"Fetching meter statuses at {System.DateTime.Now}.");

                var users = await _context.User
                    .Include(u => u.Bills)
                    .ToListAsync();

                _logger.LogInformation($"Found {users.Count} users.");

                var statuses = await _context.MeterStatus.ToListAsync();

                _logger.LogInformation($"Found {statuses.Count} meter statuses.");

                var result = users
                    .Where(u => u.MeterID != "admin") // Exclude admin user
                    .Select(u =>
                    {
                        var status = statuses.FirstOrDefault(s => s.MeterID == u.MeterID) ?? new MeterStatus { MeterID = u.MeterID, Status = "Connected", LastUpdated = System.DateTime.Now };
                        var unpaidAmount = u.Bills.Where(b => !b.PaidStatus && b.AmountDue > 0).Sum(b => b.AmountDue);
                        _logger.LogInformation($"MeterID: {u.MeterID}, Status: {status.Status}, UnpaidAmount: {unpaidAmount}");
                        return new { MeterID = u.MeterID, Status = status.Status, UnpaidAmount = unpaidAmount };
                    })
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching meter statuses at {System.DateTime.Now}.");
                return StatusCode(500, "An error occurred while fetching meter statuses.");
            }
        }
    }
}