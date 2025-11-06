using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.DTOs.Driver;
using Transport.Application.Services;

namespace Transport.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DriversController : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriversController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DriverDto>>> GetAll()
    {
        var drivers = await _driverService.GetAllAsync();
        return Ok(drivers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DriverDto>> GetById(Guid id)
    {
        var driver = await _driverService.GetByIdAsync(id);
        if (driver == null)
            return NotFound();

        return Ok(driver);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<ActionResult<DriverDto>> Create([FromBody] CreateDriverDto dto)
    {
        var driver = await _driverService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<ActionResult<DriverDto>> Update(Guid id, [FromBody] CreateDriverDto dto)
    {
        var driver = await _driverService.UpdateAsync(id, dto);
        if (driver == null)
            return NotFound();

        return Ok(driver);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _driverService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

