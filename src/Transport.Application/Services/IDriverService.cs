using Transport.Application.DTOs.Driver;

namespace Transport.Application.Services;

public interface IDriverService
{
    Task<IEnumerable<DriverDto>> GetAllAsync();
    Task<DriverDto?> GetByIdAsync(Guid id);
    Task<DriverDto> CreateAsync(CreateDriverDto dto);
    Task<DriverDto?> UpdateAsync(Guid id, CreateDriverDto dto);
    Task<bool> DeleteAsync(Guid id);
}

