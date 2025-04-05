using Common.Infrastructure.Inbox;
using Common.Infrastructure.Outbox;

namespace Promotion.Infrastructure.Persistence;

public class PromotionDbContext : DbContext
{
    public PromotionDbContext(DbContextOptions<PromotionDbContext> options)
        : base(options)
    {
    }

    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Condition> Conditions { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    private DbSet<OutboxMessage> OutboxMessages { get; set; }
    private DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }
    private DbSet<InboxMessage> InboxMessages { get; set; }
    private DbSet<InboxMessageConsumer> InboxMessageConsumers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("promotion");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PromotionDbContext).Assembly);
    }
}
