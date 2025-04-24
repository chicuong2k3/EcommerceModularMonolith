using Catalog.Core.ReadModels;
using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Queries;

public sealed record GetCategoryById(Guid Id) : IQuery<CategoryReadModel>;

internal sealed class GetCategoryByIdHandler(IReadCategoryRepository categoryRepository)
    : IQueryHandler<GetCategoryById, CategoryReadModel>
{
    public async Task<Result<CategoryReadModel>> Handle(GetCategoryById query, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(query.Id, cancellationToken);
        if (category == null)
            return Result.Fail(new NotFoundError($"The category with id '{query.Id}' not found"));

        return Result.Ok(category);
    }
}