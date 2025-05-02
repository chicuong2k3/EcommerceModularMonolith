using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public record DeleteAttribute(Guid Id) : ICommand;

internal class DeleteAttributeHandler(IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<DeleteAttribute>
{
    public async Task<Result> Handle(DeleteAttribute command, CancellationToken cancellationToken)
    {
        var productAttribute = await productAttributeRepository.GetByIdAsync(command.Id, cancellationToken);

        if (productAttribute == null)
            return Result.Fail(new NotFoundError($"ProductAttribute with id '{command.Id}' not found"));

        await productAttributeRepository.RemoveAsync(productAttribute, cancellationToken);
        return Result.Ok();
    }
}
