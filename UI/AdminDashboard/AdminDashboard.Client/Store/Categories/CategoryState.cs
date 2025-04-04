using Catalog.ApiContracts.Responses;
using Fluxor;

namespace AdminDashboard.Client.Store.Categories;

[FeatureState]
public record CategoryState(
    bool IsLoading,
    string SearchQuery,
    bool IsCreating,
    bool IsUpdating,
    bool IsDeleting,
    IEnumerable<CategoryResponse> Categories)
{
    private CategoryState()
        : this(false, string.Empty, false, false, false, Enumerable.Empty<CategoryResponse>())
    {
    }
}