namespace Ordering.ApiContracts.Responses;

public class CartResposne
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CartItemResponse> Items { get; set; } = new();
}
