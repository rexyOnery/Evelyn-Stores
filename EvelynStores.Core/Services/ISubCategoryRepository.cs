using EvelynStores.Core.Entities;

namespace EvelynStores.Core.Services;

public interface ISubCategoryRepository
{
    Task<List<SubCategory>> GetAllAsync();
    Task<SubCategory?> GetByIdAsync(Guid id);
    Task AddAsync(SubCategory subCategory);
    Task UpdateAsync(SubCategory subCategory);
    Task DeleteAsync(Guid id);
    Task<(List<SubCategory> Items, int TotalCount)> GetFilteredAsync(string? searchTerm, string status, Guid? categoryId, int page, int pageSize);
}
