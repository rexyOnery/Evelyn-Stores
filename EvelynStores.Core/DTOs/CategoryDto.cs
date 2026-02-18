using System;
using System.ComponentModel.DataAnnotations;

namespace EvelynStores.Core.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, ErrorMessage = "Name must be at most {1} characters long.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Slug must be at most {1} characters long.")]
    public string Slug { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}