using System.ComponentModel.DataAnnotations;

namespace EvelynStores.Core.DTOs;

public class UserDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    public int UserType { get; set; }

    public bool Status { get; set; }

    public DateTime CreatedAt { get; set; }
}
