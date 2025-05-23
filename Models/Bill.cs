using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartmetercms.Models
{
    public class Bill
    {
        [Key]
        public int ID { get; set; } // Primary Key
        [ForeignKey("User")]
        public string MeterID { get; set; } = null!; // Foreign Key to Users
        public DateTime BillingPeriodStart { get; set; }
        public DateTime BillingPeriodEnd { get; set; }
        public double TotalEnergyUsed { get; set; } // kWh
        public decimal AmountDue { get; set; }
        public bool PaidStatus { get; set; }
        public DateTime? PaymentDate { get; set; } // Nullable if not paid

        public User? User { get; set; } // Navigation property for related User
        public List<Payments> Payments { get; set; } = new List<Payments>(); // Navigation property for related Payments
    }
}