
namespace Catalog.Application.Products.Commands;

public record UpdateProductInfo(
    Guid ProductId,
    string Name,
    string? Description,
    Guid? CategoryId) : ICommand;

internal class UpdateProductInfoHandler(IProductRepository productRepository)
    : ICommandHandler<UpdateProductInfo>
{
    public async Task<Result> Handle(UpdateProductInfo request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            return Result.Fail(new NotFoundError("Product not found"));

        var result = product.UpdateInfo(request.Name, request.Description, request.CategoryId);

        if (result.IsFailed)
            return result;

        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
