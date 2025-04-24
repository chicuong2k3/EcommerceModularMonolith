namespace Catalog.Core.ReadModels;

public sealed class CategoryReadModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public IReadOnlyCollection<CategoryReadModel> SubCategories { get; set; } = new List<CategoryReadModel>();
    public Guid? ParentCategoryId { get; init; }
}
