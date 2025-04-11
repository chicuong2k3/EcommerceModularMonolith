namespace Billing.ApiContracts;

public class PaymentCallbackRequest
{
    public Guid OrderId { get; set; }
    public string PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string ReturnUrl { get; set; }
}
