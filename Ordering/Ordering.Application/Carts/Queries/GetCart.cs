using Ordering.Application.Carts.ReadModels;

namespace Ordering.Application.Carts.Queries;

public record GetCart(Guid OwnerId) : IQuery<CartReadModel>;

internal sealed class GetCartHandler(ICartRepository cartRepository)
    : IQueryHandler<GetCart, CartReadModel>
{
    public async Task<Result<CartReadModel>> Handle(GetCart query, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetAsync(query.OwnerId);

        if (cart == null)
        {
            cart = new Cart(query.OwnerId);
        }

        return Result.Ok(new CartReadModel()
        {
            Id = cart.Id,
            OwnerId = cart.OwnerId,
            Items = cart.Items.Select(i => new CartItemReadModel()
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductVariantId = i.ProductVariantId,
                Quantity = i.Quantity
            }).ToList()
        });
    }
}
