using Catalog.Application.Categories.ReadModels;

namespace Catalog.Application.Categories.Queries;

public record GetCategories() : IQuery<List<CategoryListItemReadModel>>;

internal sealed class GetCategoriesHandler(
    IDbConnection connection)
    : IQueryHandler<GetCategories, List<CategoryListItemReadModel>>
{
    public async Task<Result<List<CategoryListItemReadModel>>> Handle(GetCategories query, CancellationToken cancellationToken)
    {
        var sql = """
            SELECT "Id", "Name" FROM "catalog"."Categories"
            """;

        var categories = (await connection.QueryAsync<CategoryListItemReadModel>(sql)).ToList();

        return Result.Ok(categories);
    }
}
