using System;
using System.ComponentModel.DataAnnotations;

namespace EvelynStores.Core.DTOs;

public class ProductDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; } = string.Empty;

    //[Required]
    //[StringLength(100)]
    //public string SKU { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public Guid SubCategoryId { get; set; }
    public Guid BrandId { get; set; }
    //public int ReOrderLevel { get; set; }
    public string Unit { get; set; } = string.Empty;

    //[Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
    public int Quantity { get; set; }

    //[Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
    public decimal Price { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Persisted custom fields
    //public DateTime? ManufacturedDate { get; set; }
    //public DateTime? ExpiryDate { get; set; }
}
