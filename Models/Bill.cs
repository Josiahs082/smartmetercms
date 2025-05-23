using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartmetercms.Models
{
    public class Bill
    {
        [Key]
        public int ID { get; set; }
        public string MeterID { get; set; } = null!;
        public DateTime BillingPeriodStart { get; set; }
        public DateTime BillingPeriodEnd { get; set; }
        public float TotalEnergyUsed { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999999999999999.99)]
        public decimal AmountDue { get; set; }

        public bool PaidStatus { get; set; }
        public DateTime? PaymentDate { get; set; }

        public List<Payments>? Payments { get; set; }
    }
}