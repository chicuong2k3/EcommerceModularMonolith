using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;

namespace Catalog.Core.Commands;

public record CreateAttribute(Guid Id, string Name) : ICommand;

internal class CreateAttributeHandler(IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<CreateAttribute>
{
    public async Task<Result> Handle(CreateAttribute command, CancellationToken cancellationToken)
    {
        var result = ProductAttribute.Create(command.Id, command.Name);
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        var productAttribute = result.Value;
        await productAttributeRepository.AddAsync(productAttribute, cancellationToken);
        return Result.Ok();
    }
}
