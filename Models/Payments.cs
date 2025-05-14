using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartmetercms.Models
{
    public class Payments
    {   [Key]
        public int ID { get; set; } // Primary Key
        public int BillID { get; set; } // Foreign Key to Bills
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = null!; // e.g., Credit Card
    }
}