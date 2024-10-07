using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messages
{
    public class PaymentAttemptMessage : IMessage
    {
        public PaymentAttemptMessage(int orderId, PaymentStatus paymentStatus)
        {
            OrderId = orderId;
            PaymentStatus = paymentStatus;
        }
        public int OrderId { get; }
        public PaymentStatus PaymentStatus { get; }
        public string MessageType => "PaymentAttempt";
    }
}
