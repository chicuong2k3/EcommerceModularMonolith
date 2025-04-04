namespace Catalog.ApiContracts.Requests;

public class UpdateCategoryRequest
{
    public string NewName { get; set; }
    public Guid? ParentCategoryId { get; set; }
}