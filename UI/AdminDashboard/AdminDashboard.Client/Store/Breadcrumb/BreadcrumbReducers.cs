using Fluxor;
using static AdminDashboard.Client.Store.Breadcrumb.BreadcrumbActions;

namespace AdminDashboard.Client.Store.Breadcrumb;

public static class BreadcrumbReducers
{
    [ReducerMethod]
    public static BreadcrumbState ReduceSetBreadcrumb(
        BreadcrumbState state,
        SetBreadcrumbAction action) =>
            state with { Items = action.Items };

    [ReducerMethod(typeof(ClearBreadcrumbAction))]
    public static BreadcrumbState ReduceClearBreadcrumb(
        BreadcrumbState state) =>
            state with { Items = new List<BreadcrumbItem>() };
}