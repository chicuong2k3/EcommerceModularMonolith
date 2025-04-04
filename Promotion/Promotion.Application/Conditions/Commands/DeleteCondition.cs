namespace Promotion.Application.Conditions.Commands;

public record DeleteCondition(Guid Id) : ICommand;

internal sealed class DeleteConditionHandler(IConditionRepository conditionRepository)
    : ICommandHandler<DeleteCondition>
{
    public async Task<Result> Handle(DeleteCondition command, CancellationToken cancellationToken)
    {
        var condition = await conditionRepository.GetByIdAsync(command.Id, cancellationToken);

        if (condition == null)
            return Result.Fail(new NotFoundError($"The condition with id '{command.Id}' not found"));

        await conditionRepository.RemoveAsync(condition, cancellationToken);
        return Result.Ok();
    }
}
