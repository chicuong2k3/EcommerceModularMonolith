namespace Catalog.Contracts;

public class ProductVariantDto
{
    public Guid VariantId { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int Quantity { get; set; }
    public string? Image { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();
}
