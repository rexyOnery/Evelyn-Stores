using EvelynStores.Core.DTOs;
using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;

namespace EvelynStores.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<ProductDto> CreateAsync(ProductDto dto)
    {
        var p = new Product
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Name = dto.Name,
            SKU = dto.SKU,
            CategoryId = dto.CategoryId,
            SubCategoryId = dto.SubCategoryId,
            BrandId = dto.BrandId,
            Unit = dto.Unit,
            Quantity = dto.Quantity,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            CreatedBy = dto.CreatedBy,
            CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
            ManufacturedDate = dto.ManufacturedDate,
            ExpiryDate = dto.ExpiryDate
        };

        await _repo.AddAsync(p);
        dto.Id = p.Id;
        dto.CreatedAt = p.CreatedAt;
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
            SKU = p.SKU,
            CategoryId = p.CategoryId,
            SubCategoryId = p.SubCategoryId,
            BrandId = p.BrandId,
            Unit = p.Unit,
            Quantity = p.Quantity,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
            ManufacturedDate = p.ManufacturedDate,
            ExpiryDate = p.ExpiryDate
        };
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            SKU = p.SKU,
            CategoryId = p.CategoryId,
            SubCategoryId = p.SubCategoryId,
            BrandId = p.BrandId,
            Unit = p.Unit,
            Quantity = p.Quantity,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
            ManufacturedDate = p.ManufacturedDate,
            ExpiryDate = p.ExpiryDate
        }).ToList();
    }
}
