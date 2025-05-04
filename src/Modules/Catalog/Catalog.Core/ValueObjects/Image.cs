using FluentResults;
using Shared.Abstractions.Core;

namespace Catalog.Core.ValueObjects;

public record Image
{
    public string Base64Data { get; }
    public string? AltText { get; }

    private Image()
    {

    }

    private Image(string base64Data, string? altText)
    {
        Base64Data = base64Data;
        AltText = altText;
    }

    public static Result<Image> Create(string base64Data, string? altText)
    {
        if (string.IsNullOrWhiteSpace(base64Data))
            return Result.Fail(new ValidationError("Image Base64StringData is required."));

        if (!IsValidBase64(base64Data))
            return Result.Fail(new ValidationError("Invalid Base64 image data."));

        return Result.Ok(new Image(base64Data, altText));
    }

    private static bool IsValidBase64(string base64String)
    {
        try
        {
            Convert.FromBase64String(base64String);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
