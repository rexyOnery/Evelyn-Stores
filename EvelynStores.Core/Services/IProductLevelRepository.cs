using EvelynStores.Core.Entities;

namespace EvelynStores.Core.Services;

public interface IProductLevelRepository
{
    Task<List<ProductLevel>> GetAllAsync();
    Task<ProductLevel?> GetByIdAsync(Guid id);
    Task<ProductLevel?> GetByProductIdAsync(Guid productId);
    Task AddAsync(ProductLevel level);
    Task UpdateAsync(ProductLevel level);
    Task DeleteAsync(Guid id);
}
