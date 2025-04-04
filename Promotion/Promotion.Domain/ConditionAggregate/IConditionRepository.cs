namespace Promotion.Domain.ConditionAggregate;

public interface IConditionRepository
{
    Task<IEnumerable<Condition>> GetConditionsAsync(IEnumerable<Guid>? conditionIds, CancellationToken cancellationToken = default);
    Task<Condition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Condition?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Condition condition, CancellationToken cancellationToken = default);
    Task RemoveAsync(Condition condition, CancellationToken cancellationToken = default);
}
