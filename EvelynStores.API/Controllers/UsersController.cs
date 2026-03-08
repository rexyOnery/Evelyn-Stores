using EvelynStores.Core.DTOs;
using EvelynStores.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvelynStores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService, IAuthService authService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await userService.GetAllAsync();
        return Ok(EvelynPhilApiResponse<List<UserDto>>.SuccessResponse(list));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user is null)
            return NotFound(EvelynPhilApiResponse.ErrorResponse("User not found", 404));
        return Ok(EvelynPhilApiResponse<UserDto>.SuccessResponse(user));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed.", 400, errors));
        }

        var result = await authService.RegisterAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(EvelynPhilApiResponse.ErrorResponse("Validation failed", 400));

        var updated = await userService.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(EvelynPhilApiResponse.ErrorResponse("User not found", 404));

        return Ok(EvelynPhilApiResponse<UserDto>.SuccessResponse(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await userService.DeleteAsync(id);
        if (!ok) return NotFound(EvelynPhilApiResponse.ErrorResponse("User not found", 404));
        return Ok(EvelynPhilApiResponse.SuccessResponse("User deleted"));
    }
}
