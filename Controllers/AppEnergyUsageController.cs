using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartmetercms.Data;
using smartmetercms.Models;

namespace smartmetercms.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppEnergyUsageController : Controller
    {
        private readonly smartmetercmsContext _context;

        public AppEnergyUsageController(smartmetercmsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddEnergyUsage([FromBody] AppEnergyUsageRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                // Step 1: Create a new entry in IntervalEnergyUsage
                var intervalEnergyUsage = new IntervalEnergyUsage
                {
                    Timestamp = request.TimestampInterval,
                    EnergyUsed = request.IntervalEnergy,
                    MeterID = request.MeterID // No need for ToString(), MeterID is already a string
                    
                };

                _context.IntervalEnergyUsage.Add(intervalEnergyUsage);

                // Step 2: Update the cumulative energy for the specified MeterID
                var cumulativeEnergy = await _context.EnergyUsage
                    .FirstOrDefaultAsync(e => e.MeterID == request.MeterID);

                if (cumulativeEnergy != null)
                {
                    // Update the existing cumulative energy record
                    cumulativeEnergy.EnergyUsed = request.CumulativeEnergy;
                    cumulativeEnergy.Timestamp = DateTime.UtcNow; // Update timestamp
                }
                else
                {
                    // Create a new cumulative energy record if it doesn't exist
                    var newCumulativeEnergy = new EnergyUsage
                    {
                        MeterID = request.MeterID,
                        EnergyUsed = request.IntervalEnergy, // Start with the energy used during this interval
                        Timestamp = DateTime.UtcNow
                    };

                    _context.EnergyUsage.Add(newCumulativeEnergy);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Data added and cumulative energy updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // Define the request model
    public class AppEnergyUsageRequest
    {
        public required string MeterID { get; set; } // ID of the smart meter
        public double IntervalEnergy { get; set; } // Energy used during the interval
        public DateTime TimestampInterval { get; set; } // Time of the interval
        public double CumulativeEnergy { get; set; } // Total energy used (for future use if needed)
    }
}
