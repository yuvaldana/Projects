using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class OrderDBContext : DbContext
    {
        public OrderDBContext(DbContextOptions<OrderDBContext> options) : base(options)
        {
        }

        public DbSet<OrderModel> Orders { get; set; }
    }
}
