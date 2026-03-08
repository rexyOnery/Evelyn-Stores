using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvelynStores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _saleService.GetAllSalesAsync();
        return Ok(EvelynPhilApiResponse<IEnumerable<SaleDto>>.SuccessResponse(sales));
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetTotalCount()
    {
        try
        {
            var count = await _saleService.GetTotalSalesCountAsync();
            return Ok(EvelynPhilApiResponse<int>.SuccessResponse(count));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to get total sales count", 500, new List<string> { ex.Message }));
        }
    }

    [HttpGet("count/today")]
    public async Task<IActionResult> GetTodayCount()
    {
        var utcNow = DateTime.UtcNow.Date;
        var count = await _saleService.GetSalesCountForDateAsync(utcNow);
        return Ok(EvelynPhilApiResponse<int>.SuccessResponse(count));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sale = await _saleService.GetSaleByIdAsync(id);
        if (sale == null)
            return NotFound(EvelynPhilApiResponse.ErrorResponse("Sale not found", 404));
        return Ok(EvelynPhilApiResponse<SaleDto>.SuccessResponse(sale));
    }

    [HttpGet("transaction/{transactionId}")]
    public async Task<IActionResult> GetByTransactionId(string transactionId)
    {
        var sale = await _saleService.GetSaleByTransactionIdAsync(transactionId);
        if (sale == null)
            return NotFound(EvelynPhilApiResponse.ErrorResponse("Sale not found", 404));
        return Ok(EvelynPhilApiResponse<SaleDto>.SuccessResponse(sale));
    }

    [HttpGet("date-range")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var sales = await _saleService.GetSalesByDateRangeAsync(startDate, endDate);
        return Ok(EvelynPhilApiResponse<IEnumerable<SaleDto>>.SuccessResponse(sales));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed.", 400, errors));
        }
        


        if (dto.Items == null || dto.Items.Count == 0)
        {
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("At least one item is required.", 400));
        }

        try
        {
            var created = await _saleService.CreateSaleAsync(dto);
            return StatusCode(201, EvelynPhilApiResponse<SaleDto>.SuccessResponse(created, "Sale created successfully", 201));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to create sale.", 500, new List<string> { ex.Message }));
        }
    }
}
