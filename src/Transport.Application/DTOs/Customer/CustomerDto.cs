namespace Transport.Application.DTOs.Customer;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

