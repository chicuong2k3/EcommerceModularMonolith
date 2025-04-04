using Promotion.Application.Conditions.ReadModels;

namespace Promotion.Application.Conditions.Commands;

public record CreateCondition(string Name, string ConditionType, string Value)
    : ICommand<ConditionReadModel>;

internal sealed class CreateConditionHandler(IConditionRepository conditionRepository)
    : ICommandHandler<CreateCondition, ConditionReadModel>
{

    public async Task<Result<ConditionReadModel>> Handle(CreateCondition command, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ConditionType>(command.ConditionType, true, out var conditionType))
        {
            var validConditionTypes = string.Join(", ", Enum.GetNames<ConditionType>());

            return Result.Fail(new ValidationError($"Invalid condition type: {command.ConditionType}. Valid condition types: {validConditionTypes}"));
        }

        var existingCondition = await conditionRepository.GetByNameAsync(command.Name, cancellationToken);

        if (existingCondition != null)
        {
            return Result.Fail(new ValidationError($"Condition with name '{command.Name}' already exists."));
        }

        var conditionCreationResult = Condition.Create(command.Name, conditionType, command.Value);

        if (conditionCreationResult.IsFailed)
        {
            return Result.Fail(conditionCreationResult.Errors);
        }

        await conditionRepository.AddAsync(conditionCreationResult.Value, cancellationToken);

        return Result.Ok(new ConditionReadModel()
        {
            Id = conditionCreationResult.Value.Id,
            Name = conditionCreationResult.Value.Name,
            ConditionType = conditionCreationResult.Value.Type.ToString(),
            Value = conditionCreationResult.Value.Value
        });
    }
}