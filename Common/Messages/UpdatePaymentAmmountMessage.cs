using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messages
{
    public class UpdatePaymentAmmountMessage : IMessage
    {
        public UpdatePaymentAmmountMessage(int orderId, int ammount)
        {
            OrderId = orderId;
            Ammount = ammount;
        }
        public int OrderId { get; }
        public int Ammount { get; }
        public string MessageType => "UpdatePaymentAmmountMessage";
    }
}
