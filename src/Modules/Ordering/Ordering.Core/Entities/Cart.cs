using FluentResults;
using Shared.Abstractions.Core;
using System.Text.Json.Serialization;

namespace Ordering.Core.Entities;

public class Cart : AggregateRoot
{
    [JsonInclude]
    public Guid OwnerId { get; private set; }
    [JsonInclude]
    public List<CartItem> Items { get; set; } = new();

    public Cart()
    {
    }

    public Cart(Guid id, Guid ownerId)
    {
        Id = id;
        OwnerId = ownerId;
        Items = new List<CartItem>();
    }

    public async Task<Result> AddItemAsync(
                        Guid productId,
                        Guid productVariantId,
                        int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Fail(new ValidationError("Quantity must be greater than zero"));
        }

        var existingItem = Items.FirstOrDefault(i => i.ProductVariantId == productVariantId);

        var result = Result.Ok();

        if (existingItem != null)
        {
            result = existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            Items.Add(new CartItem(productId, productVariantId, quantity));
        }

        return result;
    }

    public Result RemoveItem(Guid productVariantId, int quantity)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductVariantId == productVariantId);

        if (existingItem == null)
            return Result.Fail(new NotFoundError($"Product variant with id '{productVariantId}' not found"));

        var result = existingItem.DecreaseQuantity(quantity);
        if (result.IsSuccess && existingItem.Quantity == 0)
        {
            Items.Remove(existingItem);
        }

        return result;
    }

}
