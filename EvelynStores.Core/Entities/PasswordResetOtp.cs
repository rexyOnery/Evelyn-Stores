namespace EvelynStores.Core.Entities;

public class PasswordResetOtp
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string OtpCode { get; set; } = string.Empty;
    public string? ResetToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
