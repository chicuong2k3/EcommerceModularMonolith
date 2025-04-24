using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;

namespace Catalog.Core.Queries;

public record GetCategories() : IQuery<IEnumerable<CategoryListItemReadModel>>;

internal sealed class GetCategoriesHandler(IReadCategoryRepository categoryRepository)
    : IQueryHandler<GetCategories, IEnumerable<CategoryListItemReadModel>>
{
    public async Task<Result<IEnumerable<CategoryListItemReadModel>>> Handle(GetCategories query, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);

        return Result.Ok(categories);
    }
}
