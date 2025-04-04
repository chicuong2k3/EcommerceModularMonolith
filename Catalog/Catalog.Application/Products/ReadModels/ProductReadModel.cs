namespace Catalog.Application.Products.ReadModels;

public class ProductReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
    public List<ProductVariantReadModel> Variants { get; set; } = new();
}