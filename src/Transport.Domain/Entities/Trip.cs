namespace Transport.Domain.Entities;

public class Trip : BaseEntity
{
    public string TripNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid TruckId { get; set; }
    public Guid DriverId { get; set; }
    public Guid? DispatcherId { get; set; } // User who created/dispatched
    
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled
    public decimal Distance { get; set; } // km
    public decimal? FuelCost { get; set; }
    public decimal? OtherCosts { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Truck Truck { get; set; } = null!;
    public virtual Driver Driver { get; set; } = null!;
    public virtual User? Dispatcher { get; set; }
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

