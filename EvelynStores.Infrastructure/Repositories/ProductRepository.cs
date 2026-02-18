using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;
using EvelynStores.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EvelynStores.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly EvelynStoresDbContext _db;
    public ProductRepository(EvelynStoresDbContext db) => _db = db;

    public async Task AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Product>> GetAllAsync() => await _db.Products.ToListAsync();

    public async Task<Product?> GetByIdAsync(Guid id) => await _db.Products.FindAsync(id);

    public async Task UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return;
        _db.Products.Remove(p);
        await _db.SaveChangesAsync();
    }
}
