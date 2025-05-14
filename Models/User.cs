using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations;

namespace smartmetercms.Models
{
    public class User
{
    [Key]
    [Required]
    public string? MeterID { get; set; }

    [Required]
    public string? Password { get; set; }

    public string? Role { get; set; } // Admin or Customer 

    // Navigation properties (optional for clarity)
    public List<Bill> Bills { get; set; } = new List<Bill>();  // Bills related to the user
    
    public List<IntervalEnergyUsage> IntervalEnergyUsages { get; set; } = new List<IntervalEnergyUsage>();  // EnergyUsages related to the user
    public List<EnergyUsage> EnergyUsages { get; set; } = new List<EnergyUsage>();  // EnergyUsages related to the user
}

}