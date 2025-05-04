using System.ComponentModel.DataAnnotations;

namespace Catalog.Requests;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Vui lòng điền tên sản phẩm")]
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
}