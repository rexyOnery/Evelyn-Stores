using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvelynStores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaleItemsController : ControllerBase
{
    private readonly ISaleItemService _saleItemService;

    public SaleItemsController(ISaleItemService saleItemService)
    {
        _saleItemService = saleItemService;
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int take = 5)
    {
        try
        {
            var items = await _saleItemService.GetRecentSaleItemsAsync(take);
            return Ok(EvelynPhilApiResponse<IEnumerable<RecentSaleItemDto>>.SuccessResponse(items.ToList()));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to get recent sale items", 500, new List<string> { ex.Message }));
        }
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetTotalCount()
    {
        try
        {
            var count = await _saleItemService.GetTotalCountAsync();
            return Ok(EvelynPhilApiResponse<int>.SuccessResponse(count));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to get sale items count", 500, new List<string> { ex.Message }));
        }
    }

    [HttpGet("total")]
    public async Task<IActionResult> GetTotalLineTotal()
    {
        try
        {
            var total = await _saleItemService.GetTotalLineTotalAsync();
            return Ok(EvelynPhilApiResponse<decimal>.SuccessResponse(total));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to get sale items total", 500, new List<string> { ex.Message }));
        }
    }
}
