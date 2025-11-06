using Transport.Application.DTOs.Payment;

namespace Transport.Application.Services;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllAsync();
    Task<PaymentDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PaymentDto>> GetByInvoiceIdAsync(Guid invoiceId);
    Task<PaymentDto> CreateAsync(CreatePaymentDto dto);
    Task<bool> DeleteAsync(Guid id);
}

