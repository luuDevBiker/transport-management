using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Transport.Application.DTOs.Driver;
using Transport.Domain.Entities;
using Transport.Application.Interfaces;

namespace Transport.Application.Services;

public class DriverService : IDriverService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DriverService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DriverDto>> GetAllAsync()
    {
        var drivers = await _context.Drivers.ToListAsync();
        return _mapper.Map<IEnumerable<DriverDto>>(drivers);
    }

    public async Task<DriverDto?> GetByIdAsync(Guid id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        return driver == null ? null : _mapper.Map<DriverDto>(driver);
    }

    public async Task<DriverDto> CreateAsync(CreateDriverDto dto)
    {
        var driver = _mapper.Map<Driver>(dto);
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        return _mapper.Map<DriverDto>(driver);
    }

    public async Task<DriverDto?> UpdateAsync(Guid id, CreateDriverDto dto)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null) return null;

        _mapper.Map(dto, driver);
        
        // Ensure LicenseExpiryDate is UTC
        if (driver.LicenseExpiryDate.Kind != DateTimeKind.Utc)
        {
            driver.LicenseExpiryDate = driver.LicenseExpiryDate.ToUniversalTime();
        }
        
        driver.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return _mapper.Map<DriverDto>(driver);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null) return false;

        _context.Drivers.Remove(driver);
        await _context.SaveChangesAsync();
        return true;
    }
}

