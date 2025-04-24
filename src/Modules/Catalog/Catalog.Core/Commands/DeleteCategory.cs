using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public sealed record DeleteCategory(Guid Id) : ICommand;

internal sealed class DeleteCategoryHandler(
    IWriteCategoryRepository categoryRepository)
    : ICommandHandler<DeleteCategory>
{
    public async Task<Result> Handle(DeleteCategory command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (category == null)
            return Result.Fail(new NotFoundError($"The category with id '{command.Id}' not found"));

        await categoryRepository.RemoveAsync(category, cancellationToken);
        return Result.Ok();
    }

}