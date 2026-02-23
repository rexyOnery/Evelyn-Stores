using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvelynStores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IProductService _productService;

    public ProductsController(IWebHostEnvironment env, IProductService productService)
    {
        _env = env;
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? subCategoryId)
    {
        if (subCategoryId.HasValue && subCategoryId != Guid.Empty)
        {
            var list = await _productService.GetBySubCategoryAsync(subCategoryId.Value);
            return Ok(EvelynPhilApiResponse<List<ProductDto>>.SuccessResponse(list));
        }

        var all = await _productService.GetAllAsync();
        return Ok(EvelynPhilApiResponse<List<ProductDto>>.SuccessResponse(all));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));

        return Ok(EvelynPhilApiResponse<ProductDto>.SuccessResponse(product));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _productService.DeleteAsync(id);
        if (!ok) return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));
        return Ok(EvelynPhilApiResponse.SuccessResponse("Deleted"));
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("No file uploaded"));

        var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

        var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploads, fileName);

        try
        {
            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/uploads/{fileName}";
            var result = new ImageUploadResult { OriginalUrl = url, ThumbnailUrl = url };
            return Ok(EvelynPhilApiResponse<ImageUploadResult>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to save file", 500, new List<string> { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed", 400));

        try
        {
            var updated = await _productService.UpdateAsync(id, dto);
            if (updated == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));
            return Ok(EvelynPhilApiResponse<ProductDto>.SuccessResponse(updated));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to update product.", 500, new List<string> { ex.Message }));
        }
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed.", 400, errors));
        }

        // dto.ImageUrl is expected to be a base64 data URL; productService will store as-is
        try
        {
            var created = await _productService.CreateAsync(dto);
            return StatusCode(201, EvelynPhilApiResponse<ProductDto>.SuccessResponse(created, "Created", 201));
        }
        catch (Exception ex)
        {
            return StatusCode(500, EvelynPhilApiResponse.ErrorResponse("Failed to create product.", 500, new List<string> { ex.Message }));
        }
    }
}
