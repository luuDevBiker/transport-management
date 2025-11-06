using Transport.Application.DTOs.Invoice;

namespace Transport.Application.Services;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllAsync();
    Task<InvoiceDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<InvoiceDto>> GetByCustomerIdAsync(Guid customerId);
    Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto);
    Task<InvoiceDto?> UpdateAsync(Guid id, CreateInvoiceDto dto);
    Task<bool> DeleteAsync(Guid id);
}

