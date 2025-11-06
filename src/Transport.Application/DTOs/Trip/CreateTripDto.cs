namespace Transport.Application.DTOs.Trip;

public class CreateTripDto
{
    public Guid CustomerId { get; set; }
    public Guid TruckId { get; set; }
    public Guid DriverId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public decimal Distance { get; set; }
    public string? Notes { get; set; }
}

