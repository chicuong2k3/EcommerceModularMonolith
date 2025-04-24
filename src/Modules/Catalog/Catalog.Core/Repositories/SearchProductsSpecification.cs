using Catalog.Core.ValueObjects;

namespace Catalog.Core.Repositories;

public class SearchProductsSpecification
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public Guid? CategoryId { get; set; }
    public string? SearchText { get; set; }
    public string? SortBy { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<AttributeValue>? Attributes { get; set; }
}
