using Catalog.ReadModels;
using Dapper;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;
using System.Data;

namespace Catalog.Core.Queries;

public sealed record GetProductById(Guid Id) : IQuery<ProductReadModel>;

internal sealed class GetProductByIdHandler(IDbConnection dbConnection)
    : IQueryHandler<GetProductById, ProductReadModel>
{
    public async Task<Result<ProductReadModel>> Handle(GetProductById query, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT 
                p."Id" AS "Id", 
                p."Name" AS "Name", 
                p."Description" AS "Description", 
                p."CategoryId" AS "CategoryId",
                pv."Id" AS "VariantId",
                pv."OriginalPrice" AS "OriginalPrice", 
                pv."Quantity" AS "Quantity", 
                pv."ImageData" AS "ImageData", 
                pv."ImageAltText" AS "ImageAltText", 
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
            WHERE p."Id" = @ProductId
            """
        ;
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
                    var existingVariant = productEntry.Variants
                        .FirstOrDefault(v => v.VariantId == variant.VariantId);

                    if (existingVariant == null)
                    {
                        variant.Attributes = new List<AttributeValueReadModel>();
                        if (attribute != null)
                        {
                            variant.Attributes.Add(attribute);
                        }
                        productEntry.Variants.Add(variant);
                    }
                    else if (attribute != null)
                    {
                        existingVariant.Attributes ??= new List<AttributeValueReadModel>();
                        existingVariant.Attributes.Add(attribute);
                    }
                }

                return productEntry;
            },
            new { ProductId = query.Id },
            splitOn: "VariantId,AttributeName"
        );

        var product = productDictionary.Values.FirstOrDefault();
        if (product == null)
        {
            return Result.Fail(new NotFoundError($"The product with id '{query.Id}' not found"));
        }

        return Result.Ok(product);
    }
}