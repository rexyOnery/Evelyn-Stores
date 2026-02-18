using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvelynStores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubCategoriesController : ControllerBase
{
    private readonly ISubCategoryService _service;
    private readonly IWebHostEnvironment _env;

    public SubCategoriesController(ISubCategoryService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // support query params: searchTerm, status, categoryId, page, pageSize
        var searchTerm = HttpContext.Request.Query["searchTerm"].FirstOrDefault();
        var status = HttpContext.Request.Query["status"].FirstOrDefault() ?? "All";
        var cat = HttpContext.Request.Query["categoryId"].FirstOrDefault();
        Guid? categoryId = null;
        if (Guid.TryParse(cat, out var g)) categoryId = g;

        int page = 1, pageSize = 20;
        if (int.TryParse(HttpContext.Request.Query["page"].FirstOrDefault(), out var p)) page = p > 0 ? p : 1;
        if (int.TryParse(HttpContext.Request.Query["pageSize"].FirstOrDefault(), out var ps)) pageSize = ps > 0 ? ps : 20;

        var paged = await _service.GetAllAsync(searchTerm, status, categoryId, page, pageSize);
        return Ok(EvelynPhilApiResponse<PagedResponse<SubCategoryDto>>.SuccessResponse(paged));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));
        return Ok(EvelynPhilApiResponse<SubCategoryDto>.SuccessResponse(item));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SubCategoryDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed", 400));
        var created = await _service.CreateAsync(dto);
        return StatusCode(201, EvelynPhilApiResponse<SubCategoryDto>.SuccessResponse(created, "Created", 201));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SubCategoryDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed", 400));
        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null) return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));
        return Ok(EvelynPhilApiResponse<SubCategoryDto>.SuccessResponse(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound(EvelynPhilApiResponse.ErrorResponse("Not found", 404));
        return Ok(EvelynPhilApiResponse.SuccessResponse("Deleted"));
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest(EvelynPhilApiResponse.ErrorResponse("No file uploaded", 400));

        // validation
        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        const long maxBytes = 5 * 1024 * 1024; // 5 MB
        if (file.Length > maxBytes) return BadRequest(EvelynPhilApiResponse.ErrorResponse("File too large. Max 5 MB.", 400));
        if (!allowed.Contains(file.ContentType)) return BadRequest(EvelynPhilApiResponse.ErrorResponse("Invalid file type. Only JPG, PNG, WEBP allowed.", 400));

        var uploads = Path.Combine(_env.WebRootPath, "uploads", "subcategories");
        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploads, fileName);

        // save original
        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        // generate thumbnail 300px width, keep ratio
        var thumbName = Path.GetFileNameWithoutExtension(fileName) + "_thumb" + ".jpg";
        var thumbPath = Path.Combine(uploads, thumbName);

        try
        {
            // use ImageSharp for thumbnail generation
            //using (var image = SixLabors.ImageSharp.Image.Load(filePath))
            //{
            //    var size = new SixLabors.ImageSharp.Processing.ResizeOptions
            //    {
            //        Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max,
            //        Size = new SixLabors.ImageSharp.Size(300, 300)
            //    };
            //    image.Mutate(x => x.Resize(size));
            //    image.SaveAsJpeg(thumbPath);
            //}
        }
        catch
        {
            // ignore thumbnail errors, continue returning original
            thumbName = string.Empty;
        }

        var result = new ImageUploadResult
        {
            OriginalUrl = $"/uploads/subcategories/{fileName}",
            ThumbnailUrl = string.IsNullOrEmpty(thumbName) ? string.Empty : $"/uploads/subcategories/{thumbName}"
        };

        return Ok(EvelynPhilApiResponse<ImageUploadResult>.SuccessResponse(result, "Uploaded"));
    }
}
