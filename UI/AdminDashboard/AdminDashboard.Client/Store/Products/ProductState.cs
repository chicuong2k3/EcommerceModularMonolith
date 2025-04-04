using Catalog.ApiContracts.Responses;
using Fluxor;

namespace AdminDashboard.Client.Store.Products;

[FeatureState]
public record ProductState(bool IsLoading,
                           bool IsCreating,
                           bool IsUpdating,
                           bool IsDeleting,
                           string SearchQuery,
                           IEnumerable<ProductResponse> Products)
{
    public ProductState() : this(false, false, false, false, string.Empty, Enumerable.Empty<ProductResponse>()) { }
}
