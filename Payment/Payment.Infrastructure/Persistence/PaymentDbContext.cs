using Microsoft.EntityFrameworkCore;

namespace Payment.Infrastructure.Persistence;

internal class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options)
    {

    }

    //public DbSet<Payment> Payments { get; set; }
    //public DbSet<Transaction> Transactions { get; set; }
}
