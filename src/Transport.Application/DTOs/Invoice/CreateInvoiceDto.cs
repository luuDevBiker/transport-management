namespace Transport.Application.DTOs.Invoice;

public class CreateInvoiceDto
{
    public Guid CustomerId { get; set; }
    public Guid? TripId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}

