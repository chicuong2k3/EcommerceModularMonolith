namespace Catalog.Domain.ProductAggregate;

public record Image
{
    public string Url { get; }
    public string? AltText { get; }

    private Image()
    {

    }

    private Image(string url, string? altText)
    {
        Url = url;
        AltText = altText;
    }

    public static Result<Image> Create(string url, string? altText)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Result.Fail(new ValidationError("Image URL is required."));

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            return Result.Fail(new ValidationError("Invalid image URL."));

        return Result.Ok(new Image(url, altText));
    }
}
