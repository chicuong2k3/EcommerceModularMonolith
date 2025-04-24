using Catalog.Core.Queries;
using Catalog.Core.ReadModels;
using Catalog.Core.Repositories;
using Dapper;
using System.Data;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class ReadCategoryRepository : IReadCategoryRepository
{
    private readonly IDbConnection dbConnection;

    public ReadCategoryRepository(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public async Task<IEnumerable<CategoryListItemReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                c."Id", 
                c."Name", 
                c."ParentCategoryId"
            FROM "catalog"."Categories" c
            ORDER BY c."Name"
            """;

        var categories = await dbConnection.QueryAsync<CategoryListItemReadModel>(sql);
        return categories;
    }

    public async Task<CategoryReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string categoryQuery = """
            SELECT 
                c."Id", 
                c."Name", 
                c."ParentCategoryId"
            FROM "catalog"."Categories" c
            WHERE c."Id" = @Id
            """;

        var category = await dbConnection.QueryFirstOrDefaultAsync<CategoryReadModel>(categoryQuery, new { Id = id });

        if (category == null)
            return null;

        const string subCategoriesQuery = """
            SELECT 
                c."Id", 
                c."Name", 
                c."ParentCategoryId"
            FROM "catalog"."Categories" c
            WHERE c."ParentCategoryId" = @ParentId
            """;

        var subCategories = await dbConnection.QueryAsync<CategoryReadModel>(
            subCategoriesQuery,
            new { ParentId = id });

        if (subCategories.Any())
        {
            var subCategoriesList = subCategories.ToList();
            category.SubCategories = subCategoriesList;
        }

        return category;
    }

    public async Task<IEnumerable<CategoryListItemReadModel>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                c."Id", 
                c."Name", 
                c."ParentCategoryId"
            FROM "catalog"."Categories" c
            WHERE c."ParentCategoryId" = @ParentId
            ORDER BY c."Name"
            """;

        var categories = await dbConnection.QueryAsync<CategoryListItemReadModel>(sql, new { ParentId = parentId });
        return categories;
    }
}
