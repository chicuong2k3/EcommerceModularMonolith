namespace Ordering.Domain.ProductAggregate;

public sealed class Product : AggregateRoot
{
    private Product()
    {

    }

    public Guid VariantId { get; private set; }
    public string Name { get; private set; }
    public decimal OriginalPrice { get; private set; }
    public int Quantity { get; private set; }
    public string? ImageUrl { get; }
    public decimal? SalePrice { get; private set; }
    public string? AttributesDescription { get; set; }

    public Product(
        Guid id,
        Guid variantId,
        string name,
        decimal originalPrice,
        int quantity,
        string? imageUrl,
        decimal? salePrice,
        string? attributesDescription)
    {
        Id = id;
        VariantId = variantId;
        Name = name;
        OriginalPrice = originalPrice;
        Quantity = quantity;
        ImageUrl = imageUrl;
        SalePrice = salePrice;
        AttributesDescription = attributesDescription;
    }

    public Result UpdateQuantity(int newQuantity)
    {
        if (newQuantity < 0)
            return Result.Fail(new ValidationError("Product quantity must be greater than or equal to 0"));
        Quantity = newQuantity;
        return Result.Ok();
    }
}