using Transport.Application.DTOs.Customer;

namespace Transport.Application.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerDto?> UpdateAsync(Guid id, CreateCustomerDto dto);
    Task<bool> DeleteAsync(Guid id);
}

