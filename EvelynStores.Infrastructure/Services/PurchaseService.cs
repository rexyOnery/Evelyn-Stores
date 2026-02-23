using EvelynStores.Core.DTOs;
using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;

namespace EvelynStores.Infrastructure.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _repo;
    private readonly IProductLevelService _productLevelService;
    public PurchaseService(IPurchaseRepository repo, IProductLevelService productLevelService)
    {
        _repo = repo;
        _productLevelService = productLevelService;
    }

    public async Task<PurchaseDto> CreateAsync(PurchaseDto dto)
    {
        var p = new Purchase
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            SKU = dto.SKU,
            CategoryId = dto.CategoryId,
            SubCategoryId = dto.SubCategoryId,
            ProductId = dto.ProductId,
            UnitCost = dto.UnitCost,
            Quantity = dto.Quantity,
            TotalAmount = dto.TotalAmount,
            CreatedBy = dto.CreatedBy,
            CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
            ManufacturedDate = dto.ManufacturedDate,
            ExpiryDate = dto.ExpiryDate
        };

        await _repo.AddAsync(p);
        dto.Id = p.Id;
        dto.CreatedAt = p.CreatedAt;
        // update product level: create or update product level for this purchase
        try
        {
            await _productLevelService.CreateOrUpdateForPurchaseAsync(p.ProductId, p.Quantity);
        }
        catch
        {
            // swallow to avoid failing purchase creation on level update issues
        }
        return dto;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        // when deleting a purchase, decrease PurchaseQuantity on product level
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;

        try
        {
            // adjust in-stock (decrease) and purchase quantity when deleting a purchase
            await _productLevelService.AdjustInStockAsync(existing.ProductId, -existing.Quantity);
            await _productLevelService.AdjustPurchaseQuantityAsync(existing.ProductId, -existing.Quantity);
        }
        catch { }

        await _repo.DeleteAsync(id);
        return true;
    }

    public async Task<List<PurchaseDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(p => new PurchaseDto
        {
            Id = p.Id,
            SKU = p.SKU,
            CategoryId = p.CategoryId,
            SubCategoryId = p.SubCategoryId,
            ProductId = p.ProductId,
            UnitCost = p.UnitCost,
            Quantity = p.Quantity,
            TotalAmount = p.TotalAmount,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
            ManufacturedDate = p.ManufacturedDate,
            ExpiryDate = p.ExpiryDate
        }).ToList();
    }

    public async Task<PurchaseDto?> GetByIdAsync(Guid id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return null;
        return new PurchaseDto
        {
            Id = p.Id,
            SKU = p.SKU,
            CategoryId = p.CategoryId,
            SubCategoryId = p.SubCategoryId,
            ProductId = p.ProductId,
            UnitCost = p.UnitCost,
            Quantity = p.Quantity,
            TotalAmount = p.TotalAmount,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
            ManufacturedDate = p.ManufacturedDate,
            ExpiryDate = p.ExpiryDate
        };
    }

    public async Task<PurchaseDto?> UpdateAsync(Guid id, PurchaseDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;

        existing.SKU = dto.SKU;
        existing.CategoryId = dto.CategoryId;
        existing.SubCategoryId = dto.SubCategoryId;
        existing.ProductId = dto.ProductId;
        existing.UnitCost = dto.UnitCost;
        existing.Quantity = dto.Quantity;
        existing.TotalAmount = dto.TotalAmount;
        existing.CreatedBy = dto.CreatedBy;
        existing.ManufacturedDate = dto.ManufacturedDate;
        existing.ExpiryDate = dto.ExpiryDate;

        await _repo.UpdateAsync(existing);

        dto.Id = existing.Id;
        dto.CreatedAt = existing.CreatedAt;
        return dto;
    }
}
