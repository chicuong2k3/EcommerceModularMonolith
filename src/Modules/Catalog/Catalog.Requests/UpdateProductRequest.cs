using System.ComponentModel.DataAnnotations;

namespace Catalog.Requests;

public class UpdateProductRequest
{
    [Required(ErrorMessage = "Vui lòng điền tên sản phẩm")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
}
