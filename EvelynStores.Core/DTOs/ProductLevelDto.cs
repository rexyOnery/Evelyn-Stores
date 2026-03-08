using System;
using System.Collections.Generic;
using System.Text;

namespace EvelynStores.Core.DTOs
{
    public class ProductLevelDto
    { 
        public Guid Id { get; set; }
        public Guid ProductId {  get; set; }
        public Guid CategoryId { get; set; } = Guid.Empty;
        public int PurchaseQuantity { get; set; } 
         public int InStockQuantity { get; set; }
         public int ReOrderLevel { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        // Product navigation fields (populated when returning product-levels with product info)
        public string ProductName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public int ProductQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
