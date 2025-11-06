using Microsoft.EntityFrameworkCore;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Truck> Trucks { get; }
    DbSet<Driver> Drivers { get; }
    DbSet<Trip> Trips { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<Payment> Payments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

