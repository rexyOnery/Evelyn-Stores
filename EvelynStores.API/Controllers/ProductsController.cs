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
