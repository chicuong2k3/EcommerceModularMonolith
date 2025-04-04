using Catalog.ApiContracts.Requests;
using Catalog.ApiContracts.Responses;

namespace AdminDashboard.Client.Store.ProductAttributes;

public class ProductAttributeService : IProductAttributeService
{
    private List<ProductAttributeResponse> productAttributes = new()
    {
        new ProductAttributeResponse { Id = Guid.NewGuid(), Name = "Color" },
        new ProductAttributeResponse { Id = Guid.NewGuid(), Name = "Size" },
        new ProductAttributeResponse { Id = Guid.NewGuid(), Name = "Weight" }
    };

    public Task<IEnumerable<ProductAttributeResponse>> GetProductAttributesAsync()
    {
        return Task.FromResult(productAttributes.AsEnumerable());
    }

    public Task<ProductAttributeResponse> CreateProductAttributeAsync(CreateProductAttributeRequest request)
    {
        var newAttribute = new ProductAttributeResponse { Id = Guid.NewGuid(), Name = request.Name };
        productAttributes.Add(newAttribute);
        return Task.FromResult(newAttribute);
    }

    public Task<ProductAttributeResponse> UpdateProductAttributeAsync(Guid attributeId, UpdateProductAttributeRequest request)
    {
        var attribute = productAttributes.FirstOrDefault(pa => pa.Id == attributeId);
        if (attribute != null)
        {
            attribute.Name = request.NewName;
        }
        return Task.FromResult(attribute);
    }

    public Task DeleteProductAttributeAsync(Guid attributeId)
    {
        productAttributes.RemoveAll(pa => pa.Id == attributeId);
        return Task.CompletedTask;
    }
}
