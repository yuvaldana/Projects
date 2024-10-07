using Common.Exeptions;
using Common.Interfaces;
using Common.Messages;
using Common.Models;
using System.Security.Policy;

namespace OrderService
{
    public class OrderService : IOrderService
    {
        private IOrderRepository _context;
        private IPublisher _publisher;
        public OrderService(IOrderRepository context, IPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }
        public async Task AddAsync(OrderModel orderAdd)
        {
            try
            {
                orderAdd.OrderTimeStamp = DateTime.Now;
                orderAdd.LastOrderUpdate = DateTime.Now;
                orderAdd.OrderStatus = OrderStatus.Exists;
                await _context.AddToDBAsync(orderAdd);

                // Publish order created
                await _publisher.Publish(new OrderCreatedMessage(orderAdd));
            }
            catch
            {
                throw new SavingOrUpdatingDBException();
            }
        }
        public async Task UpdateAsync(OrderModel orderUpdate)
        {
            var existingOrder = await _context.GetOrderByIDAsync(orderUpdate.ID);

            if (existingOrder != null)
            {
                try
                {
                    existingOrder.LastOrderUpdate = DateTime.Now;

                    if (orderUpdate.OrderStatus != null)
                        existingOrder.OrderStatus = orderUpdate.OrderStatus;

                    if (orderUpdate.TotalAmount != null)
                    {
                        existingOrder.TotalAmount = orderUpdate.TotalAmount;
                        await _publisher.Publish(new UpdatePaymentAmmountMessage(existingOrder.ID, Convert.ToInt32(existingOrder.TotalAmount*100)));
                    }

                    if (orderUpdate.SKU != null)
                        existingOrder.SKU = orderUpdate.SKU;

                    if (orderUpdate.ShippingDetailsId != null)
                        existingOrder.ShippingDetailsId = orderUpdate.ShippingDetailsId;

                    if (orderUpdate.PaymentDetailsId != null)
                        existingOrder.PaymentDetailsId = orderUpdate.PaymentDetailsId;

                    await _context.UpdateToDBAsync(existingOrder);
                }
                catch
                {
                    throw new SavingOrUpdatingDBException();
                }
            }
        }
        public async Task DeleteAsync(OrderModel orderRemove)
        {
            try
            {
                await _context.RemoveFromDBAsync(orderRemove.ID);
            }
            catch
            {
                throw new SavingOrUpdatingDBException();
            }
        }
        // Update to pending after reserving products for the order
        public async Task UpdateToPending(int orderId)
        {
            try
            {
                OrderModel order = await _context.GetOrderByIDAsync(orderId);
                // Fail Safe
                if (order.OrderStatus != OrderStatus.Paid)
                {
                    order.OrderStatus = OrderStatus.Pending;
                    await UpdateAsync(order);
                }
            }
            catch
            {
                throw new SavingOrUpdatingDBException();
            }
        }
        public async Task UpdateToPaid(int orderId)
        {
            try
            {
                OrderModel orderUpdate = new OrderModel();
                orderUpdate.ID = orderId;
                orderUpdate.OrderStatus = OrderStatus.Paid;
                await UpdateAsync(orderUpdate);
            }
            catch
            {
                throw new SavingOrUpdatingDBException();
            }
        }
        public async Task UpdateToInsufficientProducts(int orderId)
        {
            try
            {
                OrderModel orderUpdate = new OrderModel();
                orderUpdate.ID = orderId;
                orderUpdate.OrderStatus = OrderStatus.InsufficientProducts;
                await UpdateAsync(orderUpdate);
            }
            catch
            {
                throw new SavingOrUpdatingDBException();
            }
        }
        public async Task UpdateStatusByPayment(int orderId, PaymentStatus paymentStatus)
        {
            if (paymentStatus == PaymentStatus.Pending)
                await UpdateToPending(orderId);
            if (paymentStatus == PaymentStatus.Paid)
                await UpdateToPaid(orderId);
        }
        public async Task UpdateTotalPrice(int orderId, Dictionary<string, decimal> stringPriceDictionary)
        {
            decimal totalPrice = 0;
            foreach (var pair in stringPriceDictionary)
                totalPrice = totalPrice + pair.Value;
            OrderModel orderUpdate = new OrderModel();
            orderUpdate.ID = orderId;
            orderUpdate.TotalAmount = totalPrice;
            await UpdateAsync(orderUpdate);
        }
    }
}