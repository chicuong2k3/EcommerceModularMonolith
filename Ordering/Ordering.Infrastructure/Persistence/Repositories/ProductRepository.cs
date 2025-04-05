using Ordering.Domain.ProductAggregate;

namespace Ordering.Infrastructure.Persistence.Repositories;

internal class ProductRepository : IProductRepository
{
    private readonly OrderingDbContext dbContext;

    public ProductRepository(OrderingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .Where(p => p.Id == productId)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
