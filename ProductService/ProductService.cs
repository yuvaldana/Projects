using Common.Exeptions;
using Common.Interfaces;
using Common.Models;
using System.Net.Mail;
using System.Net;
using Common.Messages;
using System.Security.Policy;

namespace ProductService
{
    public class ProductService : IProductService
    {
        private IProductRepository _context;
        private IPublisher _publisher;
        public ProductService(IProductRepository context, IPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }
        public async Task AddAsync(ProductModel productAdd)
        {
            try
            {
                await _context.AddToDBAsync(productAdd);
            }
            catch
            {
                throw new SavingOrUpdatingDBException();
            }
        }
        public async Task DeleteAsync(ProductModel productRemove)
        {
            try
            {
                await _context.RemoveFromDBAsync(productRemove.ID);
            }
            catch
            {
                throw new SavingOrUpdatingDBException();
            }
        }
        public async Task UpdateAsync(ProductModel productUpdate)
        {
            try
            {
                var existingProduct = await _context.GetProductByIDAsync(productUpdate.ID);

                if (existingProduct != null)
                {
                    existingProduct.LastModified = DateTime.Now;

                    if (productUpdate.Name != null)
                        existingProduct.Name = productUpdate.Name;

                    if (productUpdate.ProductImageURL != null)
                        existingProduct.ProductImageURL = productUpdate.ProductImageURL;

                    if (productUpdate.Description != null)
                        existingProduct.Description = productUpdate.Description;

                    if (productUpdate.Price != null)
                        existingProduct.Price = productUpdate.Price;

                    if (productUpdate.StockQuantity != null)
                    {
                        // If new Quantity is set directly
                        existingProduct.StockQuantity = productUpdate.StockQuantity;
                    }

                    if (productUpdate.LastStockQuantityChangeValue != null)
                    {
                        // If new Quantity is set via the number of items to Add or Remove
                        if (productUpdate.LastStockQuantityChangeValue + existingProduct.StockQuantity >= 0)
                        {
                            existingProduct.StockQuantity = productUpdate.LastStockQuantityChangeValue + existingProduct.StockQuantity;
                        }
                        // In Case of Error, disable Product by setting StockQuantity to zero, and send a message of the last known stock. Admin will fix manually by updating again with the correct value.
                        else
                        {
                            existingProduct.StockQuantity = 0;
                            // SendQuantityUpdateErrorMessage(existingProduct, productUpdate.LastStockQuantityChangeValue.Value);
                        }
                    }

                    if (productUpdate.ProductCategory != null)
                    {
                        existingProduct.ProductCategory = productUpdate.ProductCategory;

                        if (productUpdate.ProductCategory.CategoryId != null)
                        {
                            existingProduct.ProductCategory.CategoryId = productUpdate.ProductCategory.CategoryId;
                        }
                    }

                    await _context.UpdateToDBAsync(existingProduct);
                }
            }
            catch
            {
                throw new SavingOrUpdatingDBException();
            }
        }

        // The next method is used to reserve products by reding skus from order
        public async Task UpdateQuantityBySkuAsync(List<string> skuList, int orderId)
        {
            try
            {
                Dictionary<string, int> skuKeyAmmountValue = CreateStringCountDictionary(skuList);
                bool sufficietnQuantity = true;
                foreach (var pair in skuKeyAmmountValue)
                {
                    var product = await _context.GetProductBySKUAsync(pair.Key);
                    if (pair.Value > product.StockQuantity)
                        sufficietnQuantity = false;
                }
                if (sufficietnQuantity)
                {
                    await _context.UpdateQuantityToDBBySkuAsync(skuKeyAmmountValue);
                    Dictionary<string, decimal> skuPriceDictionary = await CreateStringPriceDictionary(skuKeyAmmountValue);
                    await _publisher.Publish(new InventoryReservedMessage(orderId, skuPriceDictionary));
                }
                else
                {
                    await _publisher.Publish(new InventoryResravationRejectedMessage(orderId));
                }
            }
            catch
            {
                throw new SavingOrUpdatingDBException();
            }
        }
        // The next method is for updating order total ammount, this method creates relevant product data for order service to use later
        private async Task<Dictionary<string, decimal>> CreateStringPriceDictionary(Dictionary<string, int> skuKeyAmmountValue)
        {
            Dictionary<string, decimal> stringPriceDictionary = new Dictionary<string, decimal>();
            foreach (var pair in skuKeyAmmountValue)
            {
                var product = await _context.GetProductBySKUAsync(pair.Key);
                stringPriceDictionary[pair.Key] = product.Price*pair.Value;
            }
            return stringPriceDictionary;
        }
        private Dictionary<string, int> CreateStringCountDictionary(List<string> stringList)
        {
            Dictionary<string, int> stringCountDictionary = new Dictionary<string, int>();

            foreach (string str in stringList)
            {

                if (stringCountDictionary.ContainsKey(str))
                    stringCountDictionary[str]++;
                else
                    stringCountDictionary[str] = 1;
            }
            return stringCountDictionary;
        }

    }
}