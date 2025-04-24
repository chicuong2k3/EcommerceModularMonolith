using Ordering.Core.ReadModels;
using Shared.Abstractions.Core;

namespace Ordering.Core.Repositories;

public interface IReadOrderRepository
{
    Task<OrderReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginationResult<OrderReadModel>> GetAllAsync(GetOrdersSpecification specification, CancellationToken cancellationToken = default);
}
