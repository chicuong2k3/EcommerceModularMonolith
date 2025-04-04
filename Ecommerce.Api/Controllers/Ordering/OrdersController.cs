using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Ordering.ApiContracts.Requests;
using Ordering.Application.Orders.Commands;
using Ordering.Application.Orders.Queries;

namespace Ecommerce.Api.Controllers.Ordering;

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
        var result = await mediator.Send(new PlaceOrder(request.CustomerId,
                                                         request.Street,
                                                         request.Ward,
                                                         request.District,
                                                         request.Province,
                                                         request.Country,
                                                         request.PaymentMethod,
                                                         request.ShippingMethod,
                                                         request.CouponCode));

        if (result.IsFailed)
            return result.ToActionResult();

        var getOrderResult = await mediator.Send(new GetOrderById(result.Value.Id));

        return CreatedAtAction(nameof(GetOrderById), new { id = result.Value.Id }, getOrderResult.ValueOrDefault);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        var result = await mediator.Send(new CancelOrder(id));
        return result.ToActionResult();
    }
}
