using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public sealed record CreateCategory(Guid Id, string Name, Guid? ParentCategoryId)
    : ICommand;

internal sealed class CreateCategoryHandler(
    ICategoryRepository categoryRepository)
    : ICommandHandler<CreateCategory>
{
    public async Task<Result> Handle(CreateCategory command, CancellationToken cancellationToken)
    {
        var result = Category.Create(command.Id, command.Name);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        var category = result.Value;

        if (command.ParentCategoryId != null)
        {
            var parentCategory = await categoryRepository.GetByIdAsync(command.ParentCategoryId.Value, cancellationToken);

            if (parentCategory == null)
                return Result.Fail(new NotFoundError($"Category with id '{command.ParentCategoryId.Value}' not found"));

            parentCategory.AddSubCategory(category);
        }

        await categoryRepository.AddAsync(category, cancellationToken);

        return Result.Ok();

    }

}