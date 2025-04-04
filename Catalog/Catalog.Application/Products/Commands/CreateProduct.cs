namespace Catalog.Application.Products.Commands;

public sealed record CreateProduct(
    string Name,
    string? Description,
    Guid? CategoryId
) : ICommand<Product>;


internal sealed class CreateProductHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
    : ICommandHandler<CreateProduct, Product>
{
    public async Task<Result<Product>> Handle(CreateProduct command, CancellationToken cancellationToken)
    {
        if (command.CategoryId != null)
        {
            var category = await categoryRepository.GetByIdAsync(command.CategoryId.Value, cancellationToken);

            if (category == null)
                return Result.Fail(new NotFoundError($"The category with id '{command.CategoryId}' not found"));
        }

        var result = Product.Create(
            command.Name,
            command.Description,
            command.CategoryId);

        if (result.IsFailed)
            return result;

        var product = result.Value;

        await productRepository.AddAsync(product, cancellationToken);
        return result;
    }

}
