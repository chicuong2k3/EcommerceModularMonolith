namespace Catalog.Application.Products.ReadModels;

public class ProductVariantReadModel
{
    public Guid VariantId { get; set; }
    public string Sku { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAltText { get; set; }
    public DateTime? DiscountStart { get; set; }
    public DateTime? DiscountEnd { get; set; }
    public List<AttributeValueReadModel> Attributes { get; set; } = new();
}