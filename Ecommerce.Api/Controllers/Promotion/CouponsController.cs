using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Promotion.ApiContracts.Requests;
using Promotion.Application.Coupons.Commands;
using Promotion.Application.Coupons.Queries;

namespace Ecommerce.Api.Controllers.Promotion;

[ApiController]
[Route("api/[controller]")]
public class CouponsController : Controller
{
    private readonly IMediator mediator;

    public CouponsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetCoupons());
        return result.ToActionResult();
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetCoupon(string code)
    {
        var result = await mediator.Send(new GetCouponByCode(code));
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCouponRequest request)
    {
        var result = await mediator.Send(new CreateCoupon(
            request.Code,
            request.DiscountType,
            request.DiscountValue,
            request.ExpiryDate,
            request.UsageLimit,
            request.Description,
            request.ConditionIds));

        if (result.IsFailed)
        {
            return result.ToActionResult();
        }

        var coupon = result.Value;

        return CreatedAtAction(nameof(GetCoupon), new { code = coupon.Code }, coupon);
    }


    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        var result = await mediator.Send(new DeleteCoupon(code));
        return result.ToActionResult();
    }

    [HttpPost("{code}/apply")]
    public async Task<IActionResult> ApplyCoupon(string code, [FromBody] ApplyCouponRequest request)
    {
        var moneyCreationResult = Money.FromDecimal(request.Subtotal);

        if (moneyCreationResult.IsFailed)
            return moneyCreationResult.ToActionResult();

        var discountAmount = await mediator.Send(new ApplyCoupon(code,
                                                           request.OrderId,
                                                           moneyCreationResult.Value));

        var result = await mediator.Send(new GetCouponByCode(code));
        return result.ToActionResult();
    }
}
