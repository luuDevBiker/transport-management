namespace Transport.Domain.Entities;

public class Driver : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiryDate { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "Available"; // Available, OnTrip, OffDuty, Inactive
    public Guid? AssignedTruckId { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}

