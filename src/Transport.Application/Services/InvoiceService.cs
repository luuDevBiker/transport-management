using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Transport.Application.DTOs.Invoice;
using Transport.Domain.Entities;
using Transport.Application.Interfaces;

namespace Transport.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public InvoiceService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
    {
        var invoices = await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Trip)
            .Include(i => i.Payments)
            .ToListAsync();
        return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Trip)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id);
        return invoice == null ? null : _mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<IEnumerable<InvoiceDto>> GetByCustomerIdAsync(Guid customerId)
    {
        var invoices = await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Trip)
            .Include(i => i.Payments)
            .Where(i => i.CustomerId == customerId)
            .ToListAsync();
        return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
    }

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto)
    {
        var invoice = _mapper.Map<Invoice>(dto);
        invoice.InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        invoice.TotalAmount = invoice.Amount + (invoice.TaxAmount ?? 0);
        invoice.Status = "Pending";
        
        // Ensure dates are UTC
        if (invoice.IssueDate.Kind != DateTimeKind.Utc)
        {
            invoice.IssueDate = invoice.IssueDate.ToUniversalTime();
        }
        if (invoice.DueDate.Kind != DateTimeKind.Utc)
        {
            invoice.DueDate = invoice.DueDate.ToUniversalTime();
        }
        
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        
        var createdInvoice = await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Trip)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == invoice.Id);
        
        return _mapper.Map<InvoiceDto>(createdInvoice!);
    }

    public async Task<InvoiceDto?> UpdateAsync(Guid id, CreateInvoiceDto dto)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (invoice == null) return null;

        _mapper.Map(dto, invoice);
        invoice.TotalAmount = invoice.Amount + (invoice.TaxAmount ?? 0);
        
        // Ensure dates are UTC
        if (invoice.IssueDate.Kind != DateTimeKind.Utc)
        {
            invoice.IssueDate = invoice.IssueDate.ToUniversalTime();
        }
        if (invoice.DueDate.Kind != DateTimeKind.Utc)
        {
            invoice.DueDate = invoice.DueDate.ToUniversalTime();
        }
        
        var paidAmount = invoice.Payments.Sum(p => p.Amount);
        if (paidAmount >= invoice.TotalAmount)
            invoice.Status = "Paid";
        else if (paidAmount > 0)
            invoice.Status = "Partial";
        else if (invoice.DueDate < DateTime.UtcNow)
            invoice.Status = "Overdue";
        else
            invoice.Status = "Pending";
        
        invoice.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        var updatedInvoice = await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Trip)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id);
        
        return _mapper.Map<InvoiceDto>(updatedInvoice!);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return true;
    }
}

