using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class ProductDBContext : DbContext
    {
        public ProductDBContext(DbContextOptions<ProductDBContext> options) : base(options)
        {
        }

        public DbSet<ProductModel> Products { get; set; }
    }
}
