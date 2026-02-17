using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EvelynStores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitsController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var units = await _unitService.GetAllAsync();
        return Ok(EvelynPhilApiResponse<List<UnitDto>>.SuccessResponse(units));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var unit = await _unitService.GetByIdAsync(id);
        if (unit == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Unit not found", 404));
        return Ok(EvelynPhilApiResponse<UnitDto>.SuccessResponse(unit));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UnitDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed.", 400, errors));
        }

        var created = await _unitService.CreateAsync(dto);
        return StatusCode(201, EvelynPhilApiResponse<UnitDto>.SuccessResponse(created, "Created", 201));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UnitDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed.", 400, errors));
        }

        var updated = await _unitService.UpdateAsync(id, dto);
        if (updated == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Unit not found", 404));
        return Ok(EvelynPhilApiResponse<UnitDto>.SuccessResponse(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _unitService.DeleteAsync(id);
        if (!ok) return NotFound(EvelynPhilApiResponse.ErrorResponse("Unit not found", 404));
        return Ok(EvelynPhilApiResponse.SuccessResponse("Deleted"));
    }
}
