using Microsoft.VisualBasic;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class ProductModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string SKU { get; set; } = null!;
        public string? ProductImageURL { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int LastStockQuantityChangeValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }

        public ProductCategory ProductCategory { get; set; } = null!;
    }

    public class ProductCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
