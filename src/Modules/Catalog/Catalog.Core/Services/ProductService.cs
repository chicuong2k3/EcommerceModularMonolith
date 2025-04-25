using Catalog.Contracts;
using Catalog.Core.Queries;
using MediatR;

namespace Catalog.Core.Services;

internal class ProductService : IProductService
{
    private readonly IMediator mediator;

    public ProductService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductById(productId), cancellationToken);
        if (result.IsFailed)
        {
            return null;
        }

        return new ProductDto
        {
            Id = result.Value.Id,
            Name = result.Value.Name,
            Variants = result.Value.Variants.Select(v => new ProductVariantDto
            {
                VariantId = v.VariantId,
                OriginalPrice = v.OriginalPrice,
                SalePrice = v.SalePrice,
                Quantity = v.Quantity,
                ImageUrl = v.ImageUrl,
                Attributes = v.Attributes.ToDictionary(a => a.AttributeName, a => a.AttributeValue),
            }).ToList()
        };
    }
}
