namespace Catalog.Application.ProductAttributes.Commands;

public record DeleteAttribute(string Name) : ICommand;

internal class DeleteAttributeHandler(IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<DeleteAttribute>
{
    public async Task<Result> Handle(DeleteAttribute command, CancellationToken cancellationToken)
    {
        var productAttribute = await productAttributeRepository.GetByNameAsync(command.Name, cancellationToken);

        if (productAttribute == null)
            return Result.Fail(new NotFoundError($"ProductAttribute with name '{command.Name}' not found"));

        await productAttributeRepository.RemoveAsync(productAttribute, cancellationToken);
        return Result.Ok();
    }
}
