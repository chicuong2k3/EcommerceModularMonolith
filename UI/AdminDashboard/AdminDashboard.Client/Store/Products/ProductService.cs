using Catalog.ApiContracts.Requests;
using Catalog.ApiContracts.Responses;

namespace AdminDashboard.Client.Store.Products;

public class ProductService : IProductService
{
    private List<ProductResponse> products = new()
    {
        new ProductResponse { Id = Guid.NewGuid(), Name = "Laptop" },
        new ProductResponse { Id = Guid.NewGuid(), Name = "Smartphone" },
        new ProductResponse { Id = Guid.NewGuid(), Name = "Tablet" }
    };

    public Task<IEnumerable<ProductResponse>> GetProductsAsync()
    {
        return Task.FromResult(products.AsEnumerable());
    }

    public Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        var newProduct = new ProductResponse { Id = Guid.NewGuid(), Name = request.Name };
        products.Add(newProduct);
        return Task.FromResult(newProduct);
    }

    public Task<ProductResponse> UpdateProductAsync(Guid productId, UpdateProductRequest request)
    {
        var product = products.FirstOrDefault(p => p.Id == productId);
        if (product != null)
        {
            //product.Name = request.NewName;
        }
        return Task.FromResult(product);
    }

    public Task DeleteProductAsync(Guid productId)
    {
        products.RemoveAll(p => p.Id == productId);
        return Task.CompletedTask;
    }
}
