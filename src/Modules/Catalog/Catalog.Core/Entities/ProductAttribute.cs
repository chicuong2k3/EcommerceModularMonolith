using FluentResults;
using Shared.Abstractions.Core;

namespace Catalog.Core.Entities;

public class ProductAttribute : AggregateRoot
{
    public string Name { get; private set; }

    private ProductAttribute()
    {
    }

    private ProductAttribute(Guid id, string name)
    {
        Id = id;
        Name = name.ToLower();
    }

    public static Result<ProductAttribute> Create(Guid id, string name)
    {
        if (id == Guid.Empty)
            return Result.Fail(new ValidationError("Id is required."));
        if (string.IsNullOrEmpty(name))
            return Result.Fail(new ValidationError("Attribute name is required."));

        return Result.Ok(new ProductAttribute(id, name));
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return Result.Fail(new ValidationError("Attribute name is required."));
        Name = name.ToLower();
        return Result.Ok();
    }
}
