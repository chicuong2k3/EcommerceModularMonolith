using System.ComponentModel.DataAnnotations;

namespace Catalog.Requests;

public class CreateUpdateCategoryRequest
{
    [Required(ErrorMessage = "Vui lòng điền tên danh mục")]
    public string Name { get; set; }
    public Guid? ParentCategoryId { get; set; }
}