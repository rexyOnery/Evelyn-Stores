using EvelynStores.Core.DTOs;

namespace EvelynStores.Core.Services;

public interface IUnitService
{
    Task<List<UnitDto>> GetAllAsync();
    Task<UnitDto?> GetByIdAsync(Guid id);
    Task<UnitDto> CreateAsync(UnitDto dto);
    Task<UnitDto?> UpdateAsync(Guid id, UnitDto dto);
    Task<bool> DeleteAsync(Guid id);
}
