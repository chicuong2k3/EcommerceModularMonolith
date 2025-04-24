namespace Catalog.ApiContracts.Responses;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyCollection<CategoryResponse> SubCategories { get; set; } = new List<CategoryResponse>();
}
