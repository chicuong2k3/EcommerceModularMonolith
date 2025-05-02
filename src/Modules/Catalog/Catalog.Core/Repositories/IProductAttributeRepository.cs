using Catalog.Core.Entities;

namespace Catalog.Core.Repositories;

public interface IProductAttributeRepository
{
    Task<ProductAttribute?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<ProductAttribute?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProductAttribute>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    Task AddAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
    Task RemoveAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
    Task<List<ProductAttribute>> GetAttributesAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
