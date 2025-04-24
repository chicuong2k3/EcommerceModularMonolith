using Catalog.Core.ReadModels;
using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Queries;

public sealed record GetProductById(Guid Id) : IQuery<ProductReadModel>;

internal sealed class GetProductByIdHandler(IReadProductRepository productRepository)
    : IQueryHandler<GetProductById, ProductReadModel>
{
    public async Task<Result<ProductReadModel>> Handle(GetProductById query, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(query.Id, cancellationToken);
        if (product == null)
        {
            return Result.Fail(new NotFoundError($"The product with id '{query.Id}' not found"));
        }

        return Result.Ok(product);
    }
}