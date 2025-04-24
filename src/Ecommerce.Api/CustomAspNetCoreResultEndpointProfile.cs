namespace Ecommerce.Api;

public class CustomAspNetCoreResultEndpointProfile : IAspNetCoreResultEndpointProfile
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CustomAspNetCoreResultEndpointProfile(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public ActionResult TransformFailedResultToActionResult(FailedResultToActionResultTransformationContext context)
    {
        var result = context.Result;
        var requestPath = httpContextAccessor.HttpContext?.Request.Path ?? "/unknown";

        var validationErrors = result.Errors.OfType<ValidationError>();
        if (validationErrors.Any())
        {
            var errorDetails = validationErrors.Select(e => e.Message).ToList();
            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Validation Failed",
                Status = 422,
                Detail = string.Join("; ", errorDetails),
                Instance = requestPath,
                Extensions = { ["errors"] = errorDetails }
            };
            return new UnprocessableEntityObjectResult(problem);
        }

        var notFoundErrors = result.Errors.OfType<NotFoundError>();
        if (notFoundErrors.Any())
        {
            var errorDetails = notFoundErrors.Select(e => e.Message).ToList();
            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Resource Not Found",
                Status = 404,
                Detail = string.Join("; ", errorDetails),
                Instance = requestPath,
                Extensions = { ["errors"] = errorDetails }
            };
            return new NotFoundObjectResult(problem);
        }

        var errorDetailsGeneral = result.Errors.Select(e => e.Message).ToList();
        var problemGeneral = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = 400,
            Detail = string.Join("; ", errorDetailsGeneral),
            Instance = requestPath,
            Extensions = { ["errors"] = errorDetailsGeneral }
        };
        return new BadRequestObjectResult(problemGeneral);
    }

    public ActionResult TransformOkNoValueResultToActionResult(OkResultToActionResultTransformationContext<Result> context)
    {
        return new NoContentResult();
    }

    public ActionResult TransformOkValueResultToActionResult<T>(OkResultToActionResultTransformationContext<Result<T>> context)
    {
        return new OkObjectResult(context.Result.Value);
    }
}