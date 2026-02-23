using EvelynStores.Core.DTOs;

namespace EvelynStores.Core.Services;

public interface IPurchaseService
{
    Task<PurchaseDto> CreateAsync(PurchaseDto dto);
    Task<PurchaseDto?> GetByIdAsync(Guid id);
    Task<List<PurchaseDto>> GetAllAsync();
    Task<PurchaseDto?> UpdateAsync(Guid id, PurchaseDto dto);
    Task<bool> DeleteAsync(Guid id);
}
