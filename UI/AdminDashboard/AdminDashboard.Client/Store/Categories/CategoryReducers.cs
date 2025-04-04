using Fluxor;
using static AdminDashboard.Client.Store.Categories.CategoryActions;

namespace AdminDashboard.Client.Store.Categories;

public static class CategoryReducers
{
    [ReducerMethod]
    public static CategoryState ReduceSearchCategories(CategoryState state, SearchCategoriesAction action) =>
        state with { SearchQuery = action.Query };

    [ReducerMethod]
    public static CategoryState ReduceFetchCategories(CategoryState state, FetchCategoriesAction action) =>
        state with { IsLoading = true };

    [ReducerMethod]
    public static CategoryState ReduceFetchCategoriesSuccess(CategoryState state, FetchCategoriesSuccessAction action) =>
        state with { IsLoading = false, Categories = action.Categories };

    [ReducerMethod]
    public static CategoryState ReduceCreateCategory(CategoryState state, CreateCategoryAction action) =>
        state with { IsCreating = true };

    [ReducerMethod]
    public static CategoryState ReduceCreateCategorySuccess(CategoryState state, CreateCategorySuccessAction action) =>
        state with
        {
            Categories = state.Categories.Append(action.Category),
            IsCreating = false
        };

    [ReducerMethod]
    public static CategoryState ReduceUpdateCategory(CategoryState state, UpdateCategoryAction action) =>
        state with { IsUpdating = true };

    [ReducerMethod]
    public static CategoryState ReduceUpdateCategorySuccess(CategoryState state, UpdateCategorySuccessAction action) =>
        state with
        {
            Categories = state.Categories.Select(c => c.Id == action.Category.Id ? action.Category : c),
            IsUpdating = false
        };

    [ReducerMethod]
    public static CategoryState ReduceDeleteCategory(CategoryState state, DeleteCategoryAction action) =>
        state with { IsDeleting = true };

    [ReducerMethod]
    public static CategoryState ReduceDeleteCategorySuccess(CategoryState state, DeleteCategorySuccessAction action) =>
        state with
        {
            Categories = state.Categories.Where(c => c.Id != action.CategoryId),
            IsDeleting = false
        };
}