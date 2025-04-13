using Catalog.Contracts;
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
    IProductService productService)
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
            var product = await productService.GetProductByIdAsync(item.ProductId, cancellationToken);

            if (product == null)
            {
                return Result.Fail(new NotFoundError($"Product with id {item.ProductId} not found."));
            }

            var productVariant = product.Variants.FirstOrDefault(v => v.VariantId == item.ProductVariantId);
            if (productVariant == null)
            {
                return Result.Fail(new NotFoundError($"Product variant with id {item.ProductVariantId} not found."));
            }

            if (productVariant.Quantity < item.Quantity)
            {
                return Result.Fail(new OutOfStockError(product.Name, productVariant.Quantity, item.Quantity));
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
