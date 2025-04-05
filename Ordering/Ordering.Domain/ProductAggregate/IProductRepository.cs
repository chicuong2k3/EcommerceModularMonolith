namespace Ordering.Domain.ProductAggregate;

public interface IProductRepository
{
    Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
