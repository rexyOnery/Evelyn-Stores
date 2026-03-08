using EvelynStores.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvelynStores.Core.Services
{
    public interface ISaleItemService
    {
        Task<IEnumerable<RecentSaleItemDto>> GetRecentSaleItemsAsync(int take = 5);
    }
}
