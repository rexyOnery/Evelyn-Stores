using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace EvelynStores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // Note: We removed server-side file upload endpoint to prefer base64 in DTOs.

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cats = await _categoryService.GetAllAsync();
        return Ok(EvelynPhilApiResponse<List<CategoryDto>>.SuccessResponse(cats));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var cat = await _categoryService.GetByIdAsync(id);
        if (cat == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Category not found", 404));
        return Ok(EvelynPhilApiResponse<CategoryDto>.SuccessResponse(cat));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed.", 400, errors));
        }

        try
        {
            var created = await _categoryService.CreateAsync(dto);
            return StatusCode(201, EvelynPhilApiResponse<CategoryDto>.SuccessResponse(created, "Created", 201));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to create category.", 500, new List<string> { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed.", 400, errors));
        }

        var updated = await _categoryService.UpdateAsync(id, dto);
        if (updated == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Category not found", 404));
        return Ok(EvelynPhilApiResponse<CategoryDto>.SuccessResponse(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _categoryService.DeleteAsync(id);
        if (!ok) return NotFound(EvelynPhilApiResponse.ErrorResponse("Category not found", 404));
        return Ok(EvelynPhilApiResponse.SuccessResponse("Deleted"));
    }
}