using EvelynStores.Core.DTOs;

namespace EvelynStores.Core.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> UpdateAsync(Guid id, UserDto dto);
    Task<bool> DeleteAsync(Guid id);
}
