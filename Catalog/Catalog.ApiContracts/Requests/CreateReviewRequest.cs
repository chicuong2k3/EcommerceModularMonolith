namespace Catalog.ApiContracts.Requests;

public class CreateReviewRequest
{
    public int Rating { get; set; }
    public string Content { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
}
