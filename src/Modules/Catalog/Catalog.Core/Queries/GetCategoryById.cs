using Catalog.Core.ReadModels;
using Dapper;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;
using System.Data;

namespace Catalog.Core.Queries;

public sealed record GetCategoryById(Guid Id) : IQuery<CategoryReadModel>;

internal sealed class GetCategoryByIdHandler(IDbConnection dbConnection)
    : IQueryHandler<GetCategoryById, CategoryReadModel>
{
    public async Task<Result<CategoryReadModel>> Handle(GetCategoryById query, CancellationToken cancellationToken)
    {
        const string categoryQuery = """
            SELECT 
                c."Id", 
                c."Name", 
                c."ParentCategoryId"
            FROM "catalog"."Categories" c
            WHERE c."Id" = @Id
            """
        ;

        var category = await dbConnection.QueryFirstOrDefaultAsync<CategoryReadModel>(categoryQuery, new { Id = query.Id });

        if (category == null)
            return Result.Fail(new NotFoundError($"The category with id '{query.Id}' not found"));

        const string subCategoriesQuery = """
            SELECT 
                c."Id", 
                c."Name", 
                c."ParentCategoryId"
            FROM "catalog"."Categories" c
            WHERE c."ParentCategoryId" = @ParentId
            """
        ;

        var subCategories = await dbConnection.QueryAsync<CategoryReadModel>(
            subCategoriesQuery,
            new { ParentId = query.Id });

        if (subCategories.Any())
        {
            var subCategoriesList = subCategories.ToList();
            category.SubCategories = subCategoriesList;
        }



        return Result.Ok(category);
    }
}