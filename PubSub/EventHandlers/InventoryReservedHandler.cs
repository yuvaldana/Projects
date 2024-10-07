using Common.Interfaces;
using Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.EventHandlers
{
    public class InventoryReservedHandler
    {
        private IOrderService _orderService;
        public InventoryReservedHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task HandleInventoryReserved(InventoryReservedMessage inventoryReservedMessage)
        {
            await _orderService.UpdateToPending(inventoryReservedMessage.OrderId);
            await _orderService.UpdateTotalPrice(inventoryReservedMessage.OrderId, inventoryReservedMessage.StringPriceDictionary);
        }
    }
}
