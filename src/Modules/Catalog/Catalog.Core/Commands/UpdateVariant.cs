using Catalog.Core.Repositories;
using Catalog.Core.ValueObjects;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public record UpdateVariant(
    Guid ProductId,
    Guid ProductVariantId,
    int Quantity,
    decimal OriginalPrice,
    decimal? SalePrice,
    DateTime? SaleStartDate,
    DateTime? SaleEndDate,
    string? ImageData,
    string? ImageAltText) : ICommand;

internal sealed class UpdateVariantHandler(IProductRepository productRepository)
    : ICommandHandler<UpdateVariant>
{
    public async Task<Result> Handle(UpdateVariant command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdWithVariantsAsync(command.ProductId, cancellationToken);

        if (product == null)
            return Result.Fail(new NotFoundError($"The product with id '{command.ProductId}' not found"));

        var originalPriceCreationResult = Money.FromDecimal(command.OriginalPrice);
        if (originalPriceCreationResult.IsFailed)
            return originalPriceCreationResult.ToResult();
        Money? salePrice = null;
        DateTimeRange? salePriceEffectivePeriod = null;
        if (command.SaleStartDate != null && command.SaleEndDate != null && command.SalePrice != null)
        {
            var salePriceCreationResult = Money.FromDecimal(command.SalePrice.Value);
            if (salePriceCreationResult.IsFailed)
                return Result.Fail(salePriceCreationResult.Errors);

            var datetimeRangeCreateResult = DateTimeRange.Create(command.SaleStartDate.Value, command.SaleEndDate.Value);
            if (datetimeRangeCreateResult.IsFailed)
                return Result.Fail(datetimeRangeCreateResult.Errors);

            salePrice = salePriceCreationResult.Value;
            salePriceEffectivePeriod = datetimeRangeCreateResult.Value;

        }

        Image? image = null;
        if (!string.IsNullOrEmpty(command.ImageData))
        {
            var imageCreateResult = Image.Create(command.ImageData, command.ImageAltText);
            if (imageCreateResult.IsFailed)
                return Result.Fail(imageCreateResult.Errors);
            image = imageCreateResult.Value;
        }

        var result = product.UpdateVariant(command.ProductVariantId, command.Quantity, originalPriceCreationResult.Value, salePrice, salePriceEffectivePeriod, image);

        if (result.IsFailed)
            return result;


        await productRepository.SaveChangesAsync(cancellationToken);
        return result;
    }
}
