using Catalog.ApiContracts.Requests;
using Catalog.ApiContracts.Responses;

namespace AdminDashboard.Client.Store.ProductAttributes;

public static class ProductAttributeActions
{
    public record FetchProductAttributesAction();
    public record FetchProductAttributesSuccessAction(IEnumerable<ProductAttributeResponse> ProductAttributes);
    public record SearchProductAttributesAction(string Query);
    public record CreateProductAttributeAction(CreateProductAttributeRequest ProductAttribute);
    public record CreateProductAttributeSuccessAction(ProductAttributeResponse ProductAttribute);
    public record UpdateProductAttributeAction(Guid ProductAttributeId, UpdateProductAttributeRequest ProductAttribute);
    public record UpdateProductAttributeSuccessAction(ProductAttributeResponse ProductAttribute);
    public record DeleteProductAttributeAction(Guid ProductAttributeId);
    public record DeleteProductAttributeSuccessAction(Guid ProductAttributeId);
}
