using Catalog.ApiContracts.Requests;
using Catalog.ApiContracts.Responses;

namespace AdminDashboard.Client.Store.Categories;

public static class CategoryActions
{
    public record FetchCategoriesAction();
    public record FetchCategoriesSuccessAction(IEnumerable<CategoryResponse> Categories);
    public record SearchCategoriesAction(string Query);
    public record CreateCategoryAction(CreateCategoryRequest Category);
    public record CreateCategorySuccessAction(CategoryResponse Category);
    public record UpdateCategoryAction(Guid CategoryId, UpdateCategoryRequest Category);
    public record UpdateCategorySuccessAction(CategoryResponse Category);
    public record DeleteCategoryAction(Guid CategoryId);
    public record DeleteCategorySuccessAction(Guid CategoryId);
}