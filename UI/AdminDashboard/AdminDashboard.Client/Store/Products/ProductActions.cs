using Catalog.ApiContracts.Requests;
using Catalog.ApiContracts.Responses;

namespace AdminDashboard.Client.Store.Products;

public static class ProductActions
{
    public record FetchProductsAction();
    public record FetchProductsSuccessAction(IEnumerable<ProductResponse> Products);
    public record SearchProductsAction(string Query);
    public record CreateProductAction(CreateProductRequest Product);
    public record CreateProductSuccessAction(ProductResponse Product);
    public record UpdateProductAction(Guid ProductId, UpdateProductRequest Product);
    public record UpdateProductSuccessAction(ProductResponse Product);
    public record DeleteProductAction(Guid ProductId);
    public record DeleteProductSuccessAction(Guid ProductId);
}

