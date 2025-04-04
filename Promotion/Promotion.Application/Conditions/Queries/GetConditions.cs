using Promotion.Application.Conditions.ReadModels;

namespace Promotion.Application.Conditions.Queries;

public record GetConditions() : IQuery<IEnumerable<ConditionReadModel>>;

internal sealed class GetConditionsHandler(IConditionRepository conditionRepository)
    : IQueryHandler<GetConditions, IEnumerable<ConditionReadModel>>
{
    public async Task<Result<IEnumerable<ConditionReadModel>>> Handle(GetConditions request, CancellationToken cancellationToken)
    {
        var conditions = await conditionRepository.GetConditionsAsync(null, cancellationToken);

        return Result.Ok(conditions.Select(condition => new ConditionReadModel()
        {
            Id = condition.Id,
            Name = condition.Name,
            ConditionType = condition.Type.ToString(),
            Value = condition.Value
        }));
    }
}