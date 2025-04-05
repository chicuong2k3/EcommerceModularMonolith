namespace Ordering.Application.Products.Commands;

public record AddVariantsToProduct(
    Guid ProductId,
    Guid VariantId,
    decimal OriginalPrice,
    decimal? SalePrice,
    int Quantity,
    string? ImageUrl,
    Dictionary<string, string> Attributes) : ICommand;

internal class AddVariantsToProductHandler(IProductRepository productRepository)
    : ICommandHandler<AddVariantsToProduct>
{
    public async Task<Result> Handle(AddVariantsToProduct command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetProductByIdAsync(command.ProductId, cancellationToken);
        if (product == null)
        {
            return Result.Fail(new NotFoundError($"Product with id '{command.ProductId}' not found"));
        }


        var attributesDescription = string.Join(", ", command.Attributes.Select(x => $"{x.Key}: {x.Value}"));
        var variant = new ProductVariant(command.VariantId, command.OriginalPrice, command.Quantity, command.ImageUrl, command.SalePrice, attributesDescription);

        product.AddVariant(variant);
        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
