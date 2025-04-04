namespace Ordering.Domain.OrderAggregate;

public class OrderItem : Entity
{
    public Money OriginalPrice { get; private set; }
    public Money SalePrice { get; private set; }
    public int Quantity { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductVariantId { get; private set; }


    private OrderItem()
    {
    }

    private OrderItem(
        Guid productId,
        Guid productVariantId,
        int quantity,
        Money originalPrice,
        Money salePrice)
    {
        ProductId = productId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
        OriginalPrice = originalPrice;
        SalePrice = salePrice;
    }

    public static Result<OrderItem> Create(
        Guid productId,
        Guid productVariantId,
        int quantity,
        Money originalPrice,
        Money salePrice)
    {
        if (quantity <= 0)
            return Result.Fail(new ValidationError("Quantity must be greater than 0"));

        return Result.Ok(new OrderItem(productId, productVariantId, quantity, originalPrice, salePrice));
    }
}