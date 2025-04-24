using Catalog.Core.Queries;
using Catalog.Core.ReadModels;

namespace Catalog.Core.Repositories;

public interface IReadCategoryRepository
{
    Task<CategoryReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryListItemReadModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryListItemReadModel>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default);
}
