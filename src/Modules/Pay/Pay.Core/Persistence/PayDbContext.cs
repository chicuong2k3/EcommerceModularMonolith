using Microsoft.EntityFrameworkCore;
using Pay.Core.Entities;
using Shared.Infrastructure.Inbox;
using Shared.Infrastructure.Outbox;

namespace Pay.Core.Persistence;

public class PayDbContext : DbContext
{
    public PayDbContext(DbContextOptions<PayDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("pay");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PayDbContext).Assembly);
    }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }
    public DbSet<InboxMessageConsumer> InboxMessageConsumers { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }
}
