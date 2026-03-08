using EvelynStores.Core.Entities;

namespace EvelynStores.Core.Services;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Guid id);
    Task<int> GetTotalCategoriesAsync();
    Task<int> GetTotalProductsAsync();
    Task<List<EvelynStores.Core.DTOs.CategoryStatisticsBySaleItemsDto>> GetTopCategoriesBySoldItemsAsync(int take = 3);
}