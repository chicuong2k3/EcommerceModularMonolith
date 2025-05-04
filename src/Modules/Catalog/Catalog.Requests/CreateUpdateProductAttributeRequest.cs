using System.ComponentModel.DataAnnotations;

namespace Catalog.Requests;

public class CreateUpdateProductAttributeRequest
{
    [Required(ErrorMessage = "Vui lòng điền tên thuộc tính sản phẩm")]
    public string Name { get; set; }
}