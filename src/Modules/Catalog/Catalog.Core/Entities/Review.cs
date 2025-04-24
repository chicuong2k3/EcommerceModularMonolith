using Catalog.Core.ValueObjects;
using FluentResults;
using Shared.Abstractions.Core;

namespace Catalog.Core.Entities;

public class Review
{
    private Review() { } // For EF Core

    private Review(ReviewRating rating, string content, Guid userId, bool approved)
    {
        Id = Guid.NewGuid();
        Rating = rating;
        Content = content;
        UserId = userId;
        Approved = approved;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; protected set; }
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
