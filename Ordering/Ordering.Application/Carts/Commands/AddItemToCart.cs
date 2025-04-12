using Ordering.Domain.OrderAggregate.Errors;

namespace Ordering.Application.Carts.Commands;

public record AddItemToCart(Guid OwnerId,
                            List<AddItemDto> Items) : ICommand;

public record AddItemDto(
    Guid ProductId,
    Guid ProductVariantId,
    int Quantity);

internal sealed class AddItemToCartHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository)
    : ICommandHandler<AddItemToCart>
{
    public async Task<Result> Handle(AddItemToCart command, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetAsync(command.OwnerId, cancellationToken);

        if (cart == null)
        {
            cart = new Cart(command.OwnerId);
        }

        foreach (var item in command.Items)
        {
            var product = await productRepository.GetProductAsync(item.ProductId, item.ProductVariantId, cancellationToken);

            if (product == null)
            {
                return Result.Fail(new NotFoundError($"Product with id {item.ProductId} not found."));
            }

            if (product.Quantity < item.Quantity)
            {
                return Result.Fail(new OutOfStockError(product.Name, product.Quantity, item.Quantity));
            }

            var addItemResult = await cart.AddItemAsync(
                item.ProductId,
                item.ProductVariantId,
                item.Quantity);

            if (addItemResult.IsFailed)
            {
                return addItemResult;
            }
        }

        await cartRepository.UpsertAsync(cart, cancellationToken);

        return Result.Ok();
    }
}
