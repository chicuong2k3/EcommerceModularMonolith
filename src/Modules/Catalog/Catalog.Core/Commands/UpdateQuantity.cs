using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public record UpdateQuantity(Guid ProductId, Guid ProductVariantId, int NewQuantity) : ICommand;

internal sealed class UpdateQuantityHandler(IProductRepository productRepository)
    : ICommandHandler<UpdateQuantity>
{
    public async Task<Result> Handle(UpdateQuantity command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdWithVariantsAsync(command.ProductId, cancellationToken);

        if (product == null)
            return Result.Fail(new NotFoundError($"The product with id '{command.ProductId}' not found"));

        var result = product.UpdateQuantity(command.ProductVariantId, command.NewQuantity);

        if (result.IsFailed)
            return result;


        await productRepository.SaveChangesAsync(cancellationToken);
        return result;
    }
}
