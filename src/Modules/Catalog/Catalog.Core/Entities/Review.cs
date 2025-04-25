using Catalog.Core.ValueObjects;
using FluentResults;
using Shared.Abstractions.Core;

namespace Catalog.Core.Entities;

public class Review
{
    private Review() { } // For EF Core

    private Review(Guid id, ReviewRating rating, string content, Guid userId, bool approved)
    {
        Id = id;
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

    public static Result<Review> Create(Guid id, ReviewRating rating, string content, Guid userId)
    {
        if (id == Guid.Empty)
            return Result.Fail(new ValidationError("Id is required."));
        if (string.IsNullOrWhiteSpace(content))
        {
            return Result.Fail(new ValidationError("Content cannot be empty."));
        }

        return Result.Ok(new Review(id, rating, content, userId, false));
    }

    public void Approve()
    {
        Approved = true;
    }
}
