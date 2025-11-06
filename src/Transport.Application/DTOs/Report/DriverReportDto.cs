namespace Transport.Application.DTOs.Report;

public class DriverReportDto
{
    public DriverSummaryDto Summary { get; set; } = new();
    public List<DriverPerformanceDto> DriverPerformance { get; set; } = new();
    public List<DriverTripDto> DriverTrips { get; set; } = new();
}

public class DriverSummaryDto
{
    public int TotalDrivers { get; set; }
    public int AvailableDrivers { get; set; }
    public int OnTripDrivers { get; set; }
    public int OffDutyDrivers { get; set; }
    public int InactiveDrivers { get; set; }
    public int LicenseExpiringSoon { get; set; } // Trong 30 ngày
    public int LicenseExpired { get; set; }
}

public class DriverPerformanceDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int InProgressTrips { get; set; }
    public decimal CompletionRate { get; set; } // %
    public decimal TotalDistance { get; set; }
    public decimal AverageDistance { get; set; }
    public decimal TotalRevenue { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    public int DaysUntilLicenseExpiry { get; set; }
    public bool IsLicenseExpiringSoon { get; set; }
    public bool IsLicenseExpired { get; set; }
}

public class DriverTripDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public Guid TripId { get; set; }
    public string TripNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal? Distance { get; set; }
    public decimal? Revenue { get; set; }
}

