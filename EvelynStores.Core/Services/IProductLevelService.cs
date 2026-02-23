using EvelynStores.Core.DTOs;

namespace EvelynStores.Core.Services;

public interface IProductLevelService
{
    Task<ProductLevelDto> CreateOrUpdateForPurchaseAsync(Guid productId, int purchaseQuantity);
    Task<ProductLevelDto> AdjustInStockAsync(Guid productId, int quantityDelta);
    Task<ProductLevelDto?> GetByProductIdAsync(Guid productId);
    Task<List<ProductLevelDto>> GetAllAsync();
    Task<ProductLevelDto?> SetReOrderLevelAsync(Guid productId, int reorderLevel);
    Task<ProductLevelDto?> AdjustPurchaseQuantityAsync(Guid productId, int delta);
}
