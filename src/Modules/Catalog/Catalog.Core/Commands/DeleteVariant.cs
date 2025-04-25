using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public record DeleteVariant(Guid ProductId, Guid VariantId) : ICommand;

internal class DeleteVariantHandler(IProductRepository productRepository)
    : ICommandHandler<DeleteVariant>
{
    public async Task<Result> Handle(DeleteVariant command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdWithVariantsAsync(command.ProductId, cancellationToken);
        if (product == null)
        {
            return Result.Fail(new NotFoundError($"The product with id '{command.ProductId}' not found."));
        }

        var result = product.RemoveVariant(command.VariantId);
        if (result.IsFailed)
        {
            return result;
        }

        await productRepository.SaveChangesAsync(cancellationToken);
        return result;
    }
}