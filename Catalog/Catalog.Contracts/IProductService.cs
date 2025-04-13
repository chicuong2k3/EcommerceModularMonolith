namespace Catalog.Contracts;

public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken);
}
