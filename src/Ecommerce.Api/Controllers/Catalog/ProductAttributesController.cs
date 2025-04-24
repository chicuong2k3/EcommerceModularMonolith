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
        await mediator.Send(new CreateAttribute(request.Name));
        var result = await mediator.Send(new GetAttributeByName(request.Name));
        if (result.IsFailed)
            return result.ToActionResult();

        var attribute = result.Value;

        return CreatedAtAction(nameof(GetProductAttribute), new { name = attribute.Name }, attribute);
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteProductAttribute(string name)
    {
        var result = await mediator.Send(new DeleteAttribute(name));
        return result.ToActionResult();
    }
}

