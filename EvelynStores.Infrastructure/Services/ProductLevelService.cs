using System;
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

    public async Task<List<ProductLevelDto>> GetLowStockProductsAsync(int take = 5)
    {
        var list = await _repo.GetAllAsync();
        // low stock defined as InStockQuantity <= ReOrderLevel (or close to it)
        var low = list.Where(pl => pl.InStockQuantity <= pl.ReOrderLevel)
                      .OrderBy(pl => pl.InStockQuantity)
                      .Take(take)
                      .Select(pl => new ProductLevelDto
                      {
                          Id = pl.Id,
                          ProductId = pl.ProductId,
                          PurchaseQuantity = pl.PurchaseQuantity,
                          InStockQuantity = pl.InStockQuantity,
                          ReOrderLevel = pl.ReOrderLevel,
                          Price = pl.Price,
                          SKU = pl.SKU,
                          CategoryId = pl.Product?.CategoryId ?? Guid.Empty,
                          ProductName = pl.Product?.Name ?? string.Empty,
                          Unit = pl.Product?.Unit ?? string.Empty,
                          ProductQuantity = pl.Product?.Quantity ?? pl.InStockQuantity,
                          ImageUrl = pl.Product?.ImageUrl ?? string.Empty
                      }).ToList();

        return low;
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
            ReOrderLevel = existing.ReOrderLevel,
            
            SKU = existing.SKU,
            Price = existing.Price,
            CategoryId = existing.Product?.CategoryId ?? Guid.Empty,
            ProductName = existing.Product?.Name ?? string.Empty,
            Unit = existing.Product?.Unit ?? string.Empty,
            ProductQuantity = existing.Product?.Quantity ?? existing.InStockQuantity,
            ImageUrl = existing.Product?.ImageUrl ?? string.Empty
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
            ReOrderLevel = existing.ReOrderLevel,
            Price = existing.Price,
            SKU = existing.SKU,
            CategoryId = existing.Product?.CategoryId ?? Guid.Empty,
            ProductName = existing.Product?.Name ?? string.Empty,
            Unit = existing.Product?.Unit ?? string.Empty,
            ProductQuantity = existing.Product?.Quantity ?? existing.InStockQuantity,
            ImageUrl = existing.Product?.ImageUrl ?? string.Empty
        }).ToList();
    }

    public async Task<ProductLevelDto?> GetRandomAboveReorderAsync()
    {
        // Fetch all product levels with their product navigation included (repo does eager-load)
        var list = await _repo.GetAllAsync();
        // Filter product levels where ReOrderLevel < InStockQuantity
        var candidates = list.Where(pl => pl.ReOrderLevel >= pl.InStockQuantity).ToList();
        if (!candidates.Any()) return null;

        var rnd = new Random();
        var pick = candidates[rnd.Next(candidates.Count)];

        return new ProductLevelDto
        {
            Id = pick.Id,
            ProductId = pick.ProductId,
            PurchaseQuantity = pick.PurchaseQuantity,
            InStockQuantity = pick.InStockQuantity,
            ReOrderLevel = pick.ReOrderLevel,
            Price = pick.Price,
            SKU = pick.SKU,
            CategoryId = pick.Product?.CategoryId ?? Guid.Empty,
            ProductName = pick.Product?.Name ?? string.Empty,
            Unit = pick.Product?.Unit ?? string.Empty,
            ProductQuantity = pick.Product?.Quantity ?? pick.InStockQuantity,
            ImageUrl = pick.Product?.ImageUrl ?? string.Empty
        };
    }

    public async Task<ProductLevelDto> CreateAsync(ProductLevelDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        // If a record exists for the same product and SKU, update only the price
        var existing = await _repo.GetByProductIdAsync(dto.ProductId);
        if (existing != null && !string.IsNullOrWhiteSpace(existing.SKU) &&
            string.Equals(existing.SKU, dto.SKU, StringComparison.OrdinalIgnoreCase))
        {
            existing.Price = dto.Price;
            existing.InStockQuantity = dto.InStockQuantity;
            existing.PurchaseQuantity = dto.PurchaseQuantity;
            await _repo.UpdateAsync(existing);
            EnsureInStockFallback(existing);

            return new ProductLevelDto
            {
                Id = existing.Id,
                ProductId = existing.ProductId,
                PurchaseQuantity = existing.PurchaseQuantity,
                InStockQuantity = existing.InStockQuantity,
                ReOrderLevel = existing.ReOrderLevel,
                Price = existing.Price,
                SKU = existing.SKU
            };
        }

        // Otherwise insert a new record (either no existing record or SKU differs)
        var pl = new ProductLevel
        {
            ProductId = dto.ProductId,
            PurchaseQuantity = dto.PurchaseQuantity,
            InStockQuantity = dto.InStockQuantity,
            ReOrderLevel = dto.ReOrderLevel,
            Price = dto.Price,
            SKU = dto.SKU,
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
            ReOrderLevel = pl.ReOrderLevel,
            Price = pl.Price,
            SKU = pl.SKU
        };
    }

    public async Task<ProductLevelDto?> SetReOrderLevelAsync(Guid productId, int reorderLevel)
    {
        var existing = await _repo.GetByIdAsync(productId);//.GetByProductIdAsync(productId);
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
