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

    [HttpPost("set-reorder/{productId:guid}")]
    public async Task<IActionResult> SetReOrderLevel(Guid productId, [FromBody] SetReorderRequest req)
    {
        if (req == null) return BadRequest(EvelynPhilApiResponse.ErrorResponse("Invalid request", 400));
        try
        {
            var updated = await _levelService.SetReOrderLevelAsync(productId, req.ReOrderLevel);
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
