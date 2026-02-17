using EvelynStores.Core.Entities;

namespace EvelynStores.Core.Services;

public interface IUnitRepository
{
    Task<List<Unit>> GetAllAsync();
    Task<Unit?> GetByIdAsync(Guid id);
    Task AddAsync(Unit unit);
    Task UpdateAsync(Unit unit);
    Task DeleteAsync(Guid id);
}
