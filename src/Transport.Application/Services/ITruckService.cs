using Transport.Application.DTOs.Truck;

namespace Transport.Application.Services;

public interface ITruckService
{
    Task<IEnumerable<TruckDto>> GetAllAsync();
    Task<TruckDto?> GetByIdAsync(Guid id);
    Task<TruckDto> CreateAsync(CreateTruckDto dto);
    Task<TruckDto?> UpdateAsync(Guid id, CreateTruckDto dto);
    Task<bool> DeleteAsync(Guid id);
}

