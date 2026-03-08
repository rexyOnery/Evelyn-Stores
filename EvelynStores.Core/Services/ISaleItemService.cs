using EvelynStores.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvelynStores.Core.Services
{
    public interface ISaleItemService
    {
        Task<IEnumerable<RecentSaleItemDto>> GetRecentSaleItemsAsync(int take = 5);

        /// <summary>Returns the total number of sale items across all sales.</summary>
        Task<int> GetTotalCountAsync();

        /// <summary>Returns the sum of all LineTotal values across all sale items.</summary>
        Task<decimal> GetTotalLineTotalAsync();
    }
}
