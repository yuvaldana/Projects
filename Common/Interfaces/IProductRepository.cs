using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductModel>> GetProductAsync();
        Task<ProductModel?> GetProductByIDAsync(int id);
        Task<ProductModel> GetProductBySKUAsync(string sku);
        Task AddToDBAsync(ProductModel product);
        Task RemoveFromDBAsync(int productId);
        Task UpdateToDBAsync(ProductModel product);
        Task UpdateQuantityToDBBySkuAsync(Dictionary<string, int> skuKeyAmountValue);
    }
}
