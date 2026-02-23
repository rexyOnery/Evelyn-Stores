using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;
using EvelynStores.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EvelynStores.Infrastructure.Repositories;

public class ProductLevelRepository : IProductLevelRepository
{
    private readonly EvelynStoresDbContext _db;
    public ProductLevelRepository(EvelynStoresDbContext db) => _db = db;

    public async Task AddAsync(ProductLevel level)
    {
        _db.ProductLevels.Add(level);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var p = await _db.ProductLevels.FindAsync(id);
        if (p == null) return;
        _db.ProductLevels.Remove(p);
        await _db.SaveChangesAsync();
    }

    public async Task<List<ProductLevel>> GetAllAsync() => await _db.ProductLevels.ToListAsync();

    public async Task<ProductLevel?> GetByIdAsync(Guid id) => await _db.ProductLevels.FindAsync(id);

    public async Task<ProductLevel?> GetByProductIdAsync(Guid productId) => await _db.ProductLevels.FirstOrDefaultAsync(pl => pl.ProductId == productId);

    public async Task UpdateAsync(ProductLevel level)
    {
        _db.ProductLevels.Update(level);
        await _db.SaveChangesAsync();
    }
}
