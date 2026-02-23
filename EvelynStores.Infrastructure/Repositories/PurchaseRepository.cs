using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;
using EvelynStores.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EvelynStores.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly EvelynStoresDbContext _db;
    public PurchaseRepository(EvelynStoresDbContext db) => _db = db;

    public async Task AddAsync(Purchase purchase)
    {
        _db.Purchases.Add(purchase);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Purchase>> GetAllAsync() => await _db.Purchases.ToListAsync();

    public async Task<Purchase?> GetByIdAsync(Guid id) => await _db.Purchases.FindAsync(id);

    public async Task UpdateAsync(Purchase purchase)
    {
        _db.Purchases.Update(purchase);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var p = await _db.Purchases.FindAsync(id);
        if (p == null) return;
        _db.Purchases.Remove(p);
        await _db.SaveChangesAsync();
    }
}
