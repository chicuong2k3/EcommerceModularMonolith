namespace Catalog.ApiContracts.Requests;

public class CreateProductRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
}