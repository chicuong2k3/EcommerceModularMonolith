using Catalog.ReadModels;
using Catalog.Requests;
using System.Net.Http.Json;
using Webapp.Models;

namespace Webapp.Services;

public class ProductAttributeService
{
    private readonly HttpClient _httpClient;
    private readonly ResponseHandler _responseHandler;

    public ProductAttributeService(HttpClient httpClient, ResponseHandler responseHandler)
    {
        _httpClient = httpClient;
        _responseHandler = responseHandler;
    }

    public async Task<Response<List<AttributeReadModel>>> GetProductAttributesAsync()
    {
        var response = await _httpClient.GetAsync("api/product-attributes");
        return await _responseHandler.HandleResponse<List<AttributeReadModel>>(response);
    }

    public async Task<Response<AttributeReadModel>> GetProductAttributeAsync(string name)
    {
        var response = await _httpClient.GetAsync($"api/product-attributes/{name}");
        return await _responseHandler.HandleResponse<AttributeReadModel>(response);
    }

    public async Task<Response<AttributeReadModel>> CreateProductAttributeAsync(CreateUpdateProductAttributeRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/product-attributes", request);
        return await _responseHandler.HandleResponse<AttributeReadModel>(response);
    }

    public async Task<Response<AttributeReadModel>> UpdateProductAttributeAsync(Guid id, CreateUpdateProductAttributeRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/product-attributes/{id}", request);
        return await _responseHandler.HandleResponse<AttributeReadModel>(response);
    }

    public async Task<Response> DeleteProductAttributeAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/product-attributes/{id}");
        return await _responseHandler.HandleResponse(response);
    }
}