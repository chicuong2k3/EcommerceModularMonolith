using Catalog.Core.Repositories;
using Catalog.ReadModels;
using FluentResults;
using Shared.Abstractions.Application;

namespace Catalog.Core.Queries;

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