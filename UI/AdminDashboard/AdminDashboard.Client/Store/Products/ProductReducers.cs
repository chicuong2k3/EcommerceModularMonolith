using Fluxor;
using static AdminDashboard.Client.Store.Products.ProductActions;

namespace AdminDashboard.Client.Store.Products;

public static class ProductReducers
{
    [ReducerMethod]
    public static ProductState ReduceSearchProducts(ProductState state, SearchProductsAction action) =>
        state with { SearchQuery = action.Query };

    [ReducerMethod]
    public static ProductState ReduceFetchProducts(ProductState state, FetchProductsAction action) =>
        state with { IsLoading = true };

    [ReducerMethod]
    public static ProductState ReduceFetchProductsSuccess(ProductState state, FetchProductsSuccessAction action) =>
        state with { IsLoading = false, Products = action.Products };

    [ReducerMethod]
    public static ProductState ReduceCreateProduct(ProductState state, CreateProductAction action) =>
        state with { IsCreating = true };

    [ReducerMethod]
    public static ProductState ReduceCreateProductSuccess(ProductState state, CreateProductSuccessAction action) =>
        state with
        {
            Products = state.Products.Append(action.Product),
            IsCreating = false
        };

    [ReducerMethod]
    public static ProductState ReduceUpdateProduct(ProductState state, UpdateProductAction action) =>
        state with { IsUpdating = true };

    [ReducerMethod]
    public static ProductState ReduceUpdateProductSuccess(ProductState state, UpdateProductSuccessAction action) =>
        state with
        {
            Products = state.Products.Select(p => p.Id == action.Product.Id ? action.Product : p),
            IsUpdating = false
        };

    [ReducerMethod]
    public static ProductState ReduceDeleteProduct(ProductState state, DeleteProductAction action) =>
        state with { IsDeleting = true };

    [ReducerMethod]
    public static ProductState ReduceDeleteProductSuccess(ProductState state, DeleteProductSuccessAction action) =>
        state with
        {
            Products = state.Products.Where(p => p.Id != action.ProductId),
            IsDeleting = false
        };
}
