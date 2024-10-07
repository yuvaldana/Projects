using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ShippingModel
    {
        public int ShippingDetailsId { get; set; }
        public DeliveryMethod? deliveryMethod { get; set; }
        public string? ShippingAddress { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string DeliveryCompany { get; set; }
        public decimal DeliveryAmount { get; set; }
    }
    public enum DeliveryMethod
    {
        Delivery = 1,
        Warehouse_Picking = 2
    }
}
