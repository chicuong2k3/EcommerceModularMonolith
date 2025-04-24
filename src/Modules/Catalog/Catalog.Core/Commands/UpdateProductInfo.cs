using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public record UpdateProductInfo(
    Guid ProductId,
    string Name,
    string? Description,
    Guid? CategoryId) : ICommand;

internal class UpdateProductInfoHandler(
    IWriteProductRepository productRepository,
    IWriteCategoryRepository categoryRepository)
    : ICommandHandler<UpdateProductInfo>
{
    public async Task<Result> Handle(UpdateProductInfo command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product == null)
            return Result.Fail(new NotFoundError("The product not found"));

        if (command.CategoryId != null)
        {
            var category = await categoryRepository.GetByIdAsync(command.CategoryId.Value, cancellationToken);
            if (category == null)
                return Result.Fail(new NotFoundError($"The category with id '{command.CategoryId}' not found"));
        }

        var result = product.UpdateInfo(command.Name, command.Description, command.CategoryId);

        if (result.IsFailed)
            return result;

        await productRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
