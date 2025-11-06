namespace Transport.Application.DTOs.Truck;

public class CreateTruckDto
{
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Capacity { get; set; }
    public string Status { get; set; } = "Available";
    public DateTime? LastMaintenanceDate { get; set; }
    public int? MaintenanceIntervalDays { get; set; }
    public string? Notes { get; set; }
}

