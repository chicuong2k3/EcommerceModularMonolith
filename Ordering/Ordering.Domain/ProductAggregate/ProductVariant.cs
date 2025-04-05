namespace Ordering.Domain.ProductAggregate;

public class ProductVariant : Entity
{
    private ProductVariant()
    {
    }

    public decimal OriginalPrice { get; private set; }
    public int Quantity { get; private set; }
    public string? ImageUrl { get; }
    public decimal? SalePrice { get; private set; }
    public string? AttributesDescription { get; set; }

    public ProductVariant(
        Guid id,
        decimal originalPrice,
        int quantity,
        string? imageUrl,
        decimal? salePrice,
        string? attributesDescription)
    {
        Id = id;
        OriginalPrice = originalPrice;
        Quantity = quantity;
        ImageUrl = imageUrl;
        SalePrice = salePrice;
        AttributesDescription = attributesDescription;
    }


}
