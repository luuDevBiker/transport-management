using Transport.Application.DTOs.Report;

namespace Transport.Application.Services;

public interface IReportService
{
    // Existing reports
    Task<RevenueReportDto> GetRevenueReportAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<DebtReportDto>> GetDebtReportAsync();
    Task<IEnumerable<TripStatusReportDto>> GetTripStatusReportAsync();
    
    // New detailed reports
    Task<DashboardDto> GetDashboardAsync();
    Task<RevenueDetailReportDto> GetRevenueDetailReportAsync(DateTime fromDate, DateTime toDate, string periodType = "month");
    Task<TripDetailReportDto> GetTripDetailReportAsync(DateTime fromDate, DateTime toDate);
    Task<TruckReportDto> GetTruckReportAsync();
    Task<DriverReportDto> GetDriverReportAsync();
    Task<CustomerReportDto> GetCustomerReportAsync();
}

