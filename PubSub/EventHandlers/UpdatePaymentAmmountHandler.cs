using Common.Interfaces;
using Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.EventHandlers
{
    public class UpdatePaymentAmmountHandler
    {
        private IPaymentService _paymentService;
        public UpdatePaymentAmmountHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task HandlePaymentAmmountUpdate(UpdatePaymentAmmountMessage updatePaymentAmmountMessage)
        {
            await _paymentService.UpdateAmmount(updatePaymentAmmountMessage.OrderId, updatePaymentAmmountMessage.Ammount);
        }
    }
}
