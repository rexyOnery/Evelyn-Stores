using EvelynStores.Core.DTOs;
using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;
using EvelynStores.Infrastructure.Migrations;

namespace EvelynStores.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly IProductLevelRepository _levelRepo;
    public ProductService(IProductRepository repo) => _repo = repo;

    // new overload that accepts the product level repository to support shop product queries
    public ProductService(IProductRepository repo, IProductLevelRepository levelRepo)
    {
        _repo = repo;
        _levelRepo = levelRepo;
    }

    public async Task<List<ProductDto>> GetRecentProductsAsync(int take = 5)
    {
        var list = await _repo.GetRecentAsync(take);
        return list.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            CategoryId = p.CategoryId,
            SubCategoryId = p.SubCategoryId,
            BrandId = p.BrandId,
            Unit = p.Unit,
            Quantity = p.Quantity,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
        }).ToList();
    }

    public async Task<ProductDto> CreateAsync(ProductDto dto)
    {
        var p = new Product
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Name = dto.Name,
            //  SKU = dto.SKU,
            CategoryId = dto.CategoryId,
            SubCategoryId = dto.SubCategoryId,
            BrandId = dto.BrandId,
            Unit = dto.Unit,
            //Quantity = dto.Quantity,
            //Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            CreatedBy = dto.CreatedBy,
            CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
            //ManufacturedDate = dto.ManufacturedDate,
            //ExpiryDate = dto.ExpiryDate,
            //ReOrderLevel = dto.ReOrderLevel
        };

        await _repo.AddAsync(p);
        dto.Id = p.Id;
        dto.Unit = p.Unit;
        dto.CreatedAt = p.CreatedAt;
        return dto;
    }

    public async Task<List<ProductDto>> GetBySubCategoryAsync(Guid subCategoryId)
    {
        var list = await _repo.GetBySubCategoryAsync(subCategoryId);
        return list.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            CategoryId = p.CategoryId,
            SubCategoryId = p.SubCategoryId,
            BrandId = p.BrandId,
            Unit = p.Unit,
            ImageUrl = p.ImageUrl,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
        }).ToList();
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, ProductDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;

        existing.Name = dto.Name;
       //existing.SKU = dto.SKU;
        existing.CategoryId = dto.CategoryId;
        existing.SubCategoryId = dto.SubCategoryId;
        existing.BrandId = dto.BrandId;
        existing.Unit = dto.Unit;
        //existing.Quantity = dto.Quantity;
        //existing.Price = dto.Price;
        existing.ImageUrl = dto.ImageUrl;
        //existing.ManufacturedDate = dto.ManufacturedDate;
        //existing.ExpiryDate = dto.ExpiryDate;
        //existing.ReOrderLevel = dto.ReOrderLevel;

        await _repo.UpdateAsync(existing);

        dto.Id = existing.Id;
        dto.Unit = existing.Unit;
        dto.CreatedAt = existing.CreatedAt;
        return dto;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return null;
        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            //SKU = p.SKU,
            CategoryId = p.CategoryId,
            SubCategoryId = p.SubCategoryId,
            BrandId = p.BrandId,
            Unit = p.Unit,
            //Quantity = p.Quantity,
            //ReOrderLevel = p.ReOrderLevel,
            //Price = p.Price,
            ImageUrl = p.ImageUrl,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
            //ManufacturedDate = p.ManufacturedDate,
            //ExpiryDate = p.ExpiryDate
        };
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
           // SKU = p.SKU,
            CategoryId = p.CategoryId,
            SubCategoryId = p.SubCategoryId,
            BrandId = p.BrandId,
            Unit = p.Unit,
            Quantity = p.Quantity,
            //ReOrderLevel = p.ReOrderLevel,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
            //ManufacturedDate = p.ManufacturedDate,
            //ExpiryDate = p.ExpiryDate,
            
        }).ToList();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _repo.DeleteAsync(id);
        return true;
    }

    public Task<List<ProductDto>> GetAllShopProductsAsync()
    {
        // Return products for the shop view by joining ProductLevels with Products.
        // Use the product-level record as the authoritative inventory/price source.
        if (_levelRepo == null)
            throw new InvalidOperationException("IProductLevelRepository is not available. Register it in DI or use the other constructor.");

        return GetAllShopProductsInternalAsync();
    }

    private async Task<List<ProductDto>> GetAllShopProductsInternalAsync()
    {
        var levels = await _levelRepo.GetAllAsync();

        return levels.Select(pl => new ProductDto
        {
            Id = pl.ProductId,
            Name = pl.Product?.Name ?? string.Empty,
            CategoryId = pl.Product?.CategoryId ?? Guid.Empty,
            SubCategoryId = pl.Product?.SubCategoryId ?? Guid.Empty,
            BrandId = pl.Product?.BrandId ?? Guid.Empty,
            Unit = pl.Product?.Unit ?? string.Empty,
            Quantity = pl.InStockQuantity,
            Price = pl.Price,
            ImageUrl = pl.Product?.ImageUrl ?? string.Empty, 
            BatchNumber = pl.SKU.Split('-')[0]+"-"+ pl.SKU.Split('-')[2]
        }).ToList();
    }
}
