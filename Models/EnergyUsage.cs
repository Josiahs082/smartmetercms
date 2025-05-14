using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartmetercms.Models
{
    public class EnergyUsage
    {   [Key]
        public int ID { get; set; } // Primary Key
        
        [ForeignKey("User")]
        public string MeterID { get; set; } = null!; // Foreign Key to Users
        public DateTime Timestamp { get; set; }
        public double EnergyUsed { get; set; } // kWh
        public double Voltage { get; set; } // Optional
        public double Current { get; set; } // Optional
         public User? User { get; set; }  // Navigation property for the related User
    }
}