using EvelynStores.Core.Entities;
using EvelynStores.Core.Services;
using EvelynStores.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EvelynStores.Infrastructure.Repositories;

public class SubCategoryRepository : ISubCategoryRepository
{
    private readonly EvelynStoresDbContext _db;

    public SubCategoryRepository(EvelynStoresDbContext db)
    {
        _db = db;
    }

    public async Task<List<SubCategory>> GetAllAsync()
    {
        return await _db.SubCategories.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<SubCategory?> GetByIdAsync(Guid id)
    {
        return await _db.SubCategories.FindAsync(id);
    }

    public async Task AddAsync(SubCategory subCategory)
    {
        _db.SubCategories.Add(subCategory);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(SubCategory subCategory)
    {
        _db.SubCategories.Update(subCategory);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var s = await GetByIdAsync(id);
        if (s != null)
        {
            _db.SubCategories.Remove(s);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<(List<SubCategory> Items, int TotalCount)> GetFilteredAsync(string? searchTerm, string status, Guid? categoryId, int page, int pageSize)
    {
        var query = _db.SubCategories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s => s.Name.Contains(searchTerm) || s.Code.Contains(searchTerm));
        }

        if (!string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase)) query = query.Where(s => s.IsActive);
            else if (string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase)) query = query.Where(s => !s.IsActive);
        }

        if (categoryId.HasValue && categoryId != Guid.Empty) query = query.Where(s => s.CategoryId == categoryId.Value);

        var total = await query.CountAsync();

        var items = await query.OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
