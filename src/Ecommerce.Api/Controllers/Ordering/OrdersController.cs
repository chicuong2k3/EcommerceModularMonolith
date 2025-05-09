﻿namespace Ecommerce.Api.Controllers.Ordering;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : Controller
{
    private readonly IMediator mediator;

    public OrdersController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var result = await mediator.Send(new GetOrderById(id));
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        var id = Guid.NewGuid();
        var result = await mediator.Send(new PlaceOrder(id,
            request.CustomerId,
                                                         request.Street,
                                                         request.Ward,
                                                         request.District,
                                                         request.Province,
                                                         request.Country,
                                                         request.PhoneNumber,
                                                         request.PaymentMethod,
                                                         request.ShippingMethod));

        if (result.IsFailed)
            return result.ToActionResult();

        var getOrderResult = await mediator.Send(new GetOrderById(id));
        return CreatedAtAction(nameof(GetOrderById), new { id = id }, getOrderResult.ValueOrDefault);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        var result = await mediator.Send(new CancelOrder(id));
        return result.ToActionResult();
    }
}
