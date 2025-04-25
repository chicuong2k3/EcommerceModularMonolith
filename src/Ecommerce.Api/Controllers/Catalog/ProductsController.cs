using Catalog.Core.ValueObjects;

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
        var result = await mediator.Send(new CreateProduct(request.Name, request.Description, request.CategoryId));
        if (result.IsFailed)
            return result.ToActionResult();

        var product = result.Value;
        var getProductResult = await mediator.Send(new GetProductById(product.Id));

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, getProductResult.ValueOrDefault);
    }

    [HttpPost("{id}/variants")]
    public async Task<IActionResult> AddVariant(Guid id, [FromBody] AddVariantRequest request)
    {
        var command = new AddVariantForProduct(
            id,
            request.OriginalPrice,
            request.Quantity,
            request.ImageUrl,
            request.ImageAltText,
            request.Attributes.Select(a => new AttributeValue(a.Name, a.Value)).ToList(),
            request.DiscountStartAt,
            request.DiscountEndAt,
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

    [HttpPost("{id}/variants/{variantId}")]
    public async Task<IActionResult> UpdateVariantQuantity(Guid id, Guid variantId, [FromBody] int newQuantity)
    {
        var command = new UpdateQuantity(id, variantId, newQuantity);
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
}

