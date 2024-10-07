using Common.Interfaces;
using Common.Messages;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.EventHandlers
{
    public class OrderCreatedHandler
    {
        private IPaymentService _paymentService;
        private IProductService _productService;

        public OrderCreatedHandler(IPaymentService paymentService, IProductService productService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _productService = productService;
        }

        public async Task HandleOrderCreated(OrderCreatedMessage orderCreatedMessage)
        {
            
            await _paymentService.CreateAsync(orderCreatedMessage.Order.ID);
            await _productService.UpdateQuantityBySkuAsync(orderCreatedMessage.Order.SKU, orderCreatedMessage.Order.ID);
        }
    }
}
