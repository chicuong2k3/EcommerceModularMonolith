using Ordering.Application.Carts.ReadModels;

namespace Ordering.Application.Carts.Queries;

public record GetCart(Guid OwnerId) : IQuery<CartReadModel>;

internal sealed class GetCartHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository)
    : IQueryHandler<GetCart, CartReadModel>
{
    public async Task<Result<CartReadModel>> Handle(GetCart query, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetAsync(query.OwnerId, cancellationToken);
        if (cart == null)
        {
            cart = new Cart(query.OwnerId);
        }

        var productTasks = cart.Items.Select(i =>
            productRepository.GetProductAsync(i.ProductId, i.ProductVariantId, cancellationToken)).ToList();
        var products = await Task.WhenAll(productTasks);

        var cartItems = cart.Items.Zip(products, (item, product) =>
        {
            if (product == null)
            {
                return new CartItemReadModel();
            }
            return new CartItemReadModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity,
                ProductName = product.Name,
                OriginalPrice = product.OriginalPrice,
                SalePrice = product.SalePrice,
                ImageUrl = product.ImageUrl,
                AttributesDescription = product.AttributesDescription
            };
        }).ToList();

        var cartReadModel = new CartReadModel
        {
            Id = cart.Id,
            OwnerId = cart.OwnerId,
            Items = cartItems
        };

        return Result.Ok(cartReadModel);
    }
}