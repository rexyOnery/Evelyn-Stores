using EvelynStores.Core.Entities;

namespace EvelynStores.Core.Services;

public interface IPurchaseRepository
{
    Task<List<Purchase>> GetAllAsync();
    Task<Purchase?> GetByIdAsync(Guid id);
    Task AddAsync(Purchase purchase);
    Task UpdateAsync(Purchase purchase);
    Task DeleteAsync(Guid id);
}
