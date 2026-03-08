using EvelynStores.Core.DTOs;
using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;

namespace EvelynStores.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var cats = await _repo.GetAllAsync();
        return cats.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            ImageUrl = c.ImageUrl
            ,
            ProductCount = c.ProductCount
        }).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return null;
        return new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            ImageUrl = c.ImageUrl
        };
    }

    public async Task<CategoryDto> CreateAsync(CategoryDto dto)
    {
        var c = new Category
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Name = dto.Name,
            Slug = dto.Slug,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
            ImageUrl = dto.ImageUrl
        };

        await _repo.AddAsync(c);
        dto.Id = c.Id;
        dto.CreatedAt = c.CreatedAt;
        return dto;
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, CategoryDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;
        existing.Name = dto.Name;
        existing.Slug = dto.Slug;
        existing.IsActive = dto.IsActive;
        existing.ImageUrl = dto.ImageUrl;

        await _repo.UpdateAsync(existing);

        dto.Id = existing.Id;
        dto.CreatedAt = existing.CreatedAt;
        return dto;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;
        await _repo.DeleteAsync(id);
        return true;
    }

    public async Task<CategoryStatisticsDto> GetStatisticsAsync()
    {
        // Total categories and products
        // Use the repository interface to retrieve stats
        var totalCategories = await (_repo.GetType().GetMethod("GetTotalCategoriesAsync") != null ? ((dynamic)_repo).GetTotalCategoriesAsync() : Task.FromResult(0));
        var totalProducts = await (_repo.GetType().GetMethod("GetTotalProductsAsync") != null ? ((dynamic)_repo).GetTotalProductsAsync() : Task.FromResult(0));

        // Top 3 categories by sold items
        var top = await (_repo.GetType().GetMethod("GetTopCategoriesBySoldItemsAsync") != null ? ((dynamic)_repo).GetTopCategoriesBySoldItemsAsync(3) : Task.FromResult(new List<EvelynStores.Core.DTOs.CategoryStatisticsBySaleItemsDto>()));

        return new CategoryStatisticsDto
        {
            TotalCategories = totalCategories,
            TotalCategoryProductsCount = totalProducts,
            CategorySales = top
        };
    }
}