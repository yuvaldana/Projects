using Common.Interfaces;
using Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.EventHandlers
{
    public class InventoryResravationRejectedHandler
    {
        private IOrderService _orderService;
        public InventoryResravationRejectedHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public async Task HandleInventoryReserved(InventoryResravationRejectedMessage inventoryResravationRejectedMessage)
        {
            await _orderService.UpdateToInsufficientProducts(inventoryResravationRejectedMessage.OrderId);
        }
    }
}
