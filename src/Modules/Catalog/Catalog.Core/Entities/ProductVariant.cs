using Catalog.Core.ValueObjects;
using FluentResults;
using Shared.Abstractions.Core;

namespace Catalog.Core.Entities;

public class ProductVariant
{
    public Guid Id { get; private set; }
    public Money OriginalPrice { get; private set; }
    public int Quantity { get; private set; }
    public Image? Image { get; private set; }
    public Money? SalePrice { get; private set; }
    public DateTimeRange? SalePriceEffectivePeriod { get; private set; }

    private readonly List<ProductVariantAttribute> attributes = [];
    public IReadOnlyCollection<ProductVariantAttribute> Attributes => attributes.AsReadOnly();

    private ProductVariant()
    {
    }

    private ProductVariant(
        Money originalPrice,
        int quantity,
        Image? image,
        Money? salePrice,
        DateTimeRange? salePriceEffectivePeriod)
    {
        Id = Guid.NewGuid();
        OriginalPrice = originalPrice;
        Quantity = quantity;
        Image = image;
        SalePrice = salePrice;
        SalePriceEffectivePeriod = salePriceEffectivePeriod;
    }

    public static Result<ProductVariant> Create(
        Money price,
        int quantity,
        Image? image,
        Money? salePrice,
        DateTimeRange? salePriceEffectivePeriod)
    {
        if (quantity <= 0)
            return Result.Fail(new ValidationError("Quantity must be greater than 0."));

        if (salePrice != null && salePriceEffectivePeriod == null)
            return Result.Fail(new ValidationError("Sale price effective period is required when sale price is set."));

        if (salePriceEffectivePeriod != null && salePrice == null)
            return Result.Fail(new ValidationError("Sale price is required when sale price effective period is set."));

        if (salePrice != null && salePrice > price)
            return Result.Fail(new ValidationError("Sale price must be less than the original price"));

        return new ProductVariant(price, quantity, image, salePrice, salePriceEffectivePeriod);
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
        if (newQuantity < 0)
        {
            return Result.Fail(new ValidationError($"The new quantity cannot be negative"));
        }

        Quantity = newQuantity;

        return Result.Ok();
    }

    public Result UpdateImage(Image? image)
    {
        if (image == null)
            return Result.Fail(new ValidationError("Image is required."));
        Image = image;
        return Result.Ok();
    }

    public Result UpdatePrice(Money originalPrice, Money? salePrice, DateTimeRange? salePriceEffectivePeriod)
    {
        OriginalPrice = originalPrice;

        if (salePrice != null && salePriceEffectivePeriod == null)
            return Result.Fail(new ValidationError("Sale price effective period is required when sale price is set."));
        if (salePriceEffectivePeriod != null && salePrice == null)
            return Result.Fail(new ValidationError("Sale price is required when sale price effective period is set."));

        if (salePrice != null && salePrice > OriginalPrice)
            return Result.Fail(new ValidationError("Sale price cannot be greater than original price."));

        if (salePrice != null)
        {
            SalePrice = salePrice;
            SalePriceEffectivePeriod = salePriceEffectivePeriod;
        }

        return Result.Ok();
    }

    public bool InStock()
    {
        return Quantity > 0;
    }
}
