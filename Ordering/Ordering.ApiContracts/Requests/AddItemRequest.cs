namespace Ordering.ApiContracts.Requests;

public class AddItemRequest
{
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
}