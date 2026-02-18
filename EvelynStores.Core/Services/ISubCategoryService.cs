using EvelynStores.Core.DTOs;

namespace EvelynStores.Core.Services;

public interface ISubCategoryService
{
    Task<PagedResponse<SubCategoryDto>> GetAllAsync(string? searchTerm = null, string status = "All", Guid? categoryId = null, int page = 1, int pageSize = 20);
    Task<SubCategoryDto?> GetByIdAsync(Guid id);
    Task<SubCategoryDto> CreateAsync(SubCategoryDto dto);
    Task<SubCategoryDto?> UpdateAsync(Guid id, SubCategoryDto dto);
    Task<bool> DeleteAsync(Guid id);
}
