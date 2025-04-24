namespace Ecommerce.Api.Controllers.Ordering;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IMediator mediator;

    public CartController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var ownerId = new Guid("F4925C17-1DF2-4293-983D-6E49847745CE");
        var result = await mediator.Send(new GetCart(ownerId));
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> AddItemToCart([FromBody] AddItemToCartRequest request)
    {
        var ownerId = new Guid("F4925C17-1DF2-4293-983D-6E49847745CE");
        var result = await mediator.Send(new AddItemToCart(ownerId, request.Items.Select(i => new AddItemDto(
            i.ProductId,
            i.ProductVariantId,
            i.Quantity)).ToList()));
        return result.ToActionResult();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveItemFromCart([FromBody] RemoveItemFromCartRequest request)
    {
        var ownerId = new Guid("F4925C17-1DF2-4293-983D-6E49847745CE");
        var result = await mediator.Send(new RemoveItemFromCart(ownerId, request.ProductVariantId, request.Quantity));
        return result.ToActionResult();
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var ownerId = new Guid("F4925C17-1DF2-4293-983D-6E49847745CE");
        var result = await mediator.Send(new ClearCart(ownerId));
        return result.ToActionResult();
    }
}

