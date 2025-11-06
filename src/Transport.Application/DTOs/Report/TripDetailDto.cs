namespace Transport.Application.DTOs.Report;

public class TripDetailReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public TripSummaryDto Summary { get; set; } = new();
    public List<TripByStatusDto> TripsByStatus { get; set; } = new();
    public List<TripByPeriodDto> TripsByPeriod { get; set; } = new();
    public List<TripByTruckDto> TripsByTruck { get; set; } = new();
    public List<TripByDriverDto> TripsByDriver { get; set; } = new();
    public List<TripByCustomerDto> TripsByCustomer { get; set; } = new();
}

public class TripSummaryDto
{
    public int TotalTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int ScheduledTrips { get; set; }
    public int CancelledTrips { get; set; }
    public decimal TotalDistance { get; set; }
    public decimal AverageDistance { get; set; }
    public decimal TotalFuelCost { get; set; }
    public decimal AverageFuelCost { get; set; }
    public decimal TotalOtherCosts { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRevenue { get; set; }
}

public class TripByStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalDistance { get; set; }
    public decimal AverageDistance { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class TripByPeriodDto
{
    public string Period { get; set; } = string.Empty;
    public DateTime PeriodDate { get; set; }
    public int TripCount { get; set; }
    public int CompletedCount { get; set; }
    public decimal TotalDistance { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class TripByTruckDto
{
    public Guid TruckId { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int TripCount { get; set; }
    public int CompletedCount { get; set; }
    public decimal TotalDistance { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal UtilizationRate { get; set; } // %
}

public class TripByDriverDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int TripCount { get; set; }
    public int CompletedCount { get; set; }
    public decimal TotalDistance { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRating { get; set; }
}

public class TripByCustomerDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int TripCount { get; set; }
    public int CompletedCount { get; set; }
    public decimal TotalDistance { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRevenue { get; set; }
}

