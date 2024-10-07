using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class PaymentModel
    {
        [Key]
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public int PaymentConfimNumer { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Description { get; set; }
        public int TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string ClientSecret { get; set; } = null!;
        public string Currency { get; set; } = null!;
    }
    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Confirmed = 3,
        Declined = 4,
        Cancelled = 5
    }
}
