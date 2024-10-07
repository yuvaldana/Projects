using Common.Interfaces;
using Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.EventHandlers
{
    public class PaymentAttemptHandler
    {
        private IPaymentService _paymentService;
        private IOrderService _orderService;

        public PaymentAttemptHandler(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        public async Task HandlePaymentAttempt(PaymentAttemptMessage paymentAttemptMessage)
        {
            await _orderService.UpdateStatusByPayment(paymentAttemptMessage.OrderId, paymentAttemptMessage.PaymentStatus);
        }
    }
}
