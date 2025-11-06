using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.DTOs.Truck;
using Transport.Application.Services;

namespace Transport.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TrucksController : ControllerBase
{
    private readonly ITruckService _truckService;

    public TrucksController(ITruckService truckService)
    {
        _truckService = truckService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TruckDto>>> GetAll()
    {
        var trucks = await _truckService.GetAllAsync();
        return Ok(trucks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TruckDto>> GetById(Guid id)
    {
        var truck = await _truckService.GetByIdAsync(id);
        if (truck == null)
            return NotFound();

        return Ok(truck);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<ActionResult<TruckDto>> Create([FromBody] CreateTruckDto dto)
    {
        var truck = await _truckService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = truck.Id }, truck);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<ActionResult<TruckDto>> Update(Guid id, [FromBody] CreateTruckDto dto)
    {
        var truck = await _truckService.UpdateAsync(id, dto);
        if (truck == null)
            return NotFound();

        return Ok(truck);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _truckService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

