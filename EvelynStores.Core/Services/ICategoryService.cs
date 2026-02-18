using EvelynStores.Core.DTOs;

namespace EvelynStores.Core.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<CategoryDto> CreateAsync(CategoryDto dto);
    Task<CategoryDto?> UpdateAsync(Guid id, CategoryDto dto);
    Task<bool> DeleteAsync(Guid id);
}