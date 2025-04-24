using Microsoft.EntityFrameworkCore;
using Pay.Core.Entities;

namespace Pay.Infrastructure.Persistence;

public class PayDbContext : DbContext
{
    public PayDbContext(DbContextOptions<PayDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("billing");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PayDbContext).Assembly);
    }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}
