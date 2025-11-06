using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.DTOs.Trip;
using Transport.Application.Services;

namespace Transport.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetAll()
    {
        var trips = await _tripService.GetAllAsync();
        return Ok(trips);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TripDto>> GetById(Guid id)
    {
        var trip = await _tripService.GetByIdAsync(id);
        if (trip == null)
            return NotFound();

        return Ok(trip);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<ActionResult<TripDto>> Create([FromBody] CreateTripDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        Guid? dispatcherId = userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var id) ? id : null;
        
        var trip = await _tripService.CreateAsync(dto, dispatcherId);
        return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<ActionResult<TripDto>> Update(Guid id, [FromBody] CreateTripDto dto)
    {
        var trip = await _tripService.UpdateAsync(id, dto);
        if (trip == null)
            return NotFound();

        return Ok(trip);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Dispatcher,Driver")]
    public async Task<ActionResult<TripDto>> UpdateStatus(Guid id, [FromBody] UpdateTripStatusRequest request)
    {
        var trip = await _tripService.UpdateStatusAsync(id, request.Status);
        if (trip == null)
            return NotFound();

        return Ok(trip);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _tripService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

public class UpdateTripStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

