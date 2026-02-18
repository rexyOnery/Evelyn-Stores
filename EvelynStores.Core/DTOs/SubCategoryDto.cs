using System;
using System.ComponentModel.DataAnnotations;

namespace EvelynStores.Core.DTOs;

public class SubCategoryDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    [StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty; 
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
