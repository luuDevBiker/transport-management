using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Transport.Application.DTOs.Payment;
using Transport.Domain.Entities;
using Transport.Application.Interfaces;

namespace Transport.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PaymentService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllAsync()
    {
        var payments = await _context.Payments
            .Include(p => p.Invoice)
            .ToListAsync();
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<PaymentDto?> GetByIdAsync(Guid id)
    {
        var payment = await _context.Payments
            .Include(p => p.Invoice)
            .FirstOrDefaultAsync(p => p.Id == id);
        return payment == null ? null : _mapper.Map<PaymentDto>(payment);
    }

    public async Task<IEnumerable<PaymentDto>> GetByInvoiceIdAsync(Guid invoiceId)
    {
        var payments = await _context.Payments
            .Include(p => p.Invoice)
            .Where(p => p.InvoiceId == invoiceId)
            .ToListAsync();
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
    {
        var payment = _mapper.Map<Payment>(dto);
        payment.PaymentNumber = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        
        // Ensure PaymentDate is UTC
        if (payment.PaymentDate.Kind != DateTimeKind.Utc)
        {
            payment.PaymentDate = payment.PaymentDate.ToUniversalTime();
        }
        
        _context.Payments.Add(payment);
        
        // Update invoice status
        var invoice = await _context.Invoices
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == dto.InvoiceId);
        
        if (invoice != null)
        {
            var totalPaid = invoice.Payments.Sum(p => p.Amount) + payment.Amount;
            if (totalPaid >= invoice.TotalAmount)
                invoice.Status = "Paid";
            else if (totalPaid > 0)
                invoice.Status = "Partial";
            
            invoice.UpdatedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
        
        var createdPayment = await _context.Payments
            .Include(p => p.Invoice)
            .FirstOrDefaultAsync(p => p.Id == payment.Id);
        
        return _mapper.Map<PaymentDto>(createdPayment!);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var payment = await _context.Payments
            .Include(p => p.Invoice)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (payment == null) return false;

        var invoice = payment.Invoice;
        _context.Payments.Remove(payment);
        
        // Update invoice status
        if (invoice != null)
        {
            var totalPaid = invoice.Payments.Where(p => p.Id != id).Sum(p => p.Amount);
            if (totalPaid >= invoice.TotalAmount)
                invoice.Status = "Paid";
            else if (totalPaid > 0)
                invoice.Status = "Partial";
            else if (invoice.DueDate < DateTime.UtcNow)
                invoice.Status = "Overdue";
            else
                invoice.Status = "Pending";
            
            invoice.UpdatedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
        return true;
    }
}

