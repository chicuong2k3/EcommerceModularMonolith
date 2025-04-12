using Ordering.Domain.ProductAggregate;

namespace Ordering.IntegrationTests.Orders;

public static class TestProductHelper
{
    public static Product CreateTestProduct(
        Guid productId,
        Guid variantId,
        string name,
        decimal price,
        int quantity,
        string? imageUrl = null,
        decimal? salePrice = null)
    {
        return new Product
        (
            productId,
            variantId,
            name,
            price,
            quantity,
            imageUrl ?? $"https://example.com/images/{productId}.jpg",
            salePrice,
            "Test product attributes"
        );
    }
}