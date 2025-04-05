using Common.Infrastructure.Inbox;
using Common.Infrastructure.Outbox;
using Ordering.Domain.ProductAggregate;

namespace Ordering.Infrastructure.Persistence;

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
    public DbSet<Product> Products { get; set; }
    private DbSet<ProductVariant> ProductVariants { get; set; }
    private DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }


    private DbSet<OutboxMessage> OutboxMessages { get; set; }
    private DbSet<InboxMessage> InboxMessages { get; set; }
    private DbSet<InboxMessageConsumer> InboxMessageConsumers { get; set; }

}
