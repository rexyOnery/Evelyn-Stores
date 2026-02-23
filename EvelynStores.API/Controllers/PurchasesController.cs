using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvelynStores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _purchaseService.GetAllAsync();
        return Ok(EvelynPhilApiResponse<List<PurchaseDto>>.SuccessResponse(list));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var p = await _purchaseService.GetByIdAsync(id);
        if (p == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));
        return Ok(EvelynPhilApiResponse<PurchaseDto>.SuccessResponse(p));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PurchaseDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed.", 400, errors));
        }

        try
        {
            var created = await _purchaseService.CreateAsync(dto);
            return StatusCode(201, EvelynPhilApiResponse<PurchaseDto>.SuccessResponse(created, "Created", 201));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to create purchase.", 500, new List<string> { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PurchaseDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed", 400));
        try
        {
            var updated = await _purchaseService.UpdateAsync(id, dto);
            if (updated == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));
            return Ok(EvelynPhilApiResponse<PurchaseDto>.SuccessResponse(updated));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to update purchase.", 500, new List<string> { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _purchaseService.DeleteAsync(id);
        if (!ok) return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));
        return Ok(EvelynPhilApiResponse.SuccessResponse("Deleted"));
    }
}
