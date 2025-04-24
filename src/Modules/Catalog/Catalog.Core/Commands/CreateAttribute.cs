using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using FluentResults;
using Shared.Abstractions.Application;

namespace Catalog.Core.Commands;

public record CreateAttribute(string Name) : ICommand<ProductAttribute>;

internal class CreateAttributeHandler(IProductAttributeRepository productAttributeRepository)
    : ICommandHandler<CreateAttribute, ProductAttribute>
{
    public async Task<Result<ProductAttribute>> Handle(CreateAttribute command, CancellationToken cancellationToken)
    {
        var result = ProductAttribute.Create(command.Name);
        if (result.IsFailed)
            return result;

        var productAttribute = result.Value;
        await productAttributeRepository.AddAsync(productAttribute, cancellationToken);
        return result;
    }
}
