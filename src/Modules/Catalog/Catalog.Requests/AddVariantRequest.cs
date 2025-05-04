using System.ComponentModel.DataAnnotations;

namespace Catalog.Requests;

public class AddVariantRequest
{
    [Required(ErrorMessage = "Vui lòng điền giá sản phẩm")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm không hợp lệ")]
    public decimal OriginalPrice { get; set; }
    [Required(ErrorMessage = "Vui lòng điền số lượng sản phẩm")]
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng sản phẩm không hợp lệ")]
    public int Quantity { get; set; }
    public string? ImageData { get; set; }
    public string? ImageAltText { get; set; }
    public List<ProductAttributeRequest> Attributes { get; set; } = [];
    public DateTime? SaleStartDate { get; set; }
    public DateTime? SaleEndDate { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Giá khuyến mãi không hợp lệ")]
    public decimal? SalePrice { get; set; }
}