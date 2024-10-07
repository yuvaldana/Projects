using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IProductService
    {
        Task AddAsync(ProductModel productAdd);
        Task DeleteAsync(ProductModel productRemove);
        Task UpdateAsync(ProductModel productUpdate);
        Task UpdateQuantityBySkuAsync(List<string> skus, int orderId);

    }
}
