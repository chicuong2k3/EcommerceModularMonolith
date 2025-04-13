using Catalog.Contracts;
using Ordering.Application.Carts.ReadModels;

namespace Ordering.Application.Carts.Queries;

public record GetCart(Guid OwnerId) : IQuery<CartReadModel>;

internal sealed class GetCartHandler(
    ICartRepository cartRepository,
    IProductService productService)
    : IQueryHandler<GetCart, CartReadModel>
{
    public async Task<Result<CartReadModel>> Handle(GetCart query, CancellationToken cancellationToken)
    {
        // Get or create the cart
        var cart = await cartRepository.GetAsync(query.OwnerId, cancellationToken);
        if (cart == null)
        {
            cart = new Cart(query.OwnerId);
            await cartRepository.UpsertAsync(cart, cancellationToken);
        }

        // Get product information for each cart item
        var validCartItems = new List<CartItemReadModel>();

        foreach (var item in cart.Items)
        {
            var product = await productService.GetProductByIdAsync(item.ProductId, cancellationToken);
            if (product == null)
            {
                continue;
            }

            var variant = product.Variants.FirstOrDefault(v => v.VariantId == item.ProductVariantId);
            if (variant == null)
            {
                continue;
            }

            // Build attributes description
            string attributesDescription = string.Join(", ",
                variant.Attributes.Select(a => $"{a.Key}: {a.Value}"));

            // Create cart item
            var cartItem = new CartItemReadModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity,
                ProductName = product.Name,
                OriginalPrice = variant.OriginalPrice,
                SalePrice = variant.SalePrice,
                ImageUrl = variant.ImageUrl,
                AttributesDescription = attributesDescription
            };

            validCartItems.Add(cartItem);
        }

        // Create cart read model
        var cartReadModel = new CartReadModel
        {
            Id = cart.Id,
            OwnerId = cart.OwnerId,
            Items = validCartItems
        };

        return Result.Ok(cartReadModel);
    }
}