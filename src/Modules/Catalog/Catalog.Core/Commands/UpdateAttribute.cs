using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Catalog.Core.Commands;

public record UpdateAttribute(Guid Id, string NewName) : ICommand;

internal class UpdateAttributeHandler(IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<UpdateAttribute>
{
    public async Task<Result> Handle(UpdateAttribute command, CancellationToken cancellationToken)
    {
        var productAttribute = await productAttributeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (productAttribute == null)
            return Result.Fail(new NotFoundError($"Product attribute with id '{command.Id}' not found"));

        var existingAttribute = await productAttributeRepository.GetByNameAsync(command.NewName, cancellationToken);
        if (existingAttribute != null && existingAttribute.Id != command.Id)
            return Result.Fail(new ConflictError($"Product attribute with name '{command.NewName}' already exists."));

        var result = productAttribute.UpdateName(command.NewName);
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await productAttributeRepository.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}