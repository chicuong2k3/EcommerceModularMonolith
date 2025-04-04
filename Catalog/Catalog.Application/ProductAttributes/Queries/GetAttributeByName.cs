using Catalog.Application.ProductAttributes.ReadModels;

namespace Catalog.Application.ProductAttributes.Queries;

public record GetAttributeByName(string Name) : IQuery<AttributeReadModel>;


internal class GetAttributeByNameHandler(IProductAttributeRepository productAttributeRepository)
    : IQueryHandler<GetAttributeByName, AttributeReadModel>
{
    public async Task<Result<AttributeReadModel>> Handle(GetAttributeByName query, CancellationToken cancellationToken)
    {
        var attribute = await productAttributeRepository.GetByNameAsync(query.Name, cancellationToken);

        if (attribute == null)
            return Result.Fail(new NotFoundError($"ProductAttribute with name '{query.Name}' not found"));

        return Result.Ok(new AttributeReadModel()
        {
            Id = attribute.Id,
            Name = attribute.Name
        });
    }
}
