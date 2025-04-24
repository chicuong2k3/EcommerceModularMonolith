using Catalog.ApiContracts.Requests;
using Catalog.ApiContracts.Responses;
using System.Net.Http.Json;

namespace Webapp.Client.Features.Category;

public class CategoryService
{
    private readonly HttpClient httpClient;
    public CategoryService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/categories", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<CategoryResponse>>() ?? [];
        }

        return [];
    }

    public async Task<CategoryResponse?> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/categories", request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CategoryResponse>();
        }

        return null;
    }

    public async Task<CategoryResponse?> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"api/categories/{id}", request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CategoryResponse>();
        }
        return null;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/categories/{id}", cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
