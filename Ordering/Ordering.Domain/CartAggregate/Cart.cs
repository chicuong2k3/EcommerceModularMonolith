using System.Text.Json.Serialization;

namespace Ordering.Domain.CartAggregate;

public class Cart : AggregateRoot
{
    [JsonInclude]
    public Guid OwnerId { get; private set; }
    [JsonInclude]
    public DateTime CreatedAt { get; private set; }
    [JsonInclude]
    public List<CartItem> Items { get; set; } = new();

    public Cart()
    {
    }

    public Cart(Guid ownerId)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        CreatedAt = DateTime.UtcNow;
        Items = new List<CartItem>();
    }

    public async Task<Result> AddItemAsync(
                        Guid productId,
                        Guid productVariantId,
                        int quantity,
                        IProductService productService)
    {
        if (quantity <= 0)
        {
            return Result.Fail(new ValidationError("Quantity must be greater than zero"));
        }

        var productAvailabilityResult = await productService.ValidateProductAvailabilityAsync(productId, productVariantId, quantity);

        if (productAvailabilityResult.IsFailed)
        {
            return Result.Fail(productAvailabilityResult.Errors);
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

        existingItem.DecreaseQuantity(quantity);
        if (existingItem.Quantity == 0)
        {
            Items.Remove(existingItem);
        }

        return Result.Ok();
    }

}
