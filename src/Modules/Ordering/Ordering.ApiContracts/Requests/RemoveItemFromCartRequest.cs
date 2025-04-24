namespace Ordering.ApiContracts.Requests;

public class RemoveItemFromCartRequest
{
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
}