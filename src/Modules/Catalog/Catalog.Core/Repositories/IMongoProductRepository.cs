using Catalog.Core.Entities;

namespace Catalog.Core.Repositories;

public interface IMongoProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);
    Task UpdateAsync(CancellationToken cancellationToken);
    Task GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
