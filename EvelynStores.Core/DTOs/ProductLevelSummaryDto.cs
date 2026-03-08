namespace EvelynStores.Core.DTOs
{
    public class ProductLevelSummaryDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int InStockQuantity { get; set; }
    }
}
