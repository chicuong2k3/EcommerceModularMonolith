namespace Catalog.Core.Queries;

public sealed class CategoryListItemReadModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public Guid? ParentCategoryId { get; init; }
}