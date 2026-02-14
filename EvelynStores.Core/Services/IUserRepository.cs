using EvelynStores.Core.Entities;

namespace EvelynStores.Core.Services;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task UpdateAsync(User user);
}
