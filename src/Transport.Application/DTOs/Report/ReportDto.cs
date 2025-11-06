namespace Transport.Application.DTOs.Report;

public class RevenueReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalOutstanding { get; set; }
    public int TotalTrips { get; set; }
    public int CompletedTrips { get; set; }
}

public class DebtReportDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalDebt { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal RemainingDebt { get; set; }
    public int InvoiceCount { get; set; }
    public DateTime? OldestInvoiceDate { get; set; }
    public int DaysOverdue { get; set; }
}

public class TripStatusReportDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalDistance { get; set; }
}

