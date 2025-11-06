using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport.Application.DTOs.Report;
using Transport.Application.Services;

namespace Transport.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("revenue")]
    [Authorize(Roles = "Admin,Accountant")]
    public async Task<ActionResult<RevenueReportDto>> GetRevenueReport(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = toDate ?? DateTime.UtcNow;
        
        var report = await _reportService.GetRevenueReportAsync(from, to);
        return Ok(report);
    }

    [HttpGet("debt")]
    [Authorize(Roles = "Admin,Accountant")]
    public async Task<ActionResult<IEnumerable<DebtReportDto>>> GetDebtReport()
    {
        var report = await _reportService.GetDebtReportAsync();
        return Ok(report);
    }

    [HttpGet("trip-status")]
    public async Task<ActionResult<IEnumerable<TripStatusReportDto>>> GetTripStatusReport()
    {
        var report = await _reportService.GetTripStatusReportAsync();
        return Ok(report);
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var report = await _reportService.GetDashboardAsync();
        return Ok(report);
    }

    [HttpGet("revenue-detail")]
    [Authorize(Roles = "Admin,Accountant")]
    public async Task<ActionResult<RevenueDetailReportDto>> GetRevenueDetailReport(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string periodType = "month")
    {
        var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = toDate ?? DateTime.UtcNow;
        
        var report = await _reportService.GetRevenueDetailReportAsync(from, to, periodType);
        return Ok(report);
    }

    [HttpGet("trip-detail")]
    public async Task<ActionResult<TripDetailReportDto>> GetTripDetailReport(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = toDate ?? DateTime.UtcNow;
        
        var report = await _reportService.GetTripDetailReportAsync(from, to);
        return Ok(report);
    }

    [HttpGet("truck")]
    public async Task<ActionResult<TruckReportDto>> GetTruckReport()
    {
        var report = await _reportService.GetTruckReportAsync();
        return Ok(report);
    }

    [HttpGet("driver")]
    public async Task<ActionResult<DriverReportDto>> GetDriverReport()
    {
        var report = await _reportService.GetDriverReportAsync();
        return Ok(report);
    }

    [HttpGet("customer")]
    [Authorize(Roles = "Admin,Accountant")]
    public async Task<ActionResult<CustomerReportDto>> GetCustomerReport()
    {
        var report = await _reportService.GetCustomerReportAsync();
        return Ok(report);
    }
}

