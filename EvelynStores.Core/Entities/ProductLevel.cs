using System;
using System.Collections.Generic;
using System.Text;

namespace EvelynStores.Core.Entities
{
    public class ProductLevel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public int PurchaseQuantity { get; set; }
        public int InStockQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ReOrderLevel { get; set; }
         public Product Product { get; set; } = null!;
    }
}
