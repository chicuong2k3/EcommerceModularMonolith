using Catalog.Application.ProductAttributes.ReadModels;

namespace Catalog.Application.ProductAttributes.Queries;

public record GetAttributes() : IQuery<List<AttributeReadModel>>;

internal class GetAttributesHandler(IProductAttributeRepository productAttributeRepository)
    : IQueryHandler<GetAttributes, List<AttributeReadModel>>
{
    public async Task<Result<List<AttributeReadModel>>> Handle(GetAttributes request, CancellationToken cancellationToken)
    {
        var attributes = await productAttributeRepository.GetAttributesAsync(cancellationToken);
        return Result.Ok(attributes.Select(x => new AttributeReadModel
        {
            Id = x.Id,
            Name = x.Name
        }).ToList());
    }
}