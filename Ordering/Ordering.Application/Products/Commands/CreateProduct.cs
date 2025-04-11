namespace Ordering.Application.Products.Commands;

public record CreateProduct(
    Guid ProductId,
    Guid VariantId,
    string ProductName,
    decimal OriginalPrice,
    decimal? SalePrice,
    int Quantity,
    string? ImageUrl,
    Dictionary<string, string> Attributes) : ICommand;

internal class CreateProductHandler(IProductRepository productRepository)
    : ICommandHandler<CreateProduct>
{
    public async Task<Result> Handle(CreateProduct command, CancellationToken cancellationToken)
    {
        var attributesDescription = string.Join(", ", command.Attributes.Select(x => $"{x.Key}: {x.Value}"));
        var product = new Product(
            command.ProductId,
            command.VariantId,
            command.ProductName,
            command.OriginalPrice,
            command.Quantity,
            command.ImageUrl,
            command.SalePrice,
            attributesDescription);
        await productRepository.AddAsync(product, cancellationToken);

        return Result.Ok();
    }
}
