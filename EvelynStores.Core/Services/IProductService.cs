using EvelynStores.Core.DTOs;

namespace EvelynStores.Core.Services;

public interface IProductService
{
    Task<ProductDto> CreateAsync(ProductDto dto);
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<List<ProductDto>> GetAllAsync();
    Task<List<ProductDto>> GetBySubCategoryAsync(Guid subCategoryId);
    Task<ProductDto?> UpdateAsync(Guid id, ProductDto dto);
    Task<bool> DeleteAsync(Guid id);
}
