namespace Transport.Domain.Entities;

public class Truck : BaseEntity
{
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Capacity { get; set; } // Tấn
    public string Status { get; set; } = "Available"; // Available, InUse, Maintenance, Inactive
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public int? MaintenanceIntervalDays { get; set; } // Số ngày giữa các lần bảo dưỡng
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}

