namespace EvelynStores.Core.DTOs;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int UserType { get; set; }
    public string Email { get; set; } = string.Empty;
}
