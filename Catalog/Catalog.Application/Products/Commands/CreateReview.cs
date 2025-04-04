//using Catalog.Application.Products.ReadModels;

//namespace Catalog.Application.Products.Commands;

//public record CreateReview(
//    Guid ProductId,
//    Guid UserId,
//    string Content,
//    int Rating) : ICommand<ReviewReadModel>;

//internal class CreateReviewHandler(IReviewRepository reviewRepository)
//    : ICommandHandler<CreateReview, ReviewReadModel>
//{
//    public async Task<Result<ReviewReadModel>> Handle(CreateReview command, CancellationToken cancellationToken)
//    {
//        var ratingCreationResult = ReviewRating.Create(command.Rating);

//        if (ratingCreationResult.IsFailed)
//        {
//            return Result.Fail(ratingCreationResult.Errors);
//        }

//        var reviewCreationResult = Review.Create(
//            ratingCreationResult.Value,
//            command.Content,
//            command.ProductId,
//            command.UserId);

//        if (reviewCreationResult.IsFailed)
//        {
//            return Result.Fail(reviewCreationResult.Errors);
//        }

//        await reviewRepository.AddAsync(reviewCreationResult.Value, cancellationToken);
//        return Result.Ok(new ReviewReadModel()
//        {
//            Id = reviewCreationResult.Value.Id,
//            Rating = reviewCreationResult.Value.Rating,
//            Content = reviewCreationResult.Value.Content,
//            ProductId = reviewCreationResult.Value.ProductId,
//            UserId = reviewCreationResult.Value.UserId,
//            Approved = reviewCreationResult.Value.Approved,
//            CreatedAt = reviewCreationResult.Value.CreatedAt
//        });
//    }
//}