using Catalog.Application.Products.ReadModels;

namespace Catalog.Application.Products.Queries;

public record SearchProducts(
    int PageSize,
    int PageNumber,
    Guid? CategoryId,
    string? SearchText,
    string? SortBy,
    decimal? MinPrice,
    decimal? MaxPrice,
    List<AttributeValue>? Attributes) : IQuery<PaginationResult<ProductReadModel>>;

internal sealed class SearchProductsQueryHandler(IDbConnection dbConnection)
    : IQueryHandler<SearchProducts, PaginationResult<ProductReadModel>>
{
    public async Task<Result<PaginationResult<ProductReadModel>>> Handle(SearchProducts query, CancellationToken cancellationToken)
    {
        var baseSql = """
            SELECT p."Id", p."Name", MIN(pv."OriginalPrice") AS minprice
            FROM "catalog"."Products" p
            LEFT JOIN "catalog"."ProductVariant" pv ON p."Id" = pv."ProductId"
            LEFT JOIN "catalog"."ProductVariantAttribute" pva ON pv."Id" = pva."ProductVariantId"
            LEFT JOIN "catalog"."ProductAttributes" pa ON pa."Id" = pva."AttributeId"
            WHERE 1 = 1
        """;

        var countBaseSql = """
            SELECT p."Id"
            FROM "catalog"."Products" p
            LEFT JOIN "catalog"."ProductVariant" pv ON p."Id" = pv."ProductId"
            LEFT JOIN "catalog"."ProductVariantAttribute" pva ON pv."Id" = pva."ProductVariantId"
            LEFT JOIN "catalog"."ProductAttributes" pa ON pa."Id" = pva."AttributeId"
            WHERE 1 = 1
        """;

        string orderByClause = """p."Name" ASC""";

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var sortParts = query.SortBy.Split(' ');
            var column = sortParts[0].ToLower();
            var direction = sortParts.Length > 1 ? sortParts[1].ToUpper() : "ASC";

            switch (column)
            {
                case "price":
                    orderByClause = $"minprice {(direction == "DESC" ? "DESC" : "ASC")}";
                    break;
                case "name":
                default:
                    orderByClause = $"""p."Name" {(direction == "DESC" ? "DESC" : "ASC")}""";
                    break;
            }
        }

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            baseSql += """ AND LOWER(p."Name") LIKE @SearchText""";
            countBaseSql += """ AND LOWER(p."Name") LIKE @SearchText""";
            parameters.Add("SearchText", $"%{query.SearchText.ToLower()}%");
        }

        if (query.CategoryId != null)
        {
            baseSql += """ AND p."CategoryId" = @CategoryId""";
            countBaseSql += """ AND p."CategoryId" = @CategoryId""";
            parameters.Add("CategoryId", query.CategoryId);
        }

        if (query.MinPrice != null)
        {
            baseSql += """ AND pv."OriginalPrice" >= @MinPrice""";
            countBaseSql += """ AND pv."OriginalPrice" >= @MinPrice""";
            parameters.Add("MinPrice", query.MinPrice);
        }

        if (query.MaxPrice != null)
        {
            baseSql += """ AND pv."OriginalPrice" <= @MaxPrice""";
            countBaseSql += """ AND pv."OriginalPrice" <= @MaxPrice""";
            parameters.Add("MaxPrice", query.MaxPrice);
        }

        if (query.Attributes != null && query.Attributes.Any())
        {
            var attributeConditions = new List<string>();
            for (int i = 0; i < query.Attributes.Count; i++)
            {
                var attributeNameParam = $"AttributeName{i}";
                var attributeValueParam = $"AttributeValue{i}";
                attributeConditions.Add($"""(pa."Name" = @{attributeNameParam} AND pva."Value" = @{attributeValueParam})""");
                parameters.Add(attributeNameParam, query.Attributes[i].AttributeName.ToLower());
                parameters.Add(attributeValueParam, query.Attributes[i].Value.ToLower());
            }
            baseSql += $" AND ({string.Join(" OR ", attributeConditions)})";
            countBaseSql += $" AND ({string.Join(" OR ", attributeConditions)})";
        }

        var countSql = $"""
            SELECT COUNT(DISTINCT "Id")
            FROM ({countBaseSql}) AS filtered_products
        """;
        var totalCount = await dbConnection.ExecuteScalarAsync<int>(countSql, parameters);

        var paginationSql = $"""
            {baseSql}
            GROUP BY p."Id", p."Name"
            ORDER BY {orderByClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
        """;
        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        var productIds = (await dbConnection.QueryAsync<Guid>(paginationSql, parameters)).ToList();

        if (!productIds.Any())
        {
            return new PaginationResult<ProductReadModel>(
                query.PageNumber,
                query.PageSize,
                totalCount,
                new List<ProductReadModel>()
            );
        }

        var sql = $"""
            SELECT 
                p."Id" AS "Id", 
                p."Name" AS "Name", 
                p."Description" AS "Description", 
                p."CategoryId" AS "CategoryId",
                pv."Id" AS "VariantId", 
                pv."Sku" AS "Sku", 
                pv."OriginalPrice" AS "OriginalPrice", 
                pv."Quantity" AS "Quantity", 
                pv."Image_Url" AS "ImageUrl", 
                pv."Image_AltText" AS "ImageAltText", 
                pv."SalePrice" AS "SalePrice", 
                pv."DiscountStart" AS "DiscountStart", 
                pv."DiscountEnd" AS "DiscountEnd",
                pa."Name" AS "AttributeName", 
                pva."Value" AS "AttributeValue",
                MIN(pv."OriginalPrice") OVER (PARTITION BY p."Id") AS minprice
            FROM "catalog"."Products" p
            LEFT JOIN "catalog"."ProductVariant" pv ON p."Id" = pv."ProductId"
            LEFT JOIN "catalog"."ProductVariantAttribute" pva ON pv."Id" = pva."ProductVariantId"
            LEFT JOIN "catalog"."ProductAttributes" pa ON pa."Id" = pva."AttributeId"
            WHERE p."Id" = ANY(@ProductIds)
            ORDER BY {orderByClause}
        """;

        var productDictionary = new Dictionary<Guid, ProductReadModel>();
        var results = await dbConnection.QueryAsync<ProductReadModel, ProductVariantReadModel, AttributeValueReadModel, ProductReadModel>(
            sql,
            (product, variant, attribute) =>
            {
                if (!productDictionary.TryGetValue(product.Id, out var productEntry))
                {
                    productEntry = product;
                    productEntry.Variants = new List<ProductVariantReadModel>();
                    productDictionary.Add(productEntry.Id, productEntry);
                }

                if (variant != null)
                {
                    var existingVariant = productEntry.Variants.FirstOrDefault(v => v.VariantId == variant.VariantId);
                    if (existingVariant == null)
                    {
                        variant.Attributes = new List<AttributeValueReadModel>();
                        if (attribute != null)
                            variant.Attributes.Add(attribute);
                        productEntry.Variants.Add(variant);
                    }
                    else
                    {
                        if (attribute != null)
                        {
                            existingVariant.Attributes ??= new List<AttributeValueReadModel>();
                            existingVariant.Attributes.Add(attribute);
                        }
                    }
                }

                return productEntry;
            },
            new { ProductIds = productIds },
            splitOn: "VariantId,AttributeName"
        );

        var products = productDictionary.Values.ToList();

        return Result.Ok(new PaginationResult<ProductReadModel>(
            query.PageNumber,
            query.PageSize,
            totalCount,
            products
        ));
    }
}