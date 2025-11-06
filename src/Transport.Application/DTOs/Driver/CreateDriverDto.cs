namespace Transport.Application.DTOs.Driver;

public class CreateDriverDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiryDate { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "Available";
    public string? Notes { get; set; }
}

