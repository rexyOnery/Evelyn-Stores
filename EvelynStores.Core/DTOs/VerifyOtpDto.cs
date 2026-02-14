using System.ComponentModel.DataAnnotations;

namespace EvelynStores.Core.DTOs;

public class VerifyOtpDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP code is required")]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "OTP must be 4 digits")]
    public string OtpCode { get; set; } = string.Empty;
}
