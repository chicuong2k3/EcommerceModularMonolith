namespace AdminDashboard.Client.Store.Breadcrumb;

public static class BreadcrumbActions
{
    public record SetBreadcrumbAction(List<BreadcrumbItem> Items);
    public record ClearBreadcrumbAction();
}
