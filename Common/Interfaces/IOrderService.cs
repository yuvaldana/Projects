using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IOrderService
    {
        Task AddAsync(OrderModel orderAdd);
        Task UpdateAsync(OrderModel orderUpdate);
        Task DeleteAsync(OrderModel orderRemove);
        Task UpdateToPending(int orderId);
        Task UpdateToInsufficientProducts(int orderId);
        Task UpdateStatusByPayment(int orderId, PaymentStatus paymentStatus);
        Task UpdateTotalPrice(int orderId, Dictionary<string, decimal> stringPriceDictionary);
        Task UpdateToPaid(int orderId);
    }
}
