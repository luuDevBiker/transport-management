namespace Transport.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

