namespace Ordering.ApiContracts.Requests;

public class PlaceOrderRequest
{
    public Guid CustomerId { get; set; }
    public string? Street { get; set; }
    public string Ward { get; set; }
    public string District { get; set; }
    public string Province { get; set; }
    public string Country { get; set; }
    public string PaymentMethod { get; set; }
    public string ShippingMethod { get; set; }
    public string? CouponCode { get; set; }
    public List<Guid> CartItemIds { get; set; }
}