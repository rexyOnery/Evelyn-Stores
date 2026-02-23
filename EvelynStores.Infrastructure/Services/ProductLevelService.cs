using EvelynStores.Core.DTOs;
using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;

namespace EvelynStores.Infrastructure.Services;

public class ProductLevelService : IProductLevelService
{
    private readonly IProductLevelRepository _repo;

    public ProductLevelService(IProductLevelRepository repo) => _repo = repo;
    
    private void EnsureInStockFallback(ProductLevel pl)
    {
        if (pl == null) return;
        if (pl.InStockQuantity == 0)
        {
            pl.InStockQuantity = pl.PurchaseQuantity;
        }
    }
    public async Task<ProductLevelDto> CreateOrUpdateForPurchaseAsync(Guid productId, int purchaseQuantity)
    {
        var existing = await _repo.GetByProductIdAsync(productId);
        if (existing == null)
        {
            var pl = new ProductLevel
            {
                ProductId = productId,
                PurchaseQuantity = purchaseQuantity,
                InStockQuantity = purchaseQuantity,
                CreatedAt = DateTime.UtcNow,
                ReOrderLevel = 0
            };
            EnsureInStockFallback(pl);
            await _repo.AddAsync(pl);
            return new ProductLevelDto
            {
                Id = pl.Id,
                ProductId = pl.ProductId,
                PurchaseQuantity = pl.PurchaseQuantity,
                InStockQuantity = pl.InStockQuantity,
                ReOrderLevel = pl.ReOrderLevel
            };
        }

        // update purchase quantity only
        existing.PurchaseQuantity += purchaseQuantity;
        EnsureInStockFallback(existing);
        await _repo.UpdateAsync(existing);
        return new ProductLevelDto
        {
            Id = existing.Id,
            ProductId = existing.ProductId,
            PurchaseQuantity = existing.PurchaseQuantity,
            InStockQuantity = existing.InStockQuantity,
            ReOrderLevel = existing.ReOrderLevel
        };
    }

    public async Task<ProductLevelDto?> AdjustPurchaseQuantityAsync(Guid productId, int delta)
    {
        var existing = await _repo.GetByProductIdAsync(productId);
        if (existing == null)
        {
            if (delta <= 0) return null;
            var pl = new ProductLevel
            {
                ProductId = productId,
                PurchaseQuantity = delta,
                InStockQuantity = delta,
                CreatedAt = DateTime.UtcNow,
                ReOrderLevel = 0
            };
            EnsureInStockFallback(pl);
            await _repo.AddAsync(pl);
            return new ProductLevelDto
            {
                Id = pl.Id,
                ProductId = pl.ProductId,
                PurchaseQuantity = pl.PurchaseQuantity,
                InStockQuantity = pl.InStockQuantity,
                ReOrderLevel = pl.ReOrderLevel
            };
        }

        existing.PurchaseQuantity = Math.Max(0, existing.PurchaseQuantity + delta);
        EnsureInStockFallback(existing);
        await _repo.UpdateAsync(existing);
        return new ProductLevelDto
        {
            Id = existing.Id,
            ProductId = existing.ProductId,
            PurchaseQuantity = existing.PurchaseQuantity,
            InStockQuantity = existing.InStockQuantity,
            ReOrderLevel = existing.ReOrderLevel
        };
    }

    public async Task<ProductLevelDto> AdjustInStockAsync(Guid productId, int quantityDelta)
    {
        var existing = await _repo.GetByProductIdAsync(productId);
        if (existing == null)
        {
            // if no level exists, create one with InStock = quantityDelta and PurchaseQuantity = 0
            var pl = new ProductLevel
            {
                ProductId = productId,
                PurchaseQuantity = 0,
                InStockQuantity = Math.Max(0, quantityDelta),
                ReOrderLevel = 0,
                CreatedAt = DateTime.UtcNow
            };
            EnsureInStockFallback(pl);
            await _repo.AddAsync(pl);
            return new ProductLevelDto
            {
                Id = pl.Id,
                ProductId = pl.ProductId,
                PurchaseQuantity = pl.PurchaseQuantity,
                InStockQuantity = pl.InStockQuantity,
                ReOrderLevel = pl.ReOrderLevel
            };
        }

        existing.InStockQuantity += quantityDelta;
        if (existing.InStockQuantity < 0) existing.InStockQuantity = 0;
        EnsureInStockFallback(existing);
        await _repo.UpdateAsync(existing);

        return new ProductLevelDto
        {
            Id = existing.Id,
            ProductId = existing.ProductId,
            PurchaseQuantity = existing.PurchaseQuantity,
            InStockQuantity = existing.InStockQuantity,
            ReOrderLevel = existing.ReOrderLevel
        };
    }

    public async Task<ProductLevelDto?> GetByProductIdAsync(Guid productId)
    {
        var existing = await _repo.GetByProductIdAsync(productId);
        if (existing == null) return null;
        return new ProductLevelDto
        {
            Id = existing.Id,
            ProductId = existing.ProductId,
            PurchaseQuantity = existing.PurchaseQuantity,
            InStockQuantity = existing.InStockQuantity,
            ReOrderLevel = existing.ReOrderLevel
        };
    }

    public async Task<List<ProductLevelDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(existing => new ProductLevelDto
        {
            Id = existing.Id,
            ProductId = existing.ProductId,
            PurchaseQuantity = existing.PurchaseQuantity,
            InStockQuantity = existing.InStockQuantity,
            ReOrderLevel = existing.ReOrderLevel
        }).ToList();
    }

    public async Task<ProductLevelDto?> SetReOrderLevelAsync(Guid productId, int reorderLevel)
    {
        var existing = await _repo.GetByProductIdAsync(productId);
        if (existing == null) return null;
        existing.ReOrderLevel = reorderLevel;
        EnsureInStockFallback(existing);
        await _repo.UpdateAsync(existing);
        return new ProductLevelDto
        {
            Id = existing.Id,
            ProductId = existing.ProductId,
            PurchaseQuantity = existing.PurchaseQuantity,
            InStockQuantity = existing.InStockQuantity,
            ReOrderLevel = existing.ReOrderLevel
        };
    }
}
