namespace Transport.Application.DTOs.Report;

public class RevenueDetailReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public RevenueSummaryDto Summary { get; set; } = new();
    public List<RevenueByPeriodDto> RevenueByPeriod { get; set; } = new(); // Theo ngày/tháng/quý
    public List<RevenueByCustomerDto> RevenueByCustomer { get; set; } = new();
    public List<RevenueByTripDto> RevenueByTrip { get; set; } = new();
    public RevenueTrendDto Trend { get; set; } = new();
}

public class RevenueSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal AverageInvoiceAmount { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int PendingInvoices { get; set; }
    public int OverdueInvoices { get; set; }
}

public class RevenueByPeriodDto
{
    public string Period { get; set; } = string.Empty; // "2024-01", "2024-Q1", etc.
    public DateTime PeriodDate { get; set; }
    public decimal Revenue { get; set; }
    public decimal Paid { get; set; }
    public decimal Outstanding { get; set; }
    public int InvoiceCount { get; set; }
}

public class RevenueByCustomerDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal RemainingDebt { get; set; }
    public int InvoiceCount { get; set; }
    public int TripCount { get; set; }
}

public class RevenueByTripDto
{
    public Guid TripId { get; set; }
    public string TripNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Paid { get; set; }
    public decimal Outstanding { get; set; }
    public DateTime IssueDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class RevenueTrendDto
{
    public decimal CurrentPeriodRevenue { get; set; }
    public decimal PreviousPeriodRevenue { get; set; }
    public decimal GrowthRate { get; set; } // %
    public string Trend { get; set; } = string.Empty; // "up", "down", "stable"
}

