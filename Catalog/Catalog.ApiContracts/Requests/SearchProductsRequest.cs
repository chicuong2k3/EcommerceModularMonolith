namespace Catalog.ApiContracts.Requests;

public class SearchProductsRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Guid? CategoryId { get; set; }
    public string? SearchText { get; set; }
    public string? SortBy { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<ProductAttributeRequest>? Attributes { get; set; }
}