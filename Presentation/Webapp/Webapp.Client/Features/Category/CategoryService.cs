using Catalog.ApiContracts.Requests;
using Catalog.ApiContracts.Responses;

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
        return new List<CategoryResponse> { new CategoryResponse() };
    }

    public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        return new() { Id = Guid.NewGuid(), Name = request.Name, SubCategories = [] };
    }

    public async Task<CategoryResponse> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        return new() { Id = id, Name = request.NewName, SubCategories = [] };
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        // Simulate deletion
        await Task.CompletedTask;
    }
}
