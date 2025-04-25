namespace Ecommerce.Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator mediator;

    public CategoriesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var result = await mediator.Send(new GetCategories());
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var result = await mediator.Send(new GetCategoryById(id));
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var id = Guid.NewGuid();
        var result = await mediator.Send(new CreateCategory(id, request.Name, request.ParentCategoryId));

        if (result.IsFailed)
            return result.ToActionResult();

        var getResult = await mediator.Send(new GetCategoryById(id));
        return CreatedAtAction(nameof(GetCategory), new { id }, getResult.ValueOrDefault);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var command = new UpdateCategory(id, request.NewName, request.ParentCategoryId);
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await mediator.Send(new DeleteCategory(id));
        return result.ToActionResult();
    }
}

