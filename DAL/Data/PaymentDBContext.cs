using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class PaymentDBContext : DbContext
    {
        public PaymentDBContext(DbContextOptions<PaymentDBContext> options) : base(options)
        {
        }

        public DbSet<PaymentModel> Payments { get; set; }
    }
}
