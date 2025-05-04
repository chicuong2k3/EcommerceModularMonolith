using System.ComponentModel.DataAnnotations;

namespace Catalog.Requests;

public class ProductAttributeRequest
{
    public string Name { get; set; }
    [Required(ErrorMessage = "Vui lòng điền giá trị thuộc tính sản phẩm")]
    public string Value { get; set; }
}