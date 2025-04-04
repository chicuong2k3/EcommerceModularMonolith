namespace Catalog.Domain.ProductAggregate;

public class Review : Entity
{
    private Review() { } // For EF Core

    private Review(ReviewRating rating, string content, Guid userId, bool approved)
    {
        Rating = rating;
        Content = content;
        UserId = userId;
        Approved = approved;
        CreatedAt = DateTime.UtcNow;
    }

    public ReviewRating Rating { get; set; }
    public string Content { get; set; }
    public Guid UserId { get; set; }
    public bool Approved { get; set; }
    public DateTime CreatedAt { get; set; }

    public static Result<Review> Create(ReviewRating rating, string content, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Result.Fail(new ValidationError("Content cannot be empty."));
        }

        return Result.Ok(new Review(rating, content, userId, false));
    }

    public void Approve()
    {
        Approved = true;
    }
}
