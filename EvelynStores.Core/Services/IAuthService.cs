using EvelynStores.Core.DTOs;

namespace EvelynStores.Core.Services;

public interface IAuthService
{
    Task<EvelynPhilApiResponse> RegisterAsync(RegisterDto registerDto);
    Task<EvelynPhilApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    Task<bool> EmailExistsAsync(string email);
    Task<EvelynPhilApiResponse> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
}
