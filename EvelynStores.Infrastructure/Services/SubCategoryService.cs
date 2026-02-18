using EvelynStores.Core.DTOs;
using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;

namespace EvelynStores.Infrastructure.Services;

public class SubCategoryService : ISubCategoryService
{
    private readonly ISubCategoryRepository _repo;

    public SubCategoryService(ISubCategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SubCategoryDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(s => new SubCategoryDto
        {
            Id = s.Id,
            Name = s.Name,
            CategoryId = s.CategoryId,
            Slug = s.Slug,
            Description = s.Description, 
            Code = s.Code,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt
        }).ToList();
    }

    public async Task<SubCategoryDto?> GetByIdAsync(Guid id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return null;
        return new SubCategoryDto
        {
            Id = s.Id,
            Name = s.Name,
            CategoryId = s.CategoryId,
            Slug = s.Slug,
            Description = s.Description, 
            Code = s.Code,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt
        };
    }

    public async Task<SubCategoryDto> CreateAsync(SubCategoryDto dto)
    {
        var s = new SubCategory
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Name = dto.Name,
            CategoryId = dto.CategoryId,
            Slug = dto.Slug,
            Description = dto.Description, 
            Code = dto.Code,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt
        };

        await _repo.AddAsync(s);
        dto.Id = s.Id;
        dto.CreatedAt = s.CreatedAt;
        return dto;
    }

    public async Task<SubCategoryDto?> UpdateAsync(Guid id, SubCategoryDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;
        existing.Name = dto.Name;
        existing.CategoryId = dto.CategoryId;
        existing.Slug = dto.Slug;
        existing.Description = dto.Description; 
        existing.Code = dto.Code;
        existing.IsActive = dto.IsActive;

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

    public async Task<PagedResponse<SubCategoryDto>> GetAllAsync(string? searchTerm = null, string status = "All", Guid? categoryId = null, int page = 1, int pageSize = 20)
    {
        // Ensure page parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        var (items, total) = await _repo.GetFilteredAsync(searchTerm, status, categoryId, page, pageSize);

        var dtos = items.Select(s => new SubCategoryDto
        {
            Id = s.Id,
            Name = s.Name,
            CategoryId = s.CategoryId,
            Slug = s.Slug,
            Description = s.Description, 
            Code = s.Code,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt
        }).ToList();

        return new PagedResponse<SubCategoryDto>
        {
            Items = dtos,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }
}
