namespace Promotion.Infrastructure.Persistence.Repositories;

internal class ConditionRepository : IConditionRepository
{
    private readonly PromotionDbContext dbContext;

    public ConditionRepository(PromotionDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Condition condition, CancellationToken cancellationToken = default)
    {
        dbContext.Conditions.Add(condition);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Condition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Conditions.FindAsync(id, cancellationToken);
    }

    public async Task<Condition?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await dbContext.Conditions.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Condition>> GetConditionsAsync(IEnumerable<Guid>? conditionIds, CancellationToken cancellationToken = default)
    {
        var conditions = dbContext.Conditions.AsQueryable();

        if (conditionIds != null && conditionIds.Any())
        {
            conditions = conditions.Where(c => conditionIds.Contains(c.Id));
        }

        return await conditions.ToListAsync(cancellationToken);
    }

    public async Task RemoveAsync(Condition condition, CancellationToken cancellationToken = default)
    {
        dbContext.Conditions.Remove(condition);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
