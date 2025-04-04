using Promotion.Application.Conditions.ReadModels;

namespace Promotion.Application.Conditions.Queries;

public record GetConditionById(Guid Id) : IQuery<ConditionReadModel>;

internal sealed class GetConditionByIdHandler(IConditionRepository conditionRepository)
    : IQueryHandler<GetConditionById, ConditionReadModel>
{
    public async Task<Result<ConditionReadModel>> Handle(GetConditionById query, CancellationToken cancellationToken)
    {
        var condition = await conditionRepository.GetByIdAsync(query.Id, cancellationToken);

        if (condition == null)
        {
            return Result.Fail(new NotFoundError($"The condition with id '{query.Id}' not found"));
        }

        return Result.Ok(new ConditionReadModel()
        {
            Id = condition.Id,
            Name = condition.Name,
            ConditionType = condition.Type.ToString(),
            Value = condition.Value
        });
    }
}
