namespace Catalog.Contracts;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<ProductVariantDto> Variants { get; set; } = new();
}
