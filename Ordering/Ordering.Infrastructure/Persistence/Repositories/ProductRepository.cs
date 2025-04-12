using Ordering.Domain.ProductAggregate;

namespace Ordering.Infrastructure.Persistence.Repositories;

internal class ProductRepository : IProductRepository
{
    private readonly OrderingDbContext dbContext;

    public ProductRepository(OrderingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetProductAsync(Guid productId, Guid variantId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .Where(p => p.Id == productId && p.VariantId == variantId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
