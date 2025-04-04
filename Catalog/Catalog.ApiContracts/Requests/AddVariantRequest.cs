namespace Catalog.ApiContracts.Requests;

public class AddVariantRequest
{
    public string Sku { get; set; }
    public decimal OriginalPrice { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAltText { get; set; }
    public List<ProductAttributeRequest> Attributes { get; set; }
    public DateTime? DiscountStartAt { get; set; }
    public DateTime? DiscountEndAt { get; set; }
    public decimal? SalePrice { get; set; }
}