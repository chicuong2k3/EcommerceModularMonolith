using Catalog.ReadModels;
using Catalog.Requests;
using System.Net.Http.Json;
using Webapp.Models;

namespace Webapp.Services;

public class CategoryService
{
    private readonly HttpClient _httpClient;
    private readonly ResponseHandler _responseHandler;

    public CategoryService(HttpClient httpClient, ResponseHandler responseHandler)
    {
        _httpClient = httpClient;
        _responseHandler = responseHandler;
    }

    public async Task<Response<List<CategoryReadModel>>> GetCategoriesAsync()
    {
        var response = await _httpClient.GetAsync("api/categories");
        return await _responseHandler.HandleResponse<List<CategoryReadModel>>(response);
    }

    public async Task<Response<CategoryReadModel>> GetCategoryAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/categories/{id}");
        return await _responseHandler.HandleResponse<CategoryReadModel>(response);
    }

    public async Task<Response<CategoryReadModel>> CreateCategoryAsync(CreateUpdateCategoryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/categories", request);
        return await _responseHandler.HandleResponse<CategoryReadModel>(response);
    }

    public async Task<Response<CategoryReadModel>> UpdateCategoryAsync(Guid id, CreateUpdateCategoryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", request);
        return await _responseHandler.HandleResponse<CategoryReadModel>(response);
    }

    public async Task<Response> DeleteCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/categories/{id}");
        return await _responseHandler.HandleResponse(response);
    }
}