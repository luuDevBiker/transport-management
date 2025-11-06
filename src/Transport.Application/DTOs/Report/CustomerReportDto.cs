namespace Transport.Application.DTOs.Report;

public class CustomerReportDto
{
    public CustomerSummaryDto Summary { get; set; } = new();
    public List<CustomerDetailDto> CustomerDetails { get; set; } = new();
    public List<CustomerRevenueDto> CustomerRevenue { get; set; } = new();
    public List<CustomerDebtDto> CustomerDebt { get; set; } = new();
}

public class CustomerSummaryDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; } // Có chuyến hàng trong 30 ngày
    public int InactiveCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalDebt { get; set; }
    public decimal AverageRevenuePerCustomer { get; set; }
    public int TotalTrips { get; set; }
}

public class CustomerDetailDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int TripCount { get; set; }
    public int InvoiceCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal RemainingDebt { get; set; }
    public DateTime? LastTripDate { get; set; }
    public DateTime? LastInvoiceDate { get; set; }
    public bool IsActive { get; set; }
}

public class CustomerRevenueDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty; // "2024-01", "2024-Q1", etc.
    public DateTime PeriodDate { get; set; }
    public decimal Revenue { get; set; }
    public decimal Paid { get; set; }
    public decimal Outstanding { get; set; }
    public int TripCount { get; set; }
    public int InvoiceCount { get; set; }
}

public class CustomerDebtDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalDebt { get; set; }
    public decimal OverdueDebt { get; set; }
    public int TotalInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public int PendingInvoices { get; set; }
    public DateTime? OldestInvoiceDate { get; set; }
    public int DaysOverdue { get; set; }
}

