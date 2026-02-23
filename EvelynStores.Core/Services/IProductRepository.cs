using EvelynStores.Core.Entities;

namespace EvelynStores.Core.Services;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetBySubCategoryAsync(Guid subCategoryId);
    Task<Product?> GetByIdAsync(Guid id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}
