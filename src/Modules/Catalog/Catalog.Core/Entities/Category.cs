using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Core;

namespace Catalog.Core.Entities;

public sealed class Category : AggregateRoot
{
    private Category()
    {

    }

    public string Name { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    private List<Category> subCategories = [];

    public IReadOnlyCollection<Category> SubCategories => subCategories.AsReadOnly();

    private Category(Guid id, string name)
    {
        Id = id;
        Name = name.ToLower();
    }

    public static Result<Category> Create(Guid id, string name)
    {
        if (id == Guid.Empty)
            return Result.Fail(new ValidationError("Id is required."));
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail(new ValidationError("Category name is required."));

        if (name.Length > 100)
            return Result.Fail(new ValidationError("Category name cannot exceed 100 characters."));

        return Result.Ok(new Category(id, name));
    }

    public Result ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail(new ValidationError("Category name is required."));

        if (Name != name)
        {
            Name = name.ToLower();
        }

        return Result.Ok();
    }

    public async Task<Result> AddSubCategoryAsync(Category subCategory, ICategoryRepository categoryRepository)
    {
        var isCircular = await IsCircularAsync(subCategory.Id, categoryRepository);
        if (isCircular)
            return Result.Fail(new ValidationError("Circular reference detected."));

        subCategories.Add(subCategory);
        return Result.Ok();
    }

    private async Task<bool> IsCircularAsync(Guid childId, ICategoryRepository categoryRepository)
    {
        Category? current = this;
        while (current != null && current.ParentCategoryId != null)
        {
            if (current.ParentCategoryId == childId)
                return true;

            current = await categoryRepository.GetByIdAsync(current.ParentCategoryId.Value);
        }
        return false;
    }

}
