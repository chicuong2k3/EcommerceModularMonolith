using Catalog.Core.ValueObjects;
using Catalog.Requests;

namespace Ecommerce.Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator mediator;
    private readonly ILogger<ProductsController> logger;

    public ProductsController(
        IMediator mediator,
        ILogger<ProductsController> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var result = await mediator.Send(new GetProductById(id));
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> SearchProducts([FromQuery] SearchProductsRequest request)
    {
        var result = await mediator.Send(new SearchProducts(
            request.PageSize,
            request.PageNumber,
            request.CategoryId,
            request.SearchText,
            request.SortBy,
            request.MinPrice,
            request.MaxPrice,
            request.Attributes?.Select(a => new AttributeValue(a.Name, a.Value)).ToList()));

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var id = Guid.NewGuid();
        var result = await mediator.Send(new CreateProduct(id, request.Name, request.Description, request.CategoryId));
        if (result.IsFailed)
            return result.ToActionResult();

        var getResult = await mediator.Send(new GetProductById(id));
        return CreatedAtAction(nameof(GetProduct), new { id }, getResult.ValueOrDefault);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        var command = new UpdateProductInfo(id, request.Name, request.Description, request.CategoryId);
        var result = await mediator.Send(command);
        if (result.IsFailed)
            return result.ToActionResult();
        var getResult = await mediator.Send(new GetProductById(id));
        return Ok(getResult.ValueOrDefault);
    }

    [HttpPost("{id}/variants")]
    public async Task<IActionResult> AddVariant(Guid id, [FromBody] AddVariantRequest request)
    {
        var command = new AddVariantForProduct(
            id,
            request.OriginalPrice,
            request.Quantity,
            request.ImageData,
            request.ImageAltText,
            request.Attributes.Select(a => new AttributeValue(a.Name, a.Value)).ToList(),
            request.SaleStartDate,
            request.SaleEndDate,
            request.SalePrice);

        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await mediator.Send(new DeleteProduct(id));
        return result.ToActionResult();
    }

    [HttpPut("{id}/variants/{variantId}")]
    public async Task<IActionResult> UpdateVariantQuantity(Guid id, Guid variantId, [FromBody] UpdateVariantRequest request)
    {
        var command = new UpdateVariant(
            id,
            variantId,
            request.Quantity,
            request.OriginalPrice,
            request.SalePrice,
            request.SaleStartDate,
            request.SaleEndDate,
            request.ImageData,
            request.ImageAltText);
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
}

