using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Shared.Infrastructure.Inbox;
using Shared.Infrastructure.Outbox;

namespace Ordering.Core.Persistence;

public class OrderingDbContext : DbContext
{
    public OrderingDbContext(
        DbContextOptions<OrderingDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("ordering");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
    }

    public DbSet<Order> Orders { get; set; }
    private DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    private DbSet<CartItem> CartItems { get; set; }

    private DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }
    private DbSet<OutboxMessage> OutboxMessages { get; set; }
    private DbSet<InboxMessage> InboxMessages { get; set; }
    private DbSet<InboxMessageConsumer> InboxMessageConsumers { get; set; }

}
