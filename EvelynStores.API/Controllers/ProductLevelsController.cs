using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvelynStores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductLevelsController : ControllerBase
{
    private readonly IProductLevelService _levelService;

    public ProductLevelsController(IProductLevelService levelService)
    {
        _levelService = levelService;
    }

    [HttpGet("low")]
    public async Task<IActionResult> GetLowStock([FromQuery] int take = 5)
    {
        try
        {
            var list = await _levelService.GetLowStockProductsAsync(take);
            var summaries = list.Select(pl => new ProductLevelSummaryDto { 
                ProductName = pl.ProductName, 
                ImageUrl = pl.ImageUrl,
                InStockQuantity = pl.InStockQuantity }).ToList();
            return Ok(EvelynPhilApiResponse<List<ProductLevelSummaryDto>>.SuccessResponse(summaries));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to get low stock products", 500, new List<string> { ex.Message }));
        }
    }

        [HttpGet("random/above-reorder")]
        public async Task<IActionResult> GetRandomAboveReorder()
        {
            try
            {
                var res = await _levelService.GetRandomAboveReorderAsync();
                if (res == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("No suitable product found", 404));
                // Map to summary DTO for lighter payload
                var summary = new ProductLevelSummaryDto
                {
                    ProductName = res.ProductName,
                    InStockQuantity = res.InStockQuantity
                };
                return Ok(EvelynPhilApiResponse<ProductLevelSummaryDto>.SuccessResponse(summary));
            }
            catch (Exception ex)
            {
                return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to get random product", 500, new List<string> { ex.Message }));
            }
        }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var list = await _levelService.GetAllAsync();
            return Ok(EvelynPhilApiResponse<List<ProductLevelDto>>.SuccessResponse(list));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to get product levels", 500, new List<string> { ex.Message }));
        }

    }

    /// <summary>
    /// Creates a new product level record.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductLevelDto dto)
    {
        if (dto == null) return BadRequest(EvelynPhilApiResponse.ErrorResponse("Invalid request", 400));
        try
        {
            var created = await _levelService.CreateAsync(dto);
            return Ok(EvelynPhilApiResponse<ProductLevelDto>.SuccessResponse(created));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to create product level", 500, new List<string> { ex.Message }));
        }
    }

    [HttpPost("set-reorder/{levelId:guid}")]
    public async Task<IActionResult> SetReOrderLevel(Guid levelId, [FromBody] SetReorderRequest req)
    {
        if (req == null) return BadRequest(EvelynPhilApiResponse.ErrorResponse("Invalid request", 400));
        try
        {
            var updated = await _levelService.SetReOrderLevelAsync(levelId, req.ReOrderLevel);
            if (updated == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));
            return Ok(EvelynPhilApiResponse<ProductLevelDto>.SuccessResponse(updated));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to set reorder level", 500, new List<string> { ex.Message }));
        }
    }

    public class SetReorderRequest
    {
        public int ReOrderLevel { get; set; }
    }
    

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        try
        {
            var res = await _levelService.GetByProductIdAsync(productId);
            if (res == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));
            return Ok(EvelynPhilApiResponse<ProductLevelDto>.SuccessResponse(res));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to get product level", 500, new List<string> { ex.Message }));
        }
    }

    [HttpPost("adjust/{productId:guid}")]
    public async Task<IActionResult> AdjustInStock(Guid productId, [FromBody] AdjustStockRequest req)
    {
        if (req == null) return BadRequest(EvelynPhilApiResponse.ErrorResponse("Invalid request", 400));
        try
        {
            var updated = await _levelService.AdjustInStockAsync(productId, req.QuantityDelta);
            return Ok(EvelynPhilApiResponse<ProductLevelDto>.SuccessResponse(updated));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to adjust stock", 500, new List<string> { ex.Message }));
        }
    }

    public class AdjustStockRequest
    {
        public int QuantityDelta { get; set; }
    }
}
