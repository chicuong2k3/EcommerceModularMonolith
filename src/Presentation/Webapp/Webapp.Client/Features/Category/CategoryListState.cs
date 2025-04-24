using Catalog.ApiContracts.Responses;
using TimeWarp.State;

namespace Webapp.Client.Features.Category;

internal sealed partial class CategoryListState : State<CategoryListState>
{
    public List<CategoryResponse> Categories { get; private set; }
    public bool IsLoading { get; private set; }

    public override void Initialize()
    {
        Categories = new List<CategoryResponse>();
        IsLoading = false;
    }
}
