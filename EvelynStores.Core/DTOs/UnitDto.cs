using System;
using System.ComponentModel.DataAnnotations;

namespace EvelynStores.Core.DTOs;

public class UnitDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, ErrorMessage = "Name must be at most {1} characters long.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Short name is required.")]
    [StringLength(50, ErrorMessage = "Short name must be at most {1} characters long.")]
    public string ShortName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue, ErrorMessage = "NoOfProducts must be zero or a positive number.")]
    public int NoOfProducts { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
