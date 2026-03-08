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

    public async Task<int> GetTotalCategoriesAsync() => await _db.Categories.CountAsync();

    public async Task<int> GetTotalProductsAsync() => await _db.Products.CountAsync();

    /// <summary>
    /// Returns top categories by number of sold items (joins Sales -> SaleItems -> Products -> Categories)
    /// </summary>
    public async Task<List<Core.DTOs.CategoryStatisticsBySaleItemsDto>> GetTopCategoriesBySoldItemsAsync(int take = 3)
    {
        var query = from si in _db.SaleItems
                    join p in _db.Products on si.ProductId equals p.Id
                    join c in _db.Categories on p.CategoryId equals c.Id
                    group si by new { c.Id, c.Name } into g
                    select new Core.DTOs.CategoryStatisticsBySaleItemsDto
                    {
                        CategoryName = g.Key.Name,
                        TotalCategorySaleItems = g.Count()
                    };

        return await query.OrderByDescending(x => x.TotalCategorySaleItems).Take(take).ToListAsync();
    }
    

    public async Task<List<Category>> GetAllAsync()
    {
        // Fetch categories and product counts in a single query
        var categories = await _db.Categories.OrderByDescending(c => c.CreatedAt)
            .Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                ImageUrl = c.ImageUrl,
                ProductCount = _db.Products.Count(p => p.CategoryId == c.Id)
            })
            .ToListAsync();

        return categories;
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