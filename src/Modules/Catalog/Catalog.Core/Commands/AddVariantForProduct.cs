using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.ValueObjects;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public sealed record AddVariantForProduct(
    Guid ProductId,
    decimal OriginalPrice,
    int Quantity,
    string? ImageUrl,
    string? ImageAltText,
    IEnumerable<AttributeValue> Attributes,
    DateTime? DiscountStartAt,
    DateTime? DiscountEndAt,
    decimal? SalePrice
) : ICommand;


internal sealed class AddVariantForProductHandler(
    IProductRepository productRepository,
    IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<AddVariantForProduct>
{
    public async Task<Result> Handle(AddVariantForProduct command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdWithVariantsAsync(command.ProductId, cancellationToken);

        if (product == null)
            return Result.Fail(new NotFoundError($"The product with id '{command.ProductId}' not found"));

        Image? image = null;

        if (!string.IsNullOrEmpty(command.ImageUrl))
        {
            var imageCreateResult = Image.Create(command.ImageUrl, command.ImageAltText);
            if (imageCreateResult.IsFailed)
                return Result.Fail(imageCreateResult.Errors);

            image = imageCreateResult.Value;
        }

        var priceCreateResult = Money.FromDecimal(command.OriginalPrice);
        if (priceCreateResult.IsFailed)
            return Result.Fail(priceCreateResult.Errors);


        Money? salePrice = null;
        DateTimeRange? salePriceEffectivePeriod = null;
        if (command.DiscountStartAt != null && command.DiscountEndAt != null && command.SalePrice != null)
        {
            var salePriceCreationResult = Money.FromDecimal(command.SalePrice.Value);
            if (salePriceCreationResult.IsFailed)
                return Result.Fail(salePriceCreationResult.Errors);

            var datetimeRangeCreateResult = DateTimeRange.Create(command.DiscountStartAt.Value, command.DiscountEndAt.Value);
            if (datetimeRangeCreateResult.IsFailed)
                return Result.Fail(datetimeRangeCreateResult.Errors);

            salePrice = salePriceCreationResult.Value;
            salePriceEffectivePeriod = datetimeRangeCreateResult.Value;
        }

        var variantCreationResult = ProductVariant.Create(
            priceCreateResult.Value,
            command.Quantity,
            image,
            salePrice,
            salePriceEffectivePeriod);

        if (variantCreationResult.IsFailed)
            return Result.Fail(variantCreationResult.Errors);

        var variant = variantCreationResult.Value;

        foreach (var attribute in command.Attributes)
        {
            var existingAttribute = await productAttributeRepository.GetByNameAsync(attribute.AttributeName);

            if (existingAttribute == null)
                return Result.Fail(new NotFoundError($"The attribute with name '{attribute.AttributeName}' not found"));

            variant.AddAttribute(existingAttribute, attribute.Value);
        }


        product.AddVariant(variant, command.Attributes.ToDictionary(kvp => kvp.AttributeName, kvp => kvp.Value));

        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
