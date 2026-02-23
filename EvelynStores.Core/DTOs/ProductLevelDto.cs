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
    }
}
