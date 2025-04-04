using Fluxor;
using static AdminDashboard.Client.Store.ProductAttributes.ProductAttributeActions;

namespace AdminDashboard.Client.Store.ProductAttributes;

public static class ProductAttributeReducers
{
    [ReducerMethod]
    public static ProductAttributeState ReduceSearchProductAttributes(ProductAttributeState state, SearchProductAttributesAction action) =>
        state with { SearchQuery = action.Query };

    [ReducerMethod]
    public static ProductAttributeState ReduceFetchProductAttributes(ProductAttributeState state, FetchProductAttributesAction action) =>
        state with { IsLoading = true };

    [ReducerMethod]
    public static ProductAttributeState ReduceFetchProductAttributesSuccess(ProductAttributeState state, FetchProductAttributesSuccessAction action) =>
        state with { IsLoading = false, ProductAttributes = action.ProductAttributes };

    [ReducerMethod]
    public static ProductAttributeState ReduceCreateProductAttribute(ProductAttributeState state, CreateProductAttributeAction action) =>
        state with { IsCreating = true };

    [ReducerMethod]
    public static ProductAttributeState ReduceCreateProductAttributeSuccess(ProductAttributeState state, CreateProductAttributeSuccessAction action) =>
        state with
        {
            ProductAttributes = state.ProductAttributes.Append(action.ProductAttribute),
            IsCreating = false
        };

    [ReducerMethod]
    public static ProductAttributeState ReduceUpdateProductAttribute(ProductAttributeState state, UpdateProductAttributeAction action) =>
        state with { IsUpdating = true };

    [ReducerMethod]
    public static ProductAttributeState ReduceUpdateProductAttributeSuccess(ProductAttributeState state, UpdateProductAttributeSuccessAction action) =>
        state with
        {
            ProductAttributes = state.ProductAttributes.Select(pa => pa.Id == action.ProductAttribute.Id ? action.ProductAttribute : pa),
            IsUpdating = false
        };

    [ReducerMethod]
    public static ProductAttributeState ReduceDeleteProductAttribute(ProductAttributeState state, DeleteProductAttributeAction action) =>
        state with { IsDeleting = true };

    [ReducerMethod]
    public static ProductAttributeState ReduceDeleteProductAttributeSuccess(ProductAttributeState state, DeleteProductAttributeSuccessAction action) =>
        state with
        {
            ProductAttributes = state.ProductAttributes.Where(pa => pa.Id != action.ProductAttributeId),
            IsDeleting = false
        };
}
