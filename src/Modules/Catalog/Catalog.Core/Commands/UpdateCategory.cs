using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public sealed record UpdateCategory(
    Guid Id,
    string Name,
    Guid? ParentCategoryId
) : ICommand;

internal sealed class UpdateCategoryHandler(
    ICategoryRepository categoryRepository)
    : ICommandHandler<UpdateCategory>
{
    public async Task<Result> Handle(UpdateCategory command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (category == null)
            return Result.Fail(new NotFoundError($"The category with id '{command.Id}' not found"));

        var existingCategory = await categoryRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingCategory != null && existingCategory.Id != command.Id)
            return Result.Fail(new ConflictError($"Category with name '{command.Name}' already exists."));

        if (command.ParentCategoryId != null)
        {
            var parentCategory = await categoryRepository.GetByIdAsync(command.ParentCategoryId.Value, cancellationToken);
            if (parentCategory == null)
                return Result.Fail(new NotFoundError($"The category with id '{command.ParentCategoryId.Value}' not found"));

            var addSubCatResult = await parentCategory.AddSubCategoryAsync(category, categoryRepository);
            if (addSubCatResult.IsFailed)
                return Result.Fail(addSubCatResult.Errors);
        }

        var result = category.ChangeName(command.Name);
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await categoryRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }

}