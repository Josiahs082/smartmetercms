using System;
using System.ComponentModel.DataAnnotations;

namespace smartmetercms.Models
{
    public class MeterStatus
    {
        [Key]
        [Required]
        public string? MeterID { get; set; }
        public string? Status { get; set; } // "Connected" or "Disconnected"
        public DateTime LastUpdated { get; set; }
    }
}