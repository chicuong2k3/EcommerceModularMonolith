namespace Ordering.ApiContracts.Responses;

public class CartItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal SalePrice { get; set; }
    public int Quantity { get; set; }
}
