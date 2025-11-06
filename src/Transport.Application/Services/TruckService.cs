using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Transport.Application.DTOs.Truck;
using Transport.Domain.Entities;
using Transport.Application.Interfaces;

namespace Transport.Application.Services;

public class TruckService : ITruckService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TruckService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TruckDto>> GetAllAsync()
    {
        var trucks = await _context.Trucks.ToListAsync();
        return _mapper.Map<IEnumerable<TruckDto>>(trucks);
    }

    public async Task<TruckDto?> GetByIdAsync(Guid id)
    {
        var truck = await _context.Trucks.FindAsync(id);
        return truck == null ? null : _mapper.Map<TruckDto>(truck);
    }

    public async Task<TruckDto> CreateAsync(CreateTruckDto dto)
    {
        var truck = _mapper.Map<Truck>(dto);
        if (truck.MaintenanceIntervalDays.HasValue && truck.LastMaintenanceDate.HasValue)
        {
            truck.NextMaintenanceDate = truck.LastMaintenanceDate.Value.AddDays(truck.MaintenanceIntervalDays.Value);
        }
        _context.Trucks.Add(truck);
        await _context.SaveChangesAsync();
        return _mapper.Map<TruckDto>(truck);
    }

    public async Task<TruckDto?> UpdateAsync(Guid id, CreateTruckDto dto)
    {
        var truck = await _context.Trucks.FindAsync(id);
        if (truck == null) return null;

        _mapper.Map(dto, truck);
        if (truck.MaintenanceIntervalDays.HasValue && truck.LastMaintenanceDate.HasValue)
        {
            truck.NextMaintenanceDate = truck.LastMaintenanceDate.Value.AddDays(truck.MaintenanceIntervalDays.Value);
        }
        truck.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return _mapper.Map<TruckDto>(truck);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var truck = await _context.Trucks.FindAsync(id);
        if (truck == null) return false;

        _context.Trucks.Remove(truck);
        await _context.SaveChangesAsync();
        return true;
    }
}

