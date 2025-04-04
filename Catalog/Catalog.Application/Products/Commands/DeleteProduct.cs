namespace Catalog.Application.Products.Commands;

public record DeleteProduct(Guid Id) : ICommand;

internal sealed class DeleteProductHandler(
    IProductRepository productRepository)
    : ICommandHandler<DeleteProduct>
{

    public async Task<Result> Handle(DeleteProduct command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (product == null)
            return Result.Fail(new NotFoundError($"The product with id '{command.Id}' not found"));

        await productRepository.RemoveAsync(product, cancellationToken);
        return Result.Ok();
    }
}