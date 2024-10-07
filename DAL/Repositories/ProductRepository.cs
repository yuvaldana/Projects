using Common.Exeptions;
using Common.Interfaces;
using Common.Models;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private ProductDBContext _context;
        public ProductRepository(ProductDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductModel>> GetProductAsync()
        {
            return await _context.Products.ToListAsync();
        }


        public async Task<ProductModel?> GetProductByIDAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        public async Task<ProductModel> GetProductBySKUAsync(string sku)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(p => p.SKU == sku);

            if (product == null)
            {
                throw new NoProductException();
            }
            return product;
        }
        public async Task AddToDBAsync(ProductModel product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveFromDBAsync(int productId)
        {
            var productToRemove = await GetProductByIDAsync(productId);
            if (productToRemove != null)
            {
                _context.Products.Remove(productToRemove);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateToDBAsync(ProductModel product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateQuantityToDBBySkuAsync(Dictionary<string, int> skuKeyAmountValue)
        {
            var productsToUpdate = await _context.Products
                .Where(p => skuKeyAmountValue.ContainsKey(p.SKU))
                .ToListAsync();

            foreach (var product in productsToUpdate)
            {
                if (skuKeyAmountValue.TryGetValue(product.SKU, out int amount))
                {
                    product.StockQuantity -= amount;
                    // Prevent negative quantities, Fail Safe to QuantitySufficient in Product service
                    if (product.StockQuantity < 0)
                    {
                        product.StockQuantity = 0;
                        // Send message to admin with sku
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

    }
}