namespace EvelynStores.Core.Models;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 120;
    public int SlidingExpirationMinutes { get; set; } = 60;
}
