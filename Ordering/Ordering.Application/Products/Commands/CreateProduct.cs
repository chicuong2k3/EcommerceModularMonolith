namespace Ordering.Application.Products.Commands;

public record CreateProduct(Guid ProductId, string ProductName) : ICommand;

internal class CreateProductHandler(IProductRepository productRepository)
    : ICommandHandler<CreateProduct>
{
    public async Task<Result> Handle(CreateProduct command, CancellationToken cancellationToken)
    {
        var product = new Product(command.ProductId, command.ProductName);
        await productRepository.AddAsync(product, cancellationToken);

        return Result.Ok();
    }
}
