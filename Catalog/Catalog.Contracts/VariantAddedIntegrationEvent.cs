using Common.Messages;

namespace Catalog.Contracts;

public class VariantAddedIntegrationEvent : IntegrationEvent
{
    public VariantAddedIntegrationEvent(Guid productId, Guid variantId, string name, decimal originalPrice, int quantity, string? imageUrl, decimal? salePrice, Dictionary<string, string> attributes)
    {
        ProductId = productId;
        VariantId = variantId;
        Name = name;
        OriginalPrice = originalPrice;
        Quantity = quantity;
        ImageUrl = imageUrl;
        SalePrice = salePrice;
        Attributes = attributes;
    }

    public Guid ProductId { get; private set; }
    public Guid VariantId { get; private set; }
    public string Name { get; private set; }
    public decimal OriginalPrice { get; private set; }
    public int Quantity { get; private set; }
    public string? ImageUrl { get; private set; }
    public decimal? SalePrice { get; private set; }
    public Dictionary<string, string> Attributes { get; private set; }
}
