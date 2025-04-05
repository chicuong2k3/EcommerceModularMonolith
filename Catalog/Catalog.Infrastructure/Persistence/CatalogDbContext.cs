using Common.Infrastructure.Inbox;
using Common.Infrastructure.Outbox;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext : DbContext
{
    private readonly ILogger<CatalogDbContext> logger;

    public CatalogDbContext(
        DbContextOptions<CatalogDbContext> options,
        ILogger<CatalogDbContext> logger)
        : base(options)
    {
        this.logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("catalog");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }

    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<ProductAttribute> ProductAttributes { get; set; } = default!;
    private DbSet<Review> Reviews { get; set; } = default!;

    private DbSet<OutboxMessage> OutboxMessages { get; set; }
    private DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }
    private DbSet<InboxMessage> InboxMessages { get; set; }
    private DbSet<InboxMessageConsumer> InboxMessageConsumers { get; set; }
}
