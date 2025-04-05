using FluentResults;
using Ordering.Domain.ProductAggregate;

namespace Ordering.Infrastructure.Services;

internal class ProductService : IProductService
{
    private readonly IProductRepository productRepository;

    public ProductService(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    public async Task<Result> ValidateProductAvailabilityAsync(Guid productId, Guid productVariantId, int quantity)
    {
        var product = await productRepository.GetProductByIdAsync(productId);
        if (product == null)
        {
            return Result.Fail(new NotFoundError($"Product with id '{productId}' not found"));
        }

        var variant = product.Variants.FirstOrDefault(v => v.Id == productVariantId);
        if (variant == null)
        {
            return Result.Fail(new NotFoundError($"Product variant with id '{productVariantId}' not found"));
        }

        if (variant.Quantity < quantity)
        {
            return Result.Fail($"The quantity is not available");
        }

        return Result.Ok();
    }
}
