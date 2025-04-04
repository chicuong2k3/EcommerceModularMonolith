using Catalog.Application.Categories.ReadModels;

namespace Catalog.Application.Categories.Queries;

public sealed record GetCategoryById(Guid Id) : IQuery<CategoryReadModel>;

internal sealed class GetCategoryByIdHandler(
    IDbConnection connection)
    : IQueryHandler<GetCategoryById, CategoryReadModel>
{
    public async Task<Result<CategoryReadModel>> Handle(GetCategoryById query, CancellationToken cancellationToken)
    {
        var sql = """
            SELECT * FROM "catalog"."Categories" WHERE "Id" = @Id;
            SELECT * FROM "catalog"."Categories" WHERE "ParentCategoryId" = @Id;
        """;

        using var multi = await connection.QueryMultipleAsync(sql, new { query.Id });

        var category = await multi.ReadSingleOrDefaultAsync<CategoryReadModel>();
        if (category == null)
            return Result.Fail(new NotFoundError($"The category with id '{query.Id}' not found"));

        category.SubCategories = (await multi.ReadAsync<CategoryReadModel>()).ToList();
        return Result.Ok(category);
    }
}