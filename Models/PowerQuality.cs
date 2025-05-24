using System;
using System.ComponentModel.DataAnnotations;

namespace smartmetercms.Models
{
    public class PowerQuality
    {
        public int ID { get; set; }

        [Required]
        public string? MeterID { get; set; }

        [Required]
        public double PowerFactor { get; set; }

        [Required]
        public double Frequency { get; set; }

        [Required]
        public double Voltage { get; set; }

        [Required]
        public double InstantaneousPower { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
