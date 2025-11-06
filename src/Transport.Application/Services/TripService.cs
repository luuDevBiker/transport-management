using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Transport.Application.DTOs.Trip;
using Transport.Domain.Entities;
using Transport.Application.Interfaces;

namespace Transport.Application.Services;

public class TripService : ITripService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TripService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TripDto>> GetAllAsync()
    {
        var trips = await _context.Trips
            .Include(t => t.Customer)
            .Include(t => t.Truck)
            .Include(t => t.Driver)
            .ToListAsync();
        return _mapper.Map<IEnumerable<TripDto>>(trips);
    }

    public async Task<TripDto?> GetByIdAsync(Guid id)
    {
        var trip = await _context.Trips
            .Include(t => t.Customer)
            .Include(t => t.Truck)
            .Include(t => t.Driver)
            .FirstOrDefaultAsync(t => t.Id == id);
        return trip == null ? null : _mapper.Map<TripDto>(trip);
    }

    public async Task<TripDto> CreateAsync(CreateTripDto dto, Guid? dispatcherId = null)
    {
        var trip = _mapper.Map<Trip>(dto);
        trip.DispatcherId = dispatcherId;
        trip.TripNumber = $"TRP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        trip.Status = "Scheduled";
        
        // Ensure ScheduledDate is UTC
        if (trip.ScheduledDate.Kind != DateTimeKind.Utc)
        {
            trip.ScheduledDate = trip.ScheduledDate.ToUniversalTime();
        }
        
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();
        
        var createdTrip = await _context.Trips
            .Include(t => t.Customer)
            .Include(t => t.Truck)
            .Include(t => t.Driver)
            .FirstOrDefaultAsync(t => t.Id == trip.Id);
        
        return _mapper.Map<TripDto>(createdTrip!);
    }

    public async Task<TripDto?> UpdateAsync(Guid id, CreateTripDto dto)
    {
        var trip = await _context.Trips.FindAsync(id);
        if (trip == null) return null;

        _mapper.Map(dto, trip);
        
        // Ensure ScheduledDate is UTC
        if (trip.ScheduledDate.Kind != DateTimeKind.Utc)
        {
            trip.ScheduledDate = trip.ScheduledDate.ToUniversalTime();
        }
        
        trip.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        var updatedTrip = await _context.Trips
            .Include(t => t.Customer)
            .Include(t => t.Truck)
            .Include(t => t.Driver)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        return _mapper.Map<TripDto>(updatedTrip!);
    }

    public async Task<TripDto?> UpdateStatusAsync(Guid id, string status)
    {
        var trip = await _context.Trips.FindAsync(id);
        if (trip == null) return null;

        trip.Status = status;
        if (status == "InProgress" && trip.ActualStartDate == null)
        {
            trip.ActualStartDate = DateTime.UtcNow;
        }
        else if (status == "Completed" && trip.ActualEndDate == null)
        {
            trip.ActualEndDate = DateTime.UtcNow;
        }
        
        // Ensure dates are UTC
        if (trip.ActualStartDate.HasValue && trip.ActualStartDate.Value.Kind != DateTimeKind.Utc)
        {
            trip.ActualStartDate = trip.ActualStartDate.Value.ToUniversalTime();
        }
        if (trip.ActualEndDate.HasValue && trip.ActualEndDate.Value.Kind != DateTimeKind.Utc)
        {
            trip.ActualEndDate = trip.ActualEndDate.Value.ToUniversalTime();
        }
        trip.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        var updatedTrip = await _context.Trips
            .Include(t => t.Customer)
            .Include(t => t.Truck)
            .Include(t => t.Driver)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        return _mapper.Map<TripDto>(updatedTrip!);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var trip = await _context.Trips.FindAsync(id);
        if (trip == null) return false;

        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync();
        return true;
    }
}

