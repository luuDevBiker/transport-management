using Transport.Application.DTOs.Trip;

namespace Transport.Application.Services;

public interface ITripService
{
    Task<IEnumerable<TripDto>> GetAllAsync();
    Task<TripDto?> GetByIdAsync(Guid id);
    Task<TripDto> CreateAsync(CreateTripDto dto, Guid? dispatcherId = null);
    Task<TripDto?> UpdateAsync(Guid id, CreateTripDto dto);
    Task<TripDto?> UpdateStatusAsync(Guid id, string status);
    Task<bool> DeleteAsync(Guid id);
}

