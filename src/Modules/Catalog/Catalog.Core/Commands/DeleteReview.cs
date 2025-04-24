//namespace Catalog.Application.Products.Commands;

//public record DeleteReview(Guid ReviewId) : ICommand;

//internal class DeleteReviewHandler(IReviewRepository reviewRepository)
//    : ICommandHandler<DeleteReview>
//{
//    public async Task<Result> Handle(DeleteReview command, CancellationToken cancellationToken)
//    {
//        var review = await reviewRepository.GetByIdAsync(command.ReviewId, cancellationToken);
//        if (review == null)
//        {
//            return Result.Fail(new NotFoundError($"The review with id '{command.ReviewId}' not found"));
//        }

//        await reviewRepository.RemoveAsync(review, cancellationToken);
//        return Result.Ok();
//    }
//}
