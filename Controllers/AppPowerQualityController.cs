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
    public class AppPowerQualityController : ControllerBase
    {
        private readonly smartmetercmsContext _context;

        public AppPowerQualityController(smartmetercmsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPowerQuality([FromBody] PowerQualityRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var qualityEntry = new PowerQuality
                {
                    MeterID = request.MeterID,
                    PowerFactor = request.PowerFactor,
                    Frequency = request.Frequency,
                    Voltage = request.Voltage,
                    InstantaneousPower = request.InstantaneousPower,
                    Timestamp = request.Timestamp
                };

                _context.PowerQuality.Add(qualityEntry);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Power quality data added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class PowerQualityRequest
    {
        public required string MeterID { get; set; }
        public double PowerFactor { get; set; }
        public double Frequency { get; set; }
        public double Voltage { get; set; }
        public double InstantaneousPower { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
