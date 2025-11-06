namespace Transport.Domain.Entities;

public class Payment : BaseEntity
{
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid InvoiceId { get; set; }
    
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Cash, BankTransfer, Check
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual Invoice Invoice { get; set; } = null!;
}

