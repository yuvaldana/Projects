using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderModel>> GetOrderAsync();
        Task<OrderModel?> GetOrderByIDAsync(int id);
        Task AddToDBAsync(OrderModel order);
        Task UpdateToDBAsync(OrderModel orderUpdate);
        Task RemoveFromDBAsync(int orderId);
    }
}
