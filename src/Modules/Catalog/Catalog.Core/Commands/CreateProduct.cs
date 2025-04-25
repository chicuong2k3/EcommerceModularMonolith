using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public sealed record CreateProduct(
    Guid Id,
    string Name,
    string? Description,
    Guid? CategoryId
) : ICommand;


internal sealed class CreateProductHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
    : ICommandHandler<CreateProduct>
{
    public async Task<Result> Handle(CreateProduct command, CancellationToken cancellationToken)
    {
        if (command.CategoryId != null)
        {
            var category = await categoryRepository.GetByIdAsync(command.CategoryId.Value, cancellationToken);

            if (category == null)
                return Result.Fail(new NotFoundError($"The category with id '{command.CategoryId}' not found"));
        }

        var result = Product.Create(
            command.Id,
            command.Name,
            command.Description,
            command.CategoryId);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        var product = result.Value;

        await productRepository.AddAsync(product, cancellationToken);
        return Result.Ok();
    }

}
