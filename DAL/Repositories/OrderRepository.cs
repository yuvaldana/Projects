using Common.Models;
using Common.Interfaces;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private OrderDBContext _context;
        public OrderRepository(OrderDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderModel>> GetOrderAsync()
        {
            return await _context.Orders.ToListAsync();
        }
        public async Task<OrderModel?> GetOrderByIDAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }
        public async Task AddToDBAsync(OrderModel order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateToDBAsync(OrderModel orderUpdate)
        {
            _context.Orders.Update(orderUpdate);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveFromDBAsync(int orderId)
        {
            var orderToRemove = await GetOrderByIDAsync(orderId);
            if (orderToRemove != null)
            {
                _context.Orders.Remove(orderToRemove);
                await _context.SaveChangesAsync();
            }
        }
    }
}
