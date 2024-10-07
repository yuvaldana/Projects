using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class OrderModel
    {
        [Key]
        public int ID { get; set; }
        public DateTime OrderTimeStamp { get; set; }
        public DateTime LastOrderUpdate {  get; set; }
        public OrderStatus OrderStatus { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        // For any needed notes
        public string? Description { get; set; }
        [Required]
        public int UserId { get; set; }
        public List<string>? SKU { get; set; }
        public int PaymentDetailsId { get; set; }
        public int ShippingDetailsId { get; set; }

    }
    public enum OrderStatus
    {
        Exists = 0,
        Pending = 1,
        Paid = 2,
        Confirmed = 3,
        Delivered = 4,
        Cancelled = 5,
        InsufficientProducts = 6
    }
}