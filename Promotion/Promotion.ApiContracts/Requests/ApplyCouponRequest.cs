namespace Promotion.ApiContracts.Requests;

public class ApplyCouponRequest
{
    public Guid OrderId { get; set; }
    public decimal Subtotal { get; set; }
}
