namespace Catalog.ApiContracts.Requests;

public class CreateCategoryRequest
{
    public string Name { get; set; }
    public Guid? ParentCategoryId { get; set; }
}