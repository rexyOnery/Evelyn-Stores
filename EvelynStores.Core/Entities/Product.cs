using System;

namespace EvelynStores.Core.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Guid SubCategoryId { get; set; }
    public Guid BrandId { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    // store image as base64 data url string
    public string ImageUrl { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ManufacturedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
