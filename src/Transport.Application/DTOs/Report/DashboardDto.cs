namespace Transport.Application.DTOs.Report;

public class DashboardDto
{
    // Tổng quan
    public DashboardSummaryDto Summary { get; set; } = new();
    public DashboardRevenueDto Revenue { get; set; } = new();
    public DashboardTripDto Trips { get; set; } = new();
    public DashboardDebtDto Debt { get; set; } = new();
    public List<DashboardRecentTripDto> RecentTrips { get; set; } = new();
    public List<DashboardTopCustomerDto> TopCustomers { get; set; } = new();
    public List<DashboardTruckStatusDto> TruckStatus { get; set; } = new();
}

public class DashboardSummaryDto
{
    public int TotalCustomers { get; set; }
    public int TotalTrucks { get; set; }
    public int TotalDrivers { get; set; }
    public int ActiveTrips { get; set; }
    public int PendingInvoices { get; set; }
    public decimal TotalOutstandingDebt { get; set; }
}

public class DashboardRevenueDto
{
    public decimal TodayRevenue { get; set; }
    public decimal ThisWeekRevenue { get; set; }
    public decimal ThisMonthRevenue { get; set; }
    public decimal ThisYearRevenue { get; set; }
    public decimal LastMonthRevenue { get; set; }
    public decimal RevenueGrowth { get; set; } // % tăng trưởng so với tháng trước
}

public class DashboardTripDto
{
    public int TodayTrips { get; set; }
    public int ThisWeekTrips { get; set; }
    public int ThisMonthTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int ScheduledTrips { get; set; }
    public decimal AverageDistance { get; set; }
    public decimal TotalDistance { get; set; }
}

public class DashboardDebtDto
{
    public decimal TotalDebt { get; set; }
    public decimal OverdueDebt { get; set; }
    public int OverdueInvoices { get; set; }
    public int PendingInvoices { get; set; }
    public int PaidInvoices { get; set; }
}

public class DashboardRecentTripDto
{
    public Guid Id { get; set; }
    public string TripNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public decimal? Distance { get; set; }
}

public class DashboardTopCustomerDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int TripCount { get; set; }
    public decimal RemainingDebt { get; set; }
}

public class DashboardTruckStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public int MaintenanceDue { get; set; }
}

