using EvelynStores.Core.DTOs;
using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;

namespace EvelynStores.Infrastructure.Services;

public class UnitService : IUnitService
{
    private readonly IUnitRepository _repo;

    public UnitService(IUnitRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<UnitDto>> GetAllAsync()
    {
        var units = await _repo.GetAllAsync();
        return units.Select(u => new UnitDto
        {
            Id = u.Id,
            Name = u.Name,
            ShortName = u.ShortName,
            IsActive = u.IsActive,
            NoOfProducts = u.NoOfProducts,
            CreatedAt = u.CreatedAt
        }).ToList();
    }

    public async Task<UnitDto?> GetByIdAsync(Guid id)
    {
        var u = await _repo.GetByIdAsync(id);
        if (u == null) return null;
        return new UnitDto
        {
            Id = u.Id,
            Name = u.Name,
            ShortName = u.ShortName,
            IsActive = u.IsActive,
            NoOfProducts = u.NoOfProducts,
            CreatedAt = u.CreatedAt
        };
    }

    public async Task<UnitDto> CreateAsync(UnitDto dto)
    {
        var u = new Unit
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Name = dto.Name,
            ShortName = dto.ShortName,
            IsActive = dto.IsActive,
            NoOfProducts = dto.NoOfProducts,
            CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt
        };

        await _repo.AddAsync(u);
        dto.Id = u.Id;
        dto.CreatedAt = u.CreatedAt;
        return dto;
    }

    public async Task<UnitDto?> UpdateAsync(Guid id, UnitDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;
        existing.Name = dto.Name;
        existing.ShortName = dto.ShortName;
        existing.IsActive = dto.IsActive;
        existing.NoOfProducts = dto.NoOfProducts;

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
}
