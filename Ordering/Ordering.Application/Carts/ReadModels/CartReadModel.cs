namespace Ordering.Application.Carts.ReadModels;

public class CartReadModel
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CartItemReadModel> Items { get; set; } = new();
}
