using FluentResults;
using Ordering.Core.ValueObjects;
using Shared.Abstractions.Core;

namespace Ordering.Core.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public Money OriginalPrice { get; private set; }
    public Money SalePrice { get; private set; }
    public int Quantity { get; private set; }
    public string ProductName { get; private set; }
    public string? Image { get; }
    public string? AttributesDescription { get; set; }

    private OrderItem()
    {
    }

    private OrderItem(
        Guid productId,
        Guid productVariantId,
        string productName,
        int quantity,
        Money originalPrice,
        Money salePrice,
        string? image,
        string? attributesDescription)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductVariantId = productVariantId;
        ProductName = productName;
        Quantity = quantity;
        OriginalPrice = originalPrice;
        SalePrice = salePrice;
        Image = image;
        AttributesDescription = attributesDescription;
    }

    public static Result<OrderItem> Create(
        Guid productId,
        Guid productVariantId,
        string productName,
        int quantity,
        Money originalPrice,
        Money salePrice,
        string? image,
        string? attributesDescription)
    {
        if (quantity <= 0)
            return Result.Fail(new ValidationError("Quantity must be greater than 0"));

        if (string.IsNullOrWhiteSpace(productName))
            return Result.Fail(new ValidationError("Product name cannot be empty"));

        return Result.Ok(new OrderItem(productId, productVariantId, productName, quantity, originalPrice, salePrice, image, attributesDescription));
    }
}