using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartmetercms.Models
{
    public class IntervalEnergyUsage
    {   
        public int Id { get; set; } // Primary Key
        [Required]
        [ForeignKey("User")]
        public string? MeterID { get; set; } // Foreign Key to identify the smart meter
        [Required]
        public DateTime Timestamp { get; set; } // Time of the interval
        
        
        public User? User { get; set; }  // Navigation property for related User
        public double EnergyUsed { get; set; } // Energy used during the interval (kWh)
    }
}