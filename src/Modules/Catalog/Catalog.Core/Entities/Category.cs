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

    public void AddSubCategory(Category subCategory)
    {
        subCategories.Add(subCategory);
    }
}
