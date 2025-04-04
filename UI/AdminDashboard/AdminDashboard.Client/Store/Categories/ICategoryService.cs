using Catalog.ApiContracts.Requests;
using Catalog.ApiContracts.Responses;

namespace AdminDashboard.Client.Store.Categories;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetCategoriesAsync();
    Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
    Task<CategoryResponse> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request);
    Task DeleteCategoryAsync(Guid categoryId);
    Task<CategoryResponse> GetParentCategoryAsync(Guid categoryId);
}
