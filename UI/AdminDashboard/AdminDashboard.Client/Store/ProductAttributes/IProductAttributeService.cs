using Catalog.ApiContracts.Requests;
using Catalog.ApiContracts.Responses;

namespace AdminDashboard.Client.Store.ProductAttributes;

public interface IProductAttributeService
{
    Task<IEnumerable<ProductAttributeResponse>> GetProductAttributesAsync();
    Task<ProductAttributeResponse> CreateProductAttributeAsync(CreateProductAttributeRequest request);
    Task<ProductAttributeResponse> UpdateProductAttributeAsync(Guid productAttributeId, UpdateProductAttributeRequest request);
    Task DeleteProductAttributeAsync(Guid productAttributeId);
}
