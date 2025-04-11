namespace Ordering.Domain.ProductAggregate;

public interface IProductRepository
{
    Task<Product?> GetProductAsync(Guid productId, Guid variantId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
