using System.Net.Http.Json;
using Webapp.Shared.Models;

namespace Webapp.Shared.Services;

public class ResponseHandler
{
    public async Task<Response<T>> HandleResponse<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return Response<T>.Failure(content ?? new ProblemDetails
            {
                Status = (int)response.StatusCode,
                Title = response.ReasonPhrase ?? "Unknown error",
                Detail = "No additional details available."
            });
        }

        var result = await response.Content.ReadFromJsonAsync<T>();
        if (result == null)
        {
            return Response<T>.Failure(new ProblemDetails
            {
                Status = (int)response.StatusCode,
                Title = response.ReasonPhrase ?? "Unknown error",
                Detail = "No additional details available."
            });
        }

        return Response<T>.Success(result);
    }


    public async Task<Response> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return Response.Failure(content ?? new ProblemDetails
            {
                Status = (int)response.StatusCode,
                Title = response.ReasonPhrase ?? "Unknown error",
                Detail = "No additional details available."
            });
        }

        return Response.Success();
    }

}