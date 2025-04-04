namespace Catalog.Domain.ProductAttributeAggregate;

public class ProductAttribute : AggregateRoot
{
    public string Name { get; private set; }

    private ProductAttribute()
    {
    }

    private ProductAttribute(string name)
    {
        Id = Guid.NewGuid();
        Name = name.ToLower();
    }

    public static Result<ProductAttribute> Create(string name)
    {
        if (string.IsNullOrEmpty(name))
            return Result.Fail(new ValidationError("Attribute name is required."));

        return Result.Ok(new ProductAttribute(name));
    }
}
