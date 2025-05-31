using System;
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
                _logger.LogInformation($"Fetching meter statuses at {DateTime.Now}");

                var users = await _context.User
                    .Include(u => u.Bills)
                    .ToListAsync();

                _logger.LogInformation($"Found {users.Count} users.");

                var statuses = await _context.MeterStatus.ToListAsync();
                _logger.LogInformation($"Found {statuses.Count} meter statuses.");

                var result = users
                    .Where(u => u.MeterID != "admin") // Exclude admin user
                    .Select(u => new
                    {
                        MeterID = u.MeterID,
                        Status = (statuses.FirstOrDefault(s => s.MeterID == u.MeterID) ?? 
                                 new MeterStatus { MeterID = u.MeterID, Status = "Connected", LastUpdated = DateTime.Now }).Status,
                        UnpaidAmount = CalculateUnpaidAmount(u, statuses)
                    })
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching meter statuses at {DateTime.Now}");
                return StatusCode(500, "An error occurred while fetching meter statuses.");
            }
        }

        private float CalculateUnpaidAmount(User user, List<MeterStatus> statuses)
        {
            var energyUsage = _context.EnergyUsage
                .Where(e => e.MeterID == user.MeterID)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefault();
            float cumulativeEnergy = energyUsage != null ? (float)energyUsage.EnergyUsed : 0.0f; // Explicit cast and null check

            const float ratePerKWh = 47.6f; // Adjustable rate ($0.12/kWh)
            float totalCost = cumulativeEnergy * ratePerKWh;

            float totalPayments = user.Bills
                .Where(b => b.PaidStatus) // Use PaidStatus as bool
                .Sum(b => (float)b.AmountDue); // Explicit cast for AmountDue (assuming double)
            float unpaidAmount = totalCost - totalPayments;
            if (unpaidAmount < 0) unpaidAmount = 0.0f;

            _logger.LogInformation($"MeterID: {user.MeterID}, Status: {(statuses.FirstOrDefault(s => s.MeterID == user.MeterID)?.Status ?? "Connected")}, " +
                                $"CumulativeEnergy: {cumulativeEnergy}, TotalCost: {totalCost}, TotalPayments: {totalPayments}, UnpaidAmount: {unpaidAmount}");
            return unpaidAmount;
        }
    }
}