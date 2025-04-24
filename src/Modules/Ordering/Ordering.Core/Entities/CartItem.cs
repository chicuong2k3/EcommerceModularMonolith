using FluentResults;
using Shared.Abstractions.Core;
using System.Text.Json.Serialization;

namespace Ordering.Core.Entities;

public class CartItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public int Quantity { get; private set; }

    public CartItem()
    {

    }

    [JsonConstructor]
    internal CartItem(
        Guid productId,
        Guid productVariantId,
        int quantity)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
    }

    public Result IncreaseQuantity(int additionalQuantity)
    {
        if (additionalQuantity <= 0)
        {
            return Result.Fail(new ValidationError("Additional quantity must be greater than zero"));
        }

        Quantity += additionalQuantity;
        return Result.Ok();
    }

    public Result DecreaseQuantity(int reducedQuantity)
    {
        if (reducedQuantity <= 0)
        {
            return Result.Fail(new ValidationError("Reduced quantity must be greater than zero"));
        }

        if (Quantity < reducedQuantity)
        {
            return Result.Fail(new ValidationError($"Reduced quantity cannot be greater than {Quantity}"));
        }

        Quantity -= reducedQuantity;
        return Result.Ok();
    }
}
