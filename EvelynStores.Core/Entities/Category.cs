using System;

namespace EvelynStores.Core.Entities;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Optional image URL or base64 payload for category image
    public string ImageUrl { get; set; } = string.Empty;
        // computed number of products in this category (populated by repository/service)
        public int ProductCount { get; set; }
}