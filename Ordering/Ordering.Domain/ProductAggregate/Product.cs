namespace Ordering.Domain.ProductAggregate;

public sealed class Product : AggregateRoot
{
    private Product()
    {

    }

    public string Name { get; private set; }
    private List<ProductVariant> variants = [];
    public IReadOnlyCollection<ProductVariant> Variants => variants.AsReadOnly();

    public Product(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public void AddVariant(ProductVariant variant)
    {
        variants.Add(variant);
    }
}