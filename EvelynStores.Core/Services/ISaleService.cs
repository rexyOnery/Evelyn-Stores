using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EvelynStores.Core.DTOs;

namespace EvelynStores.Core.Services
{
    public interface ISaleService
    {
        Task<SaleDto> CreateSaleAsync(CreateSaleDto dto);
        Task<SaleDto?> GetSaleByIdAsync(Guid id);
        Task<SaleDto?> GetSaleByTransactionIdAsync(string transactionId);
        Task<IEnumerable<SaleDto>> GetAllSalesAsync();
        Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
