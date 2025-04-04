namespace Ordering.Application.Carts.Commands;

public record RemoveItemFromCart(
    Guid OwnerId,
    Guid ProductVariantId,
    int Quantity) : ICommand;

internal sealed class RemoveItemFromCartHandler(
    ICartRepository cartRepository)
    : ICommandHandler<RemoveItemFromCart>
{
    public async Task<Result> Handle(RemoveItemFromCart command, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetAsync(command.OwnerId, cancellationToken);
        if (cart == null)
        {
            return Result.Fail("Cart not found");
        }

        var result = cart.RemoveItem(command.ProductVariantId, command.Quantity);

        if (result.IsFailed)
        {
            return result;
        }

        await cartRepository.UpsertAsync(cart, cancellationToken);
        return result;
    }
}