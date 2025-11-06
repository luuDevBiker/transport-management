namespace Transport.Application.DTOs.Trip;

public class TripDto
{
    public Guid Id { get; set; }
    public string TripNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid TruckId { get; set; }
    public string TruckLicensePlate { get; set; } = string.Empty;
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public Guid? DispatcherId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Distance { get; set; }
    public decimal? FuelCost { get; set; }
    public decimal? OtherCosts { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

