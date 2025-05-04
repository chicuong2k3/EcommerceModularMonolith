using Catalog.ReadModels;
using Catalog.Requests;
using System.Net.Http.Json;
using System.Web;
using Webapp.Shared.Models;

namespace Webapp.Shared.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;
    private readonly ResponseHandler _responseHandler;
    public ProductService(IHttpClientFactory httpClientFactory, ResponseHandler responseHandler)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _responseHandler = responseHandler;
    }

    public async Task<Response<PaginationResult<ProductReadModel>>> GetProductsAsync(SearchProductsRequest request)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["pageNumber"] = request.PageNumber.ToString();
        query["pageSize"] = request.PageSize.ToString();

        if (request.CategoryId.HasValue)
            query["categoryId"] = request.CategoryId.ToString();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
            query["searchText"] = request.SearchText;

        if (!string.IsNullOrWhiteSpace(request.SortBy))
            query["sortBy"] = request.SortBy;

        if (request.MinPrice.HasValue)
            query["minPrice"] = request.MinPrice.ToString();

        if (request.MaxPrice.HasValue)
            query["maxPrice"] = request.MaxPrice.ToString();

        if (request.Attributes != null)
        {
            foreach (var attr in request.Attributes)
            {
                query.Add("attributes", $"{attr.Name}:{attr.Value}");
            }
        }

        var queryString = query.ToString();
        var response = await _httpClient.GetAsync($"api/products?{queryString}");
        return await _responseHandler.HandleResponse<PaginationResult<ProductReadModel>>(response);
    }

    public async Task<Response<ProductReadModel>> GetProductAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/products/{id}");
        return await _responseHandler.HandleResponse<ProductReadModel>(response);
    }

    public async Task<Response<ProductReadModel>> CreateProductAsync(CreateProductRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/products", request);
        return await _responseHandler.HandleResponse<ProductReadModel>(response);
    }

    public async Task<Response> AddVariantAsync(Guid id, AddVariantRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/products/{id}/variants", request);
        return await _responseHandler.HandleResponse(response);
    }

    public async Task<Response> UpdateVariantAsync(Guid id, Guid variantId, UpdateVariantRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/products/{id}/variants/{variantId}", request);
        return await _responseHandler.HandleResponse(response);
    }

    public async Task<Response<ProductReadModel>> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", request);
        return await _responseHandler.HandleResponse<ProductReadModel>(response);
    }

    public async Task<Response> DeleteProductAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/products/{id}");
        return await _responseHandler.HandleResponse(response);
    }
}
