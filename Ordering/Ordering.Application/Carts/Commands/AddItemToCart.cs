using Ordering.Application.Carts.ReadModels;

namespace Ordering.Application.Carts.Commands;

public record AddItemToCart(Guid OwnerId,
                            List<AddItemDto> Items) : ICommand<CartReadModel>;

public record AddItemDto(
    Guid ProductId,
    Guid ProductVariantId,
    decimal OriginalPrice,
    decimal SalePrice,
    int Quantity);

internal sealed class AddItemToCartHandler(
    ICartRepository cartRepository,
    IProductService productService)
    : ICommandHandler<AddItemToCart, CartReadModel>
{
    public async Task<Result<CartReadModel>> Handle(AddItemToCart command, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetAsync(command.OwnerId, cancellationToken);

        if (cart == null)
        {
            cart = new Cart(command.OwnerId);
        }

        foreach (var item in command.Items)
        {
            var originalPriceCreationResult = Money.FromDecimal(item.OriginalPrice);
            var salePriceCreationResult = Money.FromDecimal(item.SalePrice);

            if (originalPriceCreationResult.IsFailed || salePriceCreationResult.IsFailed)
            {
                return Result.Fail(originalPriceCreationResult.Errors.Concat(salePriceCreationResult.Errors));
            }

            var addItemResult = await cart.AddItemAsync(
                item.ProductId,
                item.ProductVariantId,
                originalPriceCreationResult.Value,
                salePriceCreationResult.Value,
                item.Quantity,
                productService);

            if (addItemResult.IsFailed)
            {
                return addItemResult;
            }
        }

        await cartRepository.UpsertAsync(cart, cancellationToken);

        return Result.Ok(new CartReadModel()
        {
            Id = cart.Id,
            OwnerId = cart.OwnerId,
            Items = cart.Items.Select(i => new CartItemReadModel()
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductVariantId = i.ProductVariantId,
                OriginalPrice = i.OriginalPrice,
                SalePrice = i.SalePrice,
                Quantity = i.Quantity
            }).ToList()
        });
    }
}
