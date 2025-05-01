using Catalog.ReadModels;
using Dapper;
using FluentResults;
using Shared.Abstractions.Application;
using System.Data;

namespace Catalog.Core.Queries;

public record GetCategories() : IQuery<IEnumerable<CategoryListItemReadModel>>;

internal sealed class GetCategoriesHandler(IDbConnection dbConnection)
    : IQueryHandler<GetCategories, IEnumerable<CategoryListItemReadModel>>
{
    public async Task<Result<IEnumerable<CategoryListItemReadModel>>> Handle(GetCategories query, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT 
                c."Id", 
                c."Name", 
                c."ParentCategoryId"
            FROM "catalog"."Categories" c
            ORDER BY c."Name"
            """
        ;

        var categories = await dbConnection.QueryAsync<CategoryListItemReadModel>(sql);

        return Result.Ok(categories);
    }
}
