namespace Ecommerce.Api.Controllers.Catalog;

[ApiController]
[Route("api/product-attributes")]
public class ProductAttributesController : ControllerBase
{
    private readonly IMediator mediator;

    public ProductAttributesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetProductAttribute(string name)
    {
        var result = await mediator.Send(new GetAttributeByName(name));
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProductAttributes()
    {
        var result = await mediator.Send(new GetAttributes());
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAttribute([FromBody] CreateProductAttributeRequest request)
    {
        var result = await mediator.Send(new CreateAttribute(Guid.NewGuid(), request.Name));
        if (result.IsFailed)
            return result.ToActionResult();

        var getResult = await mediator.Send(new GetAttributeByName(request.Name));
        return CreatedAtAction(nameof(GetProductAttribute), new { name = request.Name }, getResult.ValueOrDefault);
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteProductAttribute(string name)
    {
        var result = await mediator.Send(new DeleteAttribute(name));
        return result.ToActionResult();
    }
}

