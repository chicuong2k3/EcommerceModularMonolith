namespace Catalog.Domain.CategoryAggregate;

public sealed class Category : AggregateRoot
{
    private Category()
    {

    }

    public string Name { get; private set; }
    private List<Category> subCategories = [];

    public IReadOnlyCollection<Category> SubCategories => subCategories.AsReadOnly();

    private Category(string name)
    {
        Id = Guid.NewGuid();
        Name = name.ToLower();
    }

    public static Result<Category> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail(new ValidationError("Category name is required."));

        return Result.Ok(new Category(name));
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
