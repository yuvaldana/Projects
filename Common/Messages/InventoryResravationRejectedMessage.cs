using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messages
{
    public class InventoryResravationRejectedMessage : IMessage
    {
        public InventoryResravationRejectedMessage(int orderId)
        {
            OrderId = orderId;
        }
        public int OrderId { get; }
        public string MessageType => "InventoryResravationRejected";
    }
}
