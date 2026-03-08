using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;
using EvelynStores.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvelynStores.Infrastructure.Services
{
    public class SaleItemService : ISaleItemService
    {
        private readonly EvelynStoresDbContext _context;

        public SaleItemService(EvelynStoresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RecentSaleItemDto>> GetRecentSaleItemsAsync(int take = 5)
        {
            // Join SaleItems with their parent Sale to get CreatedAt
            var items = await _context.SaleItems
                .AsNoTracking()
                .Join(_context.Sales,
                      si => si.SaleId,
                      s => s.Id,
                      (si, s) => new RecentSaleItemDto
                      {
                          Id = si.Id,
                          SaleId = si.SaleId,
                          ProductId = si.ProductId,
                          ProductName = si.ProductName,
                          ProductImageUrl = si.ProductImageUrl,
                          UnitPrice = si.UnitPrice,
                          Quantity = si.Quantity,
                          LineTotal = si.LineTotal,
                          SaleDate = s.CreatedAt
                      })
                .OrderByDescending(r => r.SaleDate)
                .ThenByDescending(r => r.Id)
                .Take(take)
                .ToListAsync();

            return items;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.SaleItems.AsNoTracking().CountAsync();
        }

        public async Task<decimal> GetTotalLineTotalAsync()
        {
            return await _context.SaleItems.AsNoTracking().SumAsync(si => si.LineTotal);
        }
    }
}
