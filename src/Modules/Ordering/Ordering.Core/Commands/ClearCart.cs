using FluentResults;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Shared.Abstractions.Application;

namespace Ordering.Core.Commands;

public record ClearCart(Guid OwnerId) : ICommand;

internal sealed class DeleteCartHandler(ICartRepository cartRepository)
    : ICommandHandler<ClearCart>
{
    public async Task<Result> Handle(ClearCart command, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetAsync(command.OwnerId, cancellationToken);
        if (cart == null)
        {
            cart = new Cart(command.OwnerId);
        }

        cart.Items.Clear();
        await cartRepository.UpsertAsync(cart, cancellationToken);
        return Result.Ok();
    }
}