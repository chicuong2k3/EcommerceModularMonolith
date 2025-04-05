using Common.Messages;

namespace Catalog.Contracts;

public class ProductVariantAddedIntegrationEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();
}
