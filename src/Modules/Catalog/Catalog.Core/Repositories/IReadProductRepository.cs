using Catalog.Core.ReadModels;
using Shared.Abstractions.Core;

namespace Catalog.Core.Repositories;

public interface IReadProductRepository
{
    Task<PaginationResult<ProductReadModel>> SearchAsync(SearchProductsSpecification specification, CancellationToken cancellationToken = default);
    Task<ProductReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
