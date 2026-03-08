using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;

namespace EvelynStores.Infrastructure.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            UserType = u.UserType,
            Status = u.Status,
            CreatedAt = u.CreatedAt
        }).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var u = await userRepository.GetByIdAsync(id);
        if (u is null) return null;
        return new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            UserType = u.UserType,
            Status = u.Status,
            CreatedAt = u.CreatedAt
        };
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UserDto dto)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user is null) return null;

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.UserType = dto.UserType;
        user.Status = dto.Status;

        await userRepository.UpdateAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            UserType = user.UserType,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user is null) return false;
        await userRepository.DeleteAsync(id);
        return true;
    }
}
