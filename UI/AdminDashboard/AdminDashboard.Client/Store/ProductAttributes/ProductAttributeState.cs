using Catalog.ApiContracts.Responses;
using Fluxor;

namespace AdminDashboard.Client.Store.ProductAttributes;

[FeatureState]
public record ProductAttributeState(
    bool IsLoading,
    string SearchQuery,
    bool IsCreating,
    bool IsUpdating,
    bool IsDeleting,
    IEnumerable<ProductAttributeResponse> ProductAttributes)
{
    public ProductAttributeState()
        : this(false, string.Empty, false, false, false, Array.Empty<ProductAttributeResponse>())
    { }
}