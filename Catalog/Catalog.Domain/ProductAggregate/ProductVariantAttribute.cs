namespace Catalog.Domain.ProductAggregate;

public class ProductVariantAttribute
{
    public Guid AttributeId { get; }
    public string Value { get; }

    private ProductVariantAttribute() { }

    internal ProductVariantAttribute(Guid attributeId, string value)
    {
        AttributeId = attributeId;
        Value = value;
    }
}