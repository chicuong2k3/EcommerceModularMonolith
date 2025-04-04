//using Catalog.ApiContracts.Requests;
//using Catalog.Application.Categories.Queries;
//using Catalog.Application.Reviews.Commands;
//using FluentResults.Extensions.AspNetCore;
//using Microsoft.AspNetCore.Mvc;

//namespace Ecommerce.Api.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class ReviewsController : Controller
//{
//    private readonly IMediator mediator;

//    public ReviewsController(IMediator mediator)
//    {
//        this.mediator = mediator;
//    }

//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetReview(Guid id)
//    {
//        var result = await mediator.Send(new GetCategoryById(id));
//        return result.ToActionResult();
//    }

//    [HttpPost]
//    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
//    {
//        var result = await mediator.Send(new CreateReview(request.ProductId, request.UserId, request.Content, request.Rating));

//        if (result.IsFailed)
//            return result.ToActionResult();

//        var review = result.Value;
//        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
//    }

//    [HttpDelete("{id}")]
//    public async Task<IActionResult> DeleteReview(Guid id)
//    {
//        var result = await mediator.Send(new DeleteReview(id));
//        return result.ToActionResult();
//    }
//}
