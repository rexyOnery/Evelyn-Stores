using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;
using EvelynStores.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EvelynStores.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly EvelynStoresDbContext _db;

    public CategoryRepository(EvelynStoresDbContext db)
    {
        _db = db;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _db.Set<Category>().OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _db.Set<Category>().FindAsync(id);
    }

    public async Task AddAsync(Category category)
    {
        _db.Set<Category>().Add(category);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _db.Set<Category>().Update(category);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var e = await GetByIdAsync(id);
        if (e != null)
        {
            _db.Set<Category>().Remove(e);
            await _db.SaveChangesAsync();
        }
    }
}