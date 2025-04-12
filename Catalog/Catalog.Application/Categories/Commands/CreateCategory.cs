using Catalog.Application.Categories.ReadModels;

namespace Catalog.Application.Categories.Commands;

public sealed record CreateCategory(string Name, Guid? ParentCategoryId)
    : ICommand<CategoryReadModel>;

internal sealed class CreateCategoryHandler(
    ICategoryRepository categoryRepository)
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