namespace Transport.Application.DTOs.Report;

public class TruckReportDto
{
    public TruckSummaryDto Summary { get; set; } = new();
    public List<TruckUtilizationDto> TruckUtilization { get; set; } = new();
    public List<TruckMaintenanceDto> MaintenanceSchedule { get; set; } = new();
    public List<TruckPerformanceDto> Performance { get; set; } = new();
}

public class TruckSummaryDto
{
    public int TotalTrucks { get; set; }
    public int AvailableTrucks { get; set; }
    public int InUseTrucks { get; set; }
    public int MaintenanceTrucks { get; set; }
    public int InactiveTrucks { get; set; }
    public int MaintenanceDue { get; set; }
    public int MaintenanceOverdue { get; set; }
    public decimal AverageUtilization { get; set; } // %
}

public class TruckUtilizationDto
{
    public Guid TruckId { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TripCount { get; set; }
    public int DaysInUse { get; set; }
    public decimal UtilizationRate { get; set; } // %
    public decimal TotalDistance { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class TruckMaintenanceDto
{
    public Guid TruckId { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public int? MaintenanceIntervalDays { get; set; }
    public int DaysUntilMaintenance { get; set; }
    public bool IsOverdue { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TruckPerformanceDto
{
    public Guid TruckId { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public int TripCount { get; set; }
    public decimal TotalDistance { get; set; }
    public decimal TotalFuelCost { get; set; }
    public decimal AverageFuelCostPerKm { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal ProfitMargin { get; set; } // %
    public decimal AverageTripDistance { get; set; }
}

