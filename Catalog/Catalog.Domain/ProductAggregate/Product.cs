using Catalog.Domain.ProductAggregate.Events;

namespace Catalog.Domain.ProductAggregate;

public sealed class Product : AggregateRoot
{
    private Product()
    {

    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid? CategoryId { get; private set; }

    private List<ProductVariant> variants = [];
    public IReadOnlyCollection<ProductVariant> Variants => variants.AsReadOnly();

    private List<Review> reviews = [];
    public IReadOnlyCollection<Review> Reviews => reviews.AsReadOnly();

    private Product(string name,
        string? description,
        Guid? categoryId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CategoryId = categoryId;
    }

    public static Result<Product> Create(
        string name,
        string? description,
        Guid? categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail(new ValidationError("Name is required."));

        var product = new Product(name, description, categoryId);

        product.Raise(new ProductCreated(product.Id, product.Name));

        return Result.Ok(product);
    }

    public void AddVariant(ProductVariant variant, Dictionary<string, string> attributes)
    {
        variants.Add(variant);
        Raise(new ProductVariantAdded(
            Id,
            Name,
            variant.Id,
            variant.OriginalPrice,
            variant.SalePrice?.Amount,
            variant.Quantity,
            variant.Image?.Url,
            attributes));
    }

    public Result RemoveVariant(string sku)
    {
        var variant = variants.FirstOrDefault(v => v.Sku == sku);
        if (variant == null)
            return Result.Fail(new NotFoundError($"Variant with SKU '{sku}' not found."));

        variants.Remove(variant);
        return Result.Ok();
    }

    public Result UpdateQuantity(Guid variantId, int newQuantity)
    {
        if (newQuantity < 0)
        {
            return Result.Fail(new ValidationError($"The new quantity cannot be negative"));
        }

        var variant = variants.Where(v => v.Id == variantId).FirstOrDefault();

        if (variant == null)
        {
            return Result.Fail(new NotFoundError($"Variant with id '{variantId}' not found"));
        }

        var oldQuantity = variant.Quantity;

        variant.UpdateVariantQuantity(newQuantity);

        if (oldQuantity != newQuantity)
        {
            Raise(new ProductVariantQuantityUpdated(Id, variant.Id, newQuantity));
        }

        return Result.Ok();
    }
}
