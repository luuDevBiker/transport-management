using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(Transport.Infrastructure.Data.TransportDbContext context, IPasswordHasher passwordHasher)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Users
        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new User
                {
                    Email = "admin@example.com",
                    PasswordHash = passwordHasher.HashPassword("Admin@123"),
                    FullName = "Administrator",
                    Role = "Admin",
                    IsActive = true
                },
                new User
                {
                    Email = "dispatcher@example.com",
                    PasswordHash = passwordHasher.HashPassword("Dispatcher@123"),
                    FullName = "Dispatcher User",
                    Role = "Dispatcher",
                    IsActive = true
                },
                new User
                {
                    Email = "accountant@example.com",
                    PasswordHash = passwordHasher.HashPassword("Accountant@123"),
                    FullName = "Accountant User",
                    Role = "Accountant",
                    IsActive = true
                }
            };
            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        // Seed Customers (100 records)
        if (!await context.Customers.AnyAsync())
        {
            var customers = new List<Customer>();
            var random = new Random();
            var companyNames = new[] { "Công ty", "Công ty TNHH", "Công ty Cổ phần", "Doanh nghiệp", "Xí nghiệp" };
            var businessTypes = new[] { "Xây dựng", "Vật liệu", "Thiết bị", "Cơ khí", "Điện nước" };
            
            for (int i = 1; i <= 100; i++)
            {
                var companyType = companyNames[random.Next(companyNames.Length)];
                var businessType = businessTypes[random.Next(businessTypes.Length)];
                customers.Add(new Customer
                {
                    Name = $"{companyType} {businessType} {i:D3}",
                    CompanyName = $"{companyType} {businessType} {i:D3}",
                    Phone = $"09{random.Next(10000000, 99999999)}",
                    Email = $"customer{i}@example.com",
                    Address = $"{random.Next(1, 999)} Đường {businessType}, Quận {random.Next(1, 12)}, TP.HCM",
                    TaxCode = $"{random.Next(1000000000, 2147483647)}",
                    Notes = i % 10 == 0 ? $"Khách hàng VIP {i}" : null
                });
            }
            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();
        }

        // Seed Trucks (100 records)
        if (!await context.Trucks.AnyAsync())
        {
            var trucks = new List<Truck>();
            var random = new Random();
            var brands = new[] { "Hyundai", "Isuzu", "Hino", "Fuso", "Volvo", "Mercedes", "MAN", "Scania" };
            var models = new[] { "HD370", "NPR 75", "300 Series", "Canter", "FH", "Actros", "TGX", "R Series" };
            var statuses = new[] { "Available", "InUse", "Maintenance", "Inactive" };
            var provinces = new[] { "51A", "51B", "51C", "51D", "51E", "51F", "51G", "51H", "51K", "51L" };
            
            for (int i = 1; i <= 100; i++)
            {
                var brand = brands[random.Next(brands.Length)];
                var model = models[random.Next(models.Length)];
                var province = provinces[random.Next(provinces.Length)];
                var lastMaintenance = DateTime.UtcNow.AddDays(-random.Next(0, 90));
                var maintenanceInterval = random.Next(60, 120);
                
                trucks.Add(new Truck
                {
                    LicensePlate = $"{province}-{random.Next(10000, 99999)}",
                    Brand = brand,
                    Model = model,
                    Year = random.Next(2015, 2024),
                    Capacity = random.Next(5, 20) + (decimal)random.NextDouble(),
                    Status = statuses[random.Next(statuses.Length)],
                    LastMaintenanceDate = lastMaintenance,
                    NextMaintenanceDate = lastMaintenance.AddDays(maintenanceInterval),
                    MaintenanceIntervalDays = maintenanceInterval,
                    Notes = i % 20 == 0 ? $"Xe cần kiểm tra định kỳ {i}" : null
                });
            }
            context.Trucks.AddRange(trucks);
            await context.SaveChangesAsync();
        }

        // Seed Drivers (100 records)
        if (!await context.Drivers.AnyAsync())
        {
            var drivers = new List<Driver>();
            var random = new Random();
            var firstNames = new[] { "Nguyễn Văn", "Trần Thị", "Lê Văn", "Phạm Thị", "Hoàng Văn", "Vũ Thị", "Đặng Văn", "Bùi Thị" };
            var lastNames = new[] { "An", "Bình", "Cường", "Dũng", "Em", "Giang", "Hùng", "Khang", "Long", "Minh", "Nam", "Phong", "Quang", "Sơn", "Tuấn" };
            var statuses = new[] { "Available", "OnTrip", "OffDuty", "Inactive" };
            
            for (int i = 1; i <= 100; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];
                var licenseExpiry = DateTime.UtcNow.AddDays(random.Next(-365, 1095));
                
                drivers.Add(new Driver
                {
                    FullName = $"{firstName} {lastName} {i:D3}",
                    Phone = $"09{random.Next(10000000, 99999999)}",
                    Email = $"driver{i}@example.com",
                    LicenseNumber = $"DL{random.Next(100000, 999999)}",
                    LicenseExpiryDate = licenseExpiry,
                    Address = $"{random.Next(1, 999)} Đường ABC, Quận {random.Next(1, 12)}, TP.HCM",
                    Status = statuses[random.Next(statuses.Length)],
                    Notes = licenseExpiry < DateTime.UtcNow.AddDays(30) ? $"Bằng lái sắp hết hạn {i}" : null
                });
            }
            context.Drivers.AddRange(drivers);
            await context.SaveChangesAsync();
        }

        // Seed Trips (100 records)
        if (!await context.Trips.AnyAsync())
        {
            var customers = await context.Customers.ToListAsync();
            var trucks = await context.Trucks.Where(t => t.Status == "Available" || t.Status == "InUse").ToListAsync();
            var drivers = await context.Drivers.Where(d => d.Status == "Available" || d.Status == "OnTrip").ToListAsync();
            var dispatcher = await context.Users.FirstOrDefaultAsync(u => u.Role == "Dispatcher");
            
            if (customers.Any() && trucks.Any() && drivers.Any())
            {
                var trips = new List<Trip>();
                var random = new Random();
                var origins = new[] { "TP.HCM", "Hà Nội", "Đà Nẵng", "Cần Thơ", "Nha Trang", "Vũng Tàu", "Biên Hòa", "Huế" };
                var destinations = new[] { "TP.HCM", "Hà Nội", "Đà Nẵng", "Cần Thơ", "Nha Trang", "Vũng Tàu", "Biên Hòa", "Huế", "Quy Nhon", "Phan Thiết" };
                var statuses = new[] { "Scheduled", "InProgress", "Completed", "Cancelled" };
                
                for (int i = 1; i <= 100; i++)
                {
                    var origin = origins[random.Next(origins.Length)];
                    var destination = destinations[random.Next(destinations.Length)];
                    while (destination == origin)
                    {
                        destination = destinations[random.Next(destinations.Length)];
                    }
                    
                    var scheduledDate = DateTime.UtcNow.AddDays(random.Next(-30, 60));
                    var status = statuses[random.Next(statuses.Length)];
                    var distance = random.Next(50, 2000);
                    
                    var trip = new Trip
                    {
                        TripNumber = $"TRP-{scheduledDate:yyyyMMdd}-{i:D4}",
                        CustomerId = customers[random.Next(customers.Count)].Id,
                        TruckId = trucks[random.Next(trucks.Count)].Id,
                        DriverId = drivers[random.Next(drivers.Count)].Id,
                        DispatcherId = dispatcher?.Id,
                        Origin = origin,
                        Destination = destination,
                        ScheduledDate = scheduledDate,
                        Status = status,
                        Distance = distance,
                        FuelCost = status == "Completed" ? (decimal)(distance * random.Next(2000, 5000)) : null,
                        OtherCosts = status == "Completed" && random.Next(0, 100) < 30 ? (decimal)random.Next(500000, 5000000) : null,
                        Notes = i % 10 == 0 ? $"Chuyến hàng đặc biệt {i}" : null
                    };
                    
                    if (status == "InProgress" || status == "Completed")
                    {
                        trip.ActualStartDate = scheduledDate.AddHours(random.Next(0, 12));
                    }
                    
                    if (status == "Completed")
                    {
                        trip.ActualEndDate = trip.ActualStartDate?.AddHours(random.Next(8, 48));
                    }
                    
                    trips.Add(trip);
                }
                context.Trips.AddRange(trips);
                await context.SaveChangesAsync();
            }
        }

        // Seed Invoices (100 records)
        if (!await context.Invoices.AnyAsync())
        {
            var customers = await context.Customers.ToListAsync();
            var trips = await context.Trips.Where(t => t.Status == "Completed").ToListAsync();
            
            if (customers.Any())
            {
                var invoices = new List<Invoice>();
                var random = new Random();
                var statuses = new[] { "Pending", "Partial", "Paid", "Overdue" };
                
                for (int i = 1; i <= 100; i++)
                {
                    var issueDate = DateTime.UtcNow.AddDays(random.Next(-90, 30));
                    var dueDate = issueDate.AddDays(random.Next(15, 60));
                    var amount = (decimal)random.Next(1000000, 50000000);
                    var taxAmount = amount * 0.1m;
                    var totalAmount = amount + taxAmount;
                    var status = statuses[random.Next(statuses.Length)];
                    var customer = customers[random.Next(customers.Count)];
                    var trip = trips.Any() && random.Next(0, 100) < 70 ? trips[random.Next(trips.Count)] : null;
                    
                    invoices.Add(new Invoice
                    {
                        InvoiceNumber = $"INV-{issueDate:yyyyMMdd}-{i:D4}",
                        CustomerId = customer.Id,
                        TripId = trip?.Id,
                        IssueDate = issueDate,
                        DueDate = dueDate,
                        Amount = amount,
                        TaxAmount = taxAmount,
                        TotalAmount = totalAmount,
                        Status = status,
                        Description = trip != null ? $"Hóa đơn cho chuyến hàng {trip.TripNumber}" : $"Hóa đơn dịch vụ {i}",
                        Notes = i % 10 == 0 ? $"Hóa đơn cần xử lý {i}" : null
                    });
                }
                context.Invoices.AddRange(invoices);
                await context.SaveChangesAsync();
            }
        }

        // Seed Payments (100 records)
        if (!await context.Payments.AnyAsync())
        {
            var invoices = await context.Invoices.ToListAsync();
            
            if (invoices.Any())
            {
                var payments = new List<Payment>();
                var random = new Random();
                var paymentMethods = new[] { "Cash", "BankTransfer", "Check" };
                
                for (int i = 1; i <= 100; i++)
                {
                    var invoice = invoices[random.Next(invoices.Count)];
                    var paymentDate = invoice.IssueDate.AddDays(random.Next(0, 90));
                    var maxAmount = invoice.TotalAmount - (invoice.Payments?.Sum(p => p.Amount) ?? 0);
                    var amount = maxAmount > 0 ? (decimal)random.Next((int)(maxAmount * 0.1m), (int)maxAmount) : invoice.TotalAmount;
                    
                    payments.Add(new Payment
                    {
                        PaymentNumber = $"PAY-{paymentDate:yyyyMMdd}-{i:D4}",
                        InvoiceId = invoice.Id,
                        PaymentDate = paymentDate,
                        Amount = amount,
                        PaymentMethod = paymentMethods[random.Next(paymentMethods.Length)],
                        ReferenceNumber = random.Next(0, 100) < 50 ? $"REF{random.Next(100000, 999999)}" : null,
                        Notes = i % 10 == 0 ? $"Thanh toán đặc biệt {i}" : null
                    });
                }
                context.Payments.AddRange(payments);
                await context.SaveChangesAsync();
                
                // Update invoice statuses based on payments
                foreach (var invoice in invoices)
                {
                    var totalPaid = invoice.Payments.Sum(p => p.Amount);
                    if (totalPaid >= invoice.TotalAmount)
                        invoice.Status = "Paid";
                    else if (totalPaid > 0)
                        invoice.Status = "Partial";
                    else if (invoice.DueDate < DateTime.UtcNow)
                        invoice.Status = "Overdue";
                    else
                        invoice.Status = "Pending";
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
