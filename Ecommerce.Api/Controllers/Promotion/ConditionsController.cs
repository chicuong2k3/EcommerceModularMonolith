using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Promotion.ApiContracts.Requests;
using Promotion.Application.Conditions.Commands;
using Promotion.Application.Conditions.Queries;

namespace Ecommerce.Api.Controllers.Promotion;

[ApiController]
[Route("api/[controller]")]
public class ConditionsController : Controller
{
    private readonly IMediator mediator;

    public ConditionsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetConditions()
    {
        var result = await mediator.Send(new GetConditions());
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCondition(Guid id)
    {
        var result = await mediator.Send(new GetConditionById(id));
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCondition([FromBody] CreateConditionRequest request)
    {
        var result = await mediator.Send(new CreateCondition(
            request.Name,
            request.ConditionType,
            request.Value));

        if (result.IsFailed)
        {
            return result.ToActionResult();
        }

        var condition = result.Value;

        return CreatedAtAction(nameof(GetCondition), new { id = condition.Id }, condition);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteCondition(id));
        return result.ToActionResult();
    }
}
