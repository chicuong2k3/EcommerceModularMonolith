namespace Ordering.Application.Carts.Commands;

public record ClearCart(Guid OwnerId) : ICommand;

internal sealed class DeleteCartHandler(ICartRepository cartRepository)
    : ICommandHandler<ClearCart>
{
    public async Task<Result> Handle(ClearCart command, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetAsync(command.OwnerId, cancellationToken);
        if (cart == null)
        {
            return Result.Ok();
        }

        cart.Items.Clear();
        await cartRepository.UpsertAsync(cart, cancellationToken);
        return Result.Ok();
    }
}