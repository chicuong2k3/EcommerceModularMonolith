namespace Ordering.Application.Products.Commands;

public record ReduceProductQuantity(Guid ProductId, Guid VariantId, int Quantity) : ICommand;

internal class UpdateProductQuantityHandler(
    IProductRepository productRepository) : ICommandHandler<ReduceProductQuantity>
{
    public async Task<Result> Handle(ReduceProductQuantity command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetProductAsync(command.ProductId, command.VariantId, cancellationToken);
        if (product == null)
            return Result.Fail(new NotFoundError($"Product with id '{command.ProductId}' and variant id '{command.VariantId}' not found"));

        product.UpdateQuantity(product.Quantity - command.Quantity);
        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}