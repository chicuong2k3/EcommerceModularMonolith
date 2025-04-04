using Fluxor;

namespace AdminDashboard.Client.Store.Breadcrumb;

[FeatureState]
public record BreadcrumbState(List<BreadcrumbItem> Items)
{
    public static BreadcrumbState Empty => new(new List<BreadcrumbItem>());

    private BreadcrumbState() : this(new List<BreadcrumbItem>())
    {
    }
}