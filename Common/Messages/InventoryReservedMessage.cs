using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messages
{
    public class InventoryReservedMessage : IMessage
    {
        public InventoryReservedMessage(int orderId, Dictionary<string, decimal> stringPriceDictionary)
        {
            OrderId = orderId;
            StringPriceDictionary = stringPriceDictionary;
        }
        public int OrderId { get; }
        public Dictionary<string, decimal> StringPriceDictionary { get; }  
        public string MessageType => "InventoryReserved";
    }
}