using Catalog.Core.Entities;
using Catalog.Core.ReadModels;
using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public sealed record CreateCategory(string Name, Guid? ParentCategoryId)
    : ICommand<CategoryReadModel>;

internal sealed class CreateCategoryHandler(
    IWriteCategoryRepository categoryRepository)
    : ICommandHandler<CreateCategory, CategoryReadModel>
{
    public async Task<Result<CategoryReadModel>> Handle(CreateCategory command, CancellationToken cancellationToken)
    {
        var result = Category.Create(command.Name);
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

        return Result.Ok(new CategoryReadModel()
        {
            Id = category.Id,
            Name = category.Name,
            SubCategories = category.SubCategories.Select(c => new CategoryReadModel()
            {
                Id = c.Id,
                Name = c.Name
            }).ToList(),
            ParentCategoryId = category.ParentCategoryId
        });

    }

}