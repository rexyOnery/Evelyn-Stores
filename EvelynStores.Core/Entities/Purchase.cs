using System;

namespace EvelynStores.Core.Entities;

public class Purchase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SKU { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Guid SubCategoryId { get; set; }
    public Guid ProductId { get; set; }
    public double UnitCost { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ManufacturedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
