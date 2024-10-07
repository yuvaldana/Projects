using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messages
{
    public class OrderCreatedMessage : IMessage
    {
        public OrderCreatedMessage(OrderModel order)
        {
            Order = order;
        }
        public OrderModel Order { get; }
        public string MessageType => "OrderCreated";
    }
}
