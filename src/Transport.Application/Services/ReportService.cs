using Microsoft.EntityFrameworkCore;
using Transport.Application.DTOs.Report;
using Transport.Application.Interfaces;

namespace Transport.Application.Services;

public class ReportService : IReportService
{
    private readonly IApplicationDbContext _context;

    public ReportService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime fromDate, DateTime toDate)
    {
        var invoices = await _context.Invoices
            .Include(i => i.Payments)
            .Where(i => i.IssueDate >= fromDate && i.IssueDate <= toDate)
            .ToListAsync();

        var trips = await _context.Trips
            .Where(t => t.ScheduledDate >= fromDate && t.ScheduledDate <= toDate)
            .ToListAsync();

        return new RevenueReportDto
        {
            FromDate = fromDate,
            ToDate = toDate,
            TotalRevenue = invoices.Sum(i => i.TotalAmount),
            TotalPaid = invoices.Sum(i => i.Payments.Sum(p => p.Amount)),
            TotalOutstanding = invoices.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount)),
            TotalTrips = trips.Count,
            CompletedTrips = trips.Count(t => t.Status == "Completed")
        };
    }

    public async Task<IEnumerable<DebtReportDto>> GetDebtReportAsync()
    {
        var customers = await _context.Customers
            .Include(c => c.Invoices)
                .ThenInclude(i => i.Payments)
            .ToListAsync();

        var now = DateTime.UtcNow;
        return customers.Select(c =>
        {
            var invoices = c.Invoices.ToList();
            var totalDebt = invoices.Sum(i => i.TotalAmount);
            var totalPaid = invoices.Sum(i => i.Payments.Sum(p => p.Amount));
            var remainingDebt = totalDebt - totalPaid;
            var overdueInvoices = invoices.Where(i => i.DueDate < now && i.Status != "Paid").ToList();
            var oldestInvoice = overdueInvoices.OrderBy(i => i.DueDate).FirstOrDefault();
            
            return new DebtReportDto
            {
                CustomerId = c.Id,
                CustomerName = c.Name,
                TotalDebt = totalDebt,
                TotalPaid = totalPaid,
                RemainingDebt = remainingDebt,
                InvoiceCount = invoices.Count,
                OldestInvoiceDate = oldestInvoice?.DueDate,
                DaysOverdue = oldestInvoice != null ? (int)(now - oldestInvoice.DueDate).TotalDays : 0
            };
        }).Where(d => d.RemainingDebt > 0).ToList();
    }

    public async Task<IEnumerable<TripStatusReportDto>> GetTripStatusReportAsync()
    {
        var trips = await _context.Trips.ToListAsync();

        return trips
            .GroupBy(t => t.Status)
            .Select(g => new TripStatusReportDto
            {
                Status = g.Key,
                Count = g.Count(),
                TotalDistance = g.Sum(t => t.Distance)
            })
            .ToList();
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var thisWeekStart = today.AddDays(-(int)today.DayOfWeek);
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);
        var lastMonthStart = thisMonthStart.AddMonths(-1);
        var lastMonthEnd = thisMonthStart.AddDays(-1);
        var thisYearStart = new DateTime(now.Year, 1, 1);

        // Summary
        var totalCustomers = await _context.Customers.CountAsync();
        var totalTrucks = await _context.Trucks.CountAsync();
        var totalDrivers = await _context.Drivers.CountAsync();
        var activeTrips = await _context.Trips.CountAsync(t => t.Status == "InProgress");
        var pendingInvoices = await _context.Invoices.CountAsync(i => i.Status == "Pending" || i.Status == "Partial");
        var totalOutstandingDebt = await _context.Invoices
            .Include(i => i.Payments)
            .ToListAsync();
        var outstandingDebt = totalOutstandingDebt.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount));

        // Revenue
        var allInvoices = await _context.Invoices
            .Include(i => i.Payments)
            .ToListAsync();
        
        var todayInvoices = allInvoices.Where(i => i.IssueDate.Date == today).ToList();
        var thisWeekInvoices = allInvoices.Where(i => i.IssueDate >= thisWeekStart).ToList();
        var thisMonthInvoices = allInvoices.Where(i => i.IssueDate >= thisMonthStart).ToList();
        var lastMonthInvoices = allInvoices.Where(i => i.IssueDate >= lastMonthStart && i.IssueDate <= lastMonthEnd).ToList();
        var thisYearInvoices = allInvoices.Where(i => i.IssueDate >= thisYearStart).ToList();

        var todayRevenue = todayInvoices.Sum(i => i.Payments.Sum(p => p.Amount));
        var thisWeekRevenue = thisWeekInvoices.Sum(i => i.Payments.Sum(p => p.Amount));
        var thisMonthRevenue = thisMonthInvoices.Sum(i => i.Payments.Sum(p => p.Amount));
        var lastMonthRevenue = lastMonthInvoices.Sum(i => i.Payments.Sum(p => p.Amount));
        var thisYearRevenue = thisYearInvoices.Sum(i => i.Payments.Sum(p => p.Amount));
        var revenueGrowth = lastMonthRevenue > 0 ? ((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 : 0;

        // Trips
        var allTrips = await _context.Trips.ToListAsync();
        var todayTrips = allTrips.Where(t => t.ScheduledDate.Date == today).Count();
        var thisWeekTrips = allTrips.Where(t => t.ScheduledDate >= thisWeekStart).Count();
        var thisMonthTrips = allTrips.Where(t => t.ScheduledDate >= thisMonthStart).Count();
        var completedTrips = allTrips.Count(t => t.Status == "Completed");
        var inProgressTrips = allTrips.Count(t => t.Status == "InProgress");
        var scheduledTrips = allTrips.Count(t => t.Status == "Scheduled");
        var totalDistance = allTrips.Sum(t => t.Distance);
        var averageDistance = allTrips.Any() ? totalDistance / allTrips.Count : 0;

        // Debt
        var overdueInvoices = allInvoices.Where(i => i.DueDate < now && i.Status != "Paid").ToList();
        var overdueDebt = overdueInvoices.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount));
        var paidInvoices = allInvoices.Count(i => i.Status == "Paid");

        // Recent Trips
        var recentTrips = await _context.Trips
            .Include(t => t.Customer)
            .OrderByDescending(t => t.ScheduledDate)
            .Take(10)
            .Select(t => new DashboardRecentTripDto
            {
                Id = t.Id,
                TripNumber = t.TripNumber,
                CustomerName = t.Customer.Name,
                Origin = t.Origin,
                Destination = t.Destination,
                Status = t.Status,
                ScheduledDate = t.ScheduledDate,
                Distance = t.Distance
            })
            .ToListAsync();

        // Top Customers
        var topCustomers = await _context.Customers
            .Include(c => c.Invoices)
                .ThenInclude(i => i.Payments)
            .Include(c => c.Trips)
            .ToListAsync();
        
        var topCustomersList = topCustomers
            .Select(c => new DashboardTopCustomerDto
            {
                CustomerId = c.Id,
                CustomerName = c.Name,
                TotalRevenue = c.Invoices.Sum(i => i.TotalAmount),
                TripCount = c.Trips.Count,
                RemainingDebt = c.Invoices.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount))
            })
            .OrderByDescending(c => c.TotalRevenue)
            .Take(10)
            .ToList();

        // Truck Status
        var trucks = await _context.Trucks.ToListAsync();
        var truckStatus = trucks
            .GroupBy(t => t.Status)
            .Select(g => new DashboardTruckStatusDto
            {
                Status = g.Key,
                Count = g.Count(),
                MaintenanceDue = g.Count(t => t.NextMaintenanceDate.HasValue && t.NextMaintenanceDate.Value <= now.AddDays(7))
            })
            .ToList();

        return new DashboardDto
        {
            Summary = new DashboardSummaryDto
            {
                TotalCustomers = totalCustomers,
                TotalTrucks = totalTrucks,
                TotalDrivers = totalDrivers,
                ActiveTrips = activeTrips,
                PendingInvoices = pendingInvoices,
                TotalOutstandingDebt = outstandingDebt
            },
            Revenue = new DashboardRevenueDto
            {
                TodayRevenue = todayRevenue,
                ThisWeekRevenue = thisWeekRevenue,
                ThisMonthRevenue = thisMonthRevenue,
                ThisYearRevenue = thisYearRevenue,
                LastMonthRevenue = lastMonthRevenue,
                RevenueGrowth = revenueGrowth
            },
            Trips = new DashboardTripDto
            {
                TodayTrips = todayTrips,
                ThisWeekTrips = thisWeekTrips,
                ThisMonthTrips = thisMonthTrips,
                CompletedTrips = completedTrips,
                InProgressTrips = inProgressTrips,
                ScheduledTrips = scheduledTrips,
                AverageDistance = averageDistance,
                TotalDistance = totalDistance
            },
            Debt = new DashboardDebtDto
            {
                TotalDebt = outstandingDebt,
                OverdueDebt = overdueDebt,
                OverdueInvoices = overdueInvoices.Count,
                PendingInvoices = pendingInvoices,
                PaidInvoices = paidInvoices
            },
            RecentTrips = recentTrips,
            TopCustomers = topCustomersList,
            TruckStatus = truckStatus
        };
    }

    public async Task<RevenueDetailReportDto> GetRevenueDetailReportAsync(DateTime fromDate, DateTime toDate, string periodType = "month")
    {
        var invoices = await _context.Invoices
            .Include(i => i.Payments)
            .Include(i => i.Customer)
            .Include(i => i.Trip)
            .Where(i => i.IssueDate >= fromDate && i.IssueDate <= toDate)
            .ToListAsync();

        var summary = new RevenueSummaryDto
        {
            TotalRevenue = invoices.Sum(i => i.TotalAmount),
            TotalPaid = invoices.Sum(i => i.Payments.Sum(p => p.Amount)),
            TotalOutstanding = invoices.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount)),
            TotalInvoices = invoices.Count,
            PaidInvoices = invoices.Count(i => i.Status == "Paid"),
            PendingInvoices = invoices.Count(i => i.Status == "Pending" || i.Status == "Partial"),
            OverdueInvoices = invoices.Count(i => i.DueDate < DateTime.UtcNow && i.Status != "Paid")
        };
        summary.AverageInvoiceAmount = summary.TotalInvoices > 0 ? summary.TotalRevenue / summary.TotalInvoices : 0;

        // Revenue by Period
        var revenueByPeriod = new List<RevenueByPeriodDto>();
        var current = fromDate;
        while (current <= toDate)
        {
            DateTime periodEnd;
            string periodLabel;
            
            if (periodType == "day")
            {
                periodEnd = current.AddDays(1);
                periodLabel = current.ToString("yyyy-MM-dd");
            }
            else if (periodType == "week")
            {
                periodEnd = current.AddDays(7);
                periodLabel = $"Week {GetWeekOfYear(current)}";
            }
            else if (periodType == "quarter")
            {
                var quarter = (current.Month - 1) / 3 + 1;
                periodEnd = new DateTime(current.Year, quarter * 3, 1).AddMonths(1);
                periodLabel = $"{current.Year}-Q{quarter}";
            }
            else // month
            {
                periodEnd = current.AddMonths(1);
                periodLabel = current.ToString("yyyy-MM");
            }

            var periodInvoices = invoices.Where(i => i.IssueDate >= current && i.IssueDate < periodEnd).ToList();
            revenueByPeriod.Add(new RevenueByPeriodDto
            {
                Period = periodLabel,
                PeriodDate = current,
                Revenue = periodInvoices.Sum(i => i.TotalAmount),
                Paid = periodInvoices.Sum(i => i.Payments.Sum(p => p.Amount)),
                Outstanding = periodInvoices.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount)),
                InvoiceCount = periodInvoices.Count
            });

            current = periodEnd;
        }

        // Revenue by Customer
        var revenueByCustomer = invoices
            .GroupBy(i => i.Customer)
            .Select(g => new RevenueByCustomerDto
            {
                CustomerId = g.Key.Id,
                CustomerName = g.Key.Name,
                TotalRevenue = g.Sum(i => i.TotalAmount),
                TotalPaid = g.Sum(i => i.Payments.Sum(p => p.Amount)),
                RemainingDebt = g.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount)),
                InvoiceCount = g.Count(),
                TripCount = g.Count(i => i.Trip != null)
            })
            .OrderByDescending(r => r.TotalRevenue)
            .ToList();

        // Revenue by Trip
        var revenueByTrip = invoices
            .Where(i => i.Trip != null)
            .Select(i => new RevenueByTripDto
            {
                TripId = i.Trip!.Id,
                TripNumber = i.Trip.TripNumber,
                CustomerName = i.Customer.Name,
                Revenue = i.TotalAmount,
                Paid = i.Payments.Sum(p => p.Amount),
                Outstanding = i.TotalAmount - i.Payments.Sum(p => p.Amount),
                IssueDate = i.IssueDate,
                Status = i.Status
            })
            .OrderByDescending(r => r.IssueDate)
            .ToList();

        // Trend
        var previousPeriodStart = periodType == "month" ? fromDate.AddMonths(-1) : fromDate.AddDays(-(int)(toDate - fromDate).TotalDays);
        var previousPeriodEnd = fromDate;
        var previousInvoices = await _context.Invoices
            .Include(i => i.Payments)
            .Where(i => i.IssueDate >= previousPeriodStart && i.IssueDate < previousPeriodEnd)
            .ToListAsync();
        var previousRevenue = previousInvoices.Sum(i => i.TotalAmount);
        var currentRevenue = summary.TotalRevenue;
        var growthRate = previousRevenue > 0 ? ((currentRevenue - previousRevenue) / previousRevenue) * 100 : 0;

        return new RevenueDetailReportDto
        {
            FromDate = fromDate,
            ToDate = toDate,
            Summary = summary,
            RevenueByPeriod = revenueByPeriod,
            RevenueByCustomer = revenueByCustomer,
            RevenueByTrip = revenueByTrip,
            Trend = new RevenueTrendDto
            {
                CurrentPeriodRevenue = currentRevenue,
                PreviousPeriodRevenue = previousRevenue,
                GrowthRate = growthRate,
                Trend = growthRate > 0 ? "up" : growthRate < 0 ? "down" : "stable"
            }
        };
    }

    public async Task<TripDetailReportDto> GetTripDetailReportAsync(DateTime fromDate, DateTime toDate)
    {
        var trips = await _context.Trips
            .Include(t => t.Customer)
            .Include(t => t.Truck)
            .Include(t => t.Driver)
            .Include(t => t.Invoices)
                .ThenInclude(i => i.Payments)
            .Where(t => t.ScheduledDate >= fromDate && t.ScheduledDate <= toDate)
            .ToListAsync();

        var summary = new TripSummaryDto
        {
            TotalTrips = trips.Count,
            CompletedTrips = trips.Count(t => t.Status == "Completed"),
            InProgressTrips = trips.Count(t => t.Status == "InProgress"),
            ScheduledTrips = trips.Count(t => t.Status == "Scheduled"),
            CancelledTrips = trips.Count(t => t.Status == "Cancelled"),
            TotalDistance = trips.Sum(t => t.Distance),
            TotalFuelCost = trips.Sum(t => t.FuelCost ?? 0),
            TotalOtherCosts = trips.Sum(t => t.OtherCosts ?? 0)
        };
        summary.AverageDistance = summary.TotalTrips > 0 ? summary.TotalDistance / summary.TotalTrips : 0;
        summary.AverageFuelCost = summary.TotalTrips > 0 ? summary.TotalFuelCost / summary.TotalTrips : 0;
        summary.TotalRevenue = trips.Sum(t => t.Invoices.Sum(i => i.TotalAmount));
        summary.AverageRevenue = summary.TotalTrips > 0 ? summary.TotalRevenue / summary.TotalTrips : 0;

        // Trips by Status
        var tripsByStatus = trips
            .GroupBy(t => t.Status)
            .Select(g => new TripByStatusDto
            {
                Status = g.Key,
                Count = g.Count(),
                TotalDistance = g.Sum(t => t.Distance),
                AverageDistance = g.Any() ? g.Sum(t => t.Distance) / g.Count() : 0,
                TotalRevenue = g.Sum(t => t.Invoices.Sum(i => i.TotalAmount))
            })
            .ToList();

        // Trips by Period (monthly)
        var tripsByPeriod = trips
            .GroupBy(t => new { t.ScheduledDate.Year, t.ScheduledDate.Month })
            .Select(g => new TripByPeriodDto
            {
                Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                PeriodDate = new DateTime(g.Key.Year, g.Key.Month, 1),
                TripCount = g.Count(),
                CompletedCount = g.Count(t => t.Status == "Completed"),
                TotalDistance = g.Sum(t => t.Distance),
                TotalRevenue = g.Sum(t => t.Invoices.Sum(i => i.TotalAmount))
            })
            .OrderBy(t => t.PeriodDate)
            .ToList();

        // Trips by Truck
        var tripsByTruck = trips
            .Where(t => t.Truck != null)
            .GroupBy(t => t.Truck)
            .Select(g => new TripByTruckDto
            {
                TruckId = g.Key!.Id,
                LicensePlate = g.Key.LicensePlate,
                Brand = g.Key.Brand,
                Model = g.Key.Model,
                TripCount = g.Count(),
                CompletedCount = g.Count(t => t.Status == "Completed"),
                TotalDistance = g.Sum(t => t.Distance),
                TotalRevenue = g.Sum(t => t.Invoices.Sum(i => i.TotalAmount)),
                UtilizationRate = 0 // TODO: Calculate based on days
            })
            .OrderByDescending(t => t.TripCount)
            .ToList();

        // Trips by Driver
        var tripsByDriver = trips
            .Where(t => t.Driver != null)
            .GroupBy(t => t.Driver)
            .Select(g => new TripByDriverDto
            {
                DriverId = g.Key!.Id,
                DriverName = g.Key.FullName,
                LicenseNumber = g.Key.LicenseNumber,
                TripCount = g.Count(),
                CompletedCount = g.Count(t => t.Status == "Completed"),
                TotalDistance = g.Sum(t => t.Distance),
                TotalRevenue = g.Sum(t => t.Invoices.Sum(i => i.TotalAmount)),
                AverageRating = 0 // TODO: Add rating field
            })
            .OrderByDescending(t => t.TripCount)
            .ToList();

        // Trips by Customer
        var tripsByCustomer = trips
            .Where(t => t.Customer != null)
            .GroupBy(t => t.Customer)
            .Select(g => new TripByCustomerDto
            {
                CustomerId = g.Key!.Id,
                CustomerName = g.Key.Name,
                TripCount = g.Count(),
                CompletedCount = g.Count(t => t.Status == "Completed"),
                TotalDistance = g.Sum(t => t.Distance),
                TotalRevenue = g.Sum(t => t.Invoices.Sum(i => i.TotalAmount)),
                AverageRevenue = g.Any() ? g.Sum(t => t.Invoices.Sum(i => i.TotalAmount)) / g.Count() : 0
            })
            .OrderByDescending(t => t.TripCount)
            .ToList();

        return new TripDetailReportDto
        {
            FromDate = fromDate,
            ToDate = toDate,
            Summary = summary,
            TripsByStatus = tripsByStatus,
            TripsByPeriod = tripsByPeriod,
            TripsByTruck = tripsByTruck,
            TripsByDriver = tripsByDriver,
            TripsByCustomer = tripsByCustomer
        };
    }

    public async Task<TruckReportDto> GetTruckReportAsync()
    {
        var trucks = await _context.Trucks
            .Include(t => t.Trips)
                .ThenInclude(tr => tr.Invoices)
                    .ThenInclude(i => i.Payments)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var summary = new TruckSummaryDto
        {
            TotalTrucks = trucks.Count,
            AvailableTrucks = trucks.Count(t => t.Status == "Available"),
            InUseTrucks = trucks.Count(t => t.Status == "InUse"),
            MaintenanceTrucks = trucks.Count(t => t.Status == "Maintenance"),
            InactiveTrucks = trucks.Count(t => t.Status == "Inactive"),
            MaintenanceDue = trucks.Count(t => t.NextMaintenanceDate.HasValue && t.NextMaintenanceDate.Value <= now.AddDays(7)),
            MaintenanceOverdue = trucks.Count(t => t.NextMaintenanceDate.HasValue && t.NextMaintenanceDate.Value < now)
        };

        var utilization = trucks
            .Select(t => new TruckUtilizationDto
            {
                TruckId = t.Id,
                LicensePlate = t.LicensePlate,
                Brand = t.Brand,
                Model = t.Model,
                Status = t.Status,
                TripCount = t.Trips.Count,
                DaysInUse = 0, // TODO: Calculate based on trip dates
                UtilizationRate = 0, // TODO: Calculate
                TotalDistance = t.Trips.Sum(tr => tr.Distance),
                TotalRevenue = t.Trips.Sum(tr => tr.Invoices.Sum(i => i.TotalAmount))
            })
            .OrderByDescending(t => t.TripCount)
            .ToList();

        summary.AverageUtilization = utilization.Any() ? utilization.Average(t => t.UtilizationRate) : 0;

        var maintenance = trucks
            .Select(t => new TruckMaintenanceDto
            {
                TruckId = t.Id,
                LicensePlate = t.LicensePlate,
                Brand = t.Brand,
                Model = t.Model,
                LastMaintenanceDate = t.LastMaintenanceDate,
                NextMaintenanceDate = t.NextMaintenanceDate,
                MaintenanceIntervalDays = t.MaintenanceIntervalDays,
                DaysUntilMaintenance = t.NextMaintenanceDate.HasValue 
                    ? (int)(t.NextMaintenanceDate.Value - now).TotalDays 
                    : 0,
                IsOverdue = t.NextMaintenanceDate.HasValue && t.NextMaintenanceDate.Value < now,
                Status = t.Status
            })
            .OrderBy(t => t.NextMaintenanceDate)
            .ToList();

        var performance = trucks
            .Select(t => new TruckPerformanceDto
            {
                TruckId = t.Id,
                LicensePlate = t.LicensePlate,
                TripCount = t.Trips.Count,
                TotalDistance = t.Trips.Sum(tr => tr.Distance),
                TotalFuelCost = t.Trips.Sum(tr => tr.FuelCost ?? 0),
                TotalRevenue = t.Trips.Sum(tr => tr.Invoices.Sum(i => i.TotalAmount))
            })
            .ToList();

        foreach (var p in performance)
        {
            p.AverageFuelCostPerKm = p.TotalDistance > 0 ? p.TotalFuelCost / p.TotalDistance : 0;
            p.ProfitMargin = p.TotalRevenue > 0 ? ((p.TotalRevenue - p.TotalFuelCost) / p.TotalRevenue) * 100 : 0;
            p.AverageTripDistance = p.TripCount > 0 ? p.TotalDistance / p.TripCount : 0;
        }

        return new TruckReportDto
        {
            Summary = summary,
            TruckUtilization = utilization,
            MaintenanceSchedule = maintenance,
            Performance = performance.OrderByDescending(p => p.TotalRevenue).ToList()
        };
    }

    public async Task<DriverReportDto> GetDriverReportAsync()
    {
        var drivers = await _context.Drivers
            .Include(d => d.Trips)
                .ThenInclude(t => t.Invoices)
                    .ThenInclude(i => i.Payments)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var summary = new DriverSummaryDto
        {
            TotalDrivers = drivers.Count,
            AvailableDrivers = drivers.Count(d => d.Status == "Available"),
            OnTripDrivers = drivers.Count(d => d.Status == "OnTrip"),
            OffDutyDrivers = drivers.Count(d => d.Status == "OffDuty"),
            InactiveDrivers = drivers.Count(d => d.Status == "Inactive"),
            LicenseExpiringSoon = drivers.Count(d => 
                d.LicenseExpiryDate > now && 
                d.LicenseExpiryDate <= now.AddDays(30)),
            LicenseExpired = drivers.Count(d => d.LicenseExpiryDate < now)
        };

        var performance = drivers
            .Select(d => new DriverPerformanceDto
            {
                DriverId = d.Id,
                DriverName = d.FullName,
                LicenseNumber = d.LicenseNumber,
                Status = d.Status,
                TotalTrips = d.Trips.Count,
                CompletedTrips = d.Trips.Count(t => t.Status == "Completed"),
                InProgressTrips = d.Trips.Count(t => t.Status == "InProgress"),
                TotalDistance = d.Trips.Sum(t => t.Distance),
                TotalRevenue = d.Trips.Sum(t => t.Invoices.Sum(i => i.TotalAmount)),
                LicenseExpiryDate = d.LicenseExpiryDate
            })
            .ToList();

        foreach (var p in performance)
        {
            p.CompletionRate = p.TotalTrips > 0 ? (p.CompletedTrips * 100.0m / p.TotalTrips) : 0;
            p.AverageDistance = p.TotalTrips > 0 ? p.TotalDistance / p.TotalTrips : 0;
            if (p.LicenseExpiryDate.HasValue)
            {
                p.DaysUntilLicenseExpiry = (int)(p.LicenseExpiryDate.Value - now).TotalDays;
                p.IsLicenseExpiringSoon = p.DaysUntilLicenseExpiry > 0 && p.DaysUntilLicenseExpiry <= 30;
                p.IsLicenseExpired = p.DaysUntilLicenseExpiry < 0;
            }
        }

        var driverTrips = await _context.Trips
            .Include(t => t.Driver)
            .Include(t => t.Customer)
            .Include(t => t.Invoices)
            .Where(t => t.Driver != null)
            .OrderByDescending(t => t.ScheduledDate)
            .Take(100)
            .Select(t => new DriverTripDto
            {
                DriverId = t.Driver!.Id,
                DriverName = t.Driver.FullName,
                TripId = t.Id,
                TripNumber = t.TripNumber,
                CustomerName = t.Customer.Name,
                Origin = t.Origin,
                Destination = t.Destination,
                Status = t.Status,
                ScheduledDate = t.ScheduledDate,
                ActualStartDate = t.ActualStartDate,
                ActualEndDate = t.ActualEndDate,
                Distance = t.Distance,
                Revenue = t.Invoices.Sum(i => i.TotalAmount)
            })
            .ToListAsync();

        return new DriverReportDto
        {
            Summary = summary,
            DriverPerformance = performance.OrderByDescending(p => p.TotalTrips).ToList(),
            DriverTrips = driverTrips
        };
    }

    public async Task<CustomerReportDto> GetCustomerReportAsync()
    {
        var customers = await _context.Customers
            .Include(c => c.Trips)
            .Include(c => c.Invoices)
                .ThenInclude(i => i.Payments)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var activeDate = now.AddDays(-30);
        
        var summary = new CustomerSummaryDto
        {
            TotalCustomers = customers.Count,
            ActiveCustomers = customers.Count(c => c.Trips.Any(t => t.ScheduledDate >= activeDate)),
            InactiveCustomers = customers.Count(c => !c.Trips.Any(t => t.ScheduledDate >= activeDate)),
            TotalRevenue = customers.Sum(c => c.Invoices.Sum(i => i.TotalAmount)),
            TotalDebt = customers.Sum(c => c.Invoices.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount))),
            TotalTrips = customers.Sum(c => c.Trips.Count)
        };
        summary.AverageRevenuePerCustomer = summary.TotalCustomers > 0 ? summary.TotalRevenue / summary.TotalCustomers : 0;

        var customerDetails = customers
            .Select(c => new CustomerDetailDto
            {
                CustomerId = c.Id,
                CustomerName = c.Name,
                Phone = c.Phone,
                Email = c.Email ?? string.Empty,
                Address = c.Address ?? string.Empty,
                TripCount = c.Trips.Count,
                InvoiceCount = c.Invoices.Count,
                TotalRevenue = c.Invoices.Sum(i => i.TotalAmount),
                TotalPaid = c.Invoices.Sum(i => i.Payments.Sum(p => p.Amount)),
                RemainingDebt = c.Invoices.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount)),
                LastTripDate = c.Trips.OrderByDescending(t => t.ScheduledDate).FirstOrDefault()?.ScheduledDate,
                LastInvoiceDate = c.Invoices.OrderByDescending(i => i.IssueDate).FirstOrDefault()?.IssueDate,
                IsActive = c.Trips.Any(t => t.ScheduledDate >= activeDate)
            })
            .OrderByDescending(c => c.TotalRevenue)
            .ToList();

        var customerRevenue = customers
            .SelectMany(c => c.Invoices
                .GroupBy(i => new { i.IssueDate.Year, i.IssueDate.Month })
                .Select(g => new CustomerRevenueDto
                {
                    CustomerId = c.Id,
                    CustomerName = c.Name,
                    Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                    PeriodDate = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Revenue = g.Sum(i => i.TotalAmount),
                    Paid = g.Sum(i => i.Payments.Sum(p => p.Amount)),
                    Outstanding = g.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount)),
                    TripCount = c.Trips.Count(t => t.ScheduledDate.Year == g.Key.Year && t.ScheduledDate.Month == g.Key.Month),
                    InvoiceCount = g.Count()
                }))
            .OrderByDescending(c => c.PeriodDate)
            .ThenByDescending(c => c.Revenue)
            .ToList();

        var customerDebt = customers
            .Select(c =>
            {
                var invoices = c.Invoices.ToList();
                var totalDebt = invoices.Sum(i => i.TotalAmount);
                var totalPaid = invoices.Sum(i => i.Payments.Sum(p => p.Amount));
                var remainingDebt = totalDebt - totalPaid;
                var overdueInvoices = invoices.Where(i => i.DueDate < now && i.Status != "Paid").ToList();
                var oldestInvoice = overdueInvoices.OrderBy(i => i.DueDate).FirstOrDefault();
                
                return new CustomerDebtDto
                {
                    CustomerId = c.Id,
                    CustomerName = c.Name,
                    TotalDebt = totalDebt,
                    OverdueDebt = overdueInvoices.Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount)),
                    TotalInvoices = invoices.Count,
                    OverdueInvoices = overdueInvoices.Count,
                    PendingInvoices = invoices.Count(i => i.Status == "Pending" || i.Status == "Partial"),
                    OldestInvoiceDate = oldestInvoice?.DueDate,
                    DaysOverdue = oldestInvoice != null ? (int)(now - oldestInvoice.DueDate).TotalDays : 0
                };
            })
            .Where(c => c.TotalDebt > 0)
            .OrderByDescending(c => c.OverdueDebt)
            .ToList();

        return new CustomerReportDto
        {
            Summary = summary,
            CustomerDetails = customerDetails,
            CustomerRevenue = customerRevenue,
            CustomerDebt = customerDebt
        };
    }

    private int GetWeekOfYear(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        return calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
    }
}
