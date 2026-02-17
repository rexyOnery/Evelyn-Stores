using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;
using EvelynStores.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EvelynStores.Infrastructure.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly EvelynStoresDbContext _db;

    public UnitRepository(EvelynStoresDbContext db)
    {
        _db = db;
    }

    public async Task<List<Unit>> GetAllAsync()
    {
        return await _db.Units.OrderByDescending(u => u.CreatedAt).ToListAsync();
    }

    public async Task<Unit?> GetByIdAsync(Guid id)
    {
        return await _db.Units.FindAsync(id);
    }

    public async Task AddAsync(Unit unit)
    {
        _db.Units.Add(unit);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Unit unit)
    {
        _db.Units.Update(unit);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var u = await GetByIdAsync(id);
        if (u != null)
        {
            _db.Units.Remove(u);
            await _db.SaveChangesAsync();
        }
    }
}
