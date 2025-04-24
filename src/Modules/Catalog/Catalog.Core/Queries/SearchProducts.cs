using Catalog.Core.ReadModels;
using Catalog.Core.Repositories;
using Catalog.Core.ValueObjects;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Queries;

public record SearchProducts(
    int PageSize,
    int PageNumber,
    Guid? CategoryId,
    string? SearchText,
    string? SortBy,
    decimal? MinPrice,
    decimal? MaxPrice,
    List<AttributeValue>? Attributes) : IQuery<PaginationResult<ProductReadModel>>;

internal sealed class SearchProductsHandler(IReadProductRepository productRepository)
    : IQueryHandler<SearchProducts, PaginationResult<ProductReadModel>>
{
    public async Task<Result<PaginationResult<ProductReadModel>>> Handle(SearchProducts query, CancellationToken cancellationToken)
    {
        var specification = new SearchProductsSpecification()
        {
            PageSize = query.PageSize,
            PageNumber = query.PageNumber,
            CategoryId = query.CategoryId,
            SearchText = query.SearchText,
            SortBy = query.SortBy,
            MinPrice = query.MinPrice,
            MaxPrice = query.MaxPrice,
            Attributes = query.Attributes
        };

        var result = await productRepository.SearchAsync(specification, cancellationToken);

        return Result.Ok(result);
    }
}