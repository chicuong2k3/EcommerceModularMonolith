namespace Catalog.Domain.ProductAggregate;

public class ProductVariant : Entity
{
    public string Sku { get; private set; }
    public Money OriginalPrice { get; private set; }
    public int Quantity { get; private set; }
    public Image? Image { get; }
    public Money? SalePrice { get; private set; }
    public DateTimeRange? SalePriceEffectivePeriod { get; private set; }

    private readonly List<ProductVariantAttribute> attributes = [];
    public IReadOnlyCollection<ProductVariantAttribute> Attributes => attributes.AsReadOnly();

    private ProductVariant()
    {
    }

    private ProductVariant(
        string sku,
        Money originalPrice,
        int quantity,
        Image? image,
        Money? salePrice,
        DateTimeRange? salePriceEffectivePeriod)
    {
        Id = Guid.NewGuid();
        Sku = sku;
        OriginalPrice = originalPrice;
        Quantity = quantity;
        Image = image;
        SalePrice = salePrice;
        SalePriceEffectivePeriod = salePriceEffectivePeriod;
    }

    public static Result<ProductVariant> Create(
        string sku,
        Money price,
        int quantity,
        Image? image,
        Money? salePrice,
        DateTimeRange? salePriceEffectivePeriod)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return Result.Fail(new ValidationError("SKU is required."));
        if (quantity <= 0)
            return Result.Fail(new ValidationError("Quantity must be greater than 0."));

        if (salePrice != null && salePriceEffectivePeriod == null)
            return Result.Fail(new ValidationError("Sale price effective period is required when sale price is set."));

        if (salePriceEffectivePeriod != null && salePrice == null)
            return Result.Fail(new ValidationError("Sale price is required when sale price effective period is set."));

        return new ProductVariant(sku, price, quantity, image, salePrice, salePriceEffectivePeriod);
    }

    public Result AddAttribute(ProductAttribute attribute, string value)
    {
        if (attributes.Any(a => a.AttributeId == attribute.Id))
            return Result.Fail(new ValidationError($"Attribute '{attribute.Name}' already exists."));

        attributes.Add(new ProductVariantAttribute(attribute.Id, value));
        return Result.Ok();
    }

    internal Result UpdateVariantQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            return Result.Fail(new ValidationError("Quantity must be greater than 0."));

        Quantity = newQuantity;

        return Result.Ok();
    }
}
