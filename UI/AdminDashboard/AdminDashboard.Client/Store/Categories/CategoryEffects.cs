using Fluxor;
using Microsoft.AspNetCore.Components;
using Radzen;
using static AdminDashboard.Client.Store.Categories.CategoryActions;

namespace AdminDashboard.Client.Store.Categories;

public class CategoryEffects
{
    private readonly ICategoryService categoryService;
    private readonly NotificationService notificationService;
    private readonly NavigationManager navigationManager;

    public CategoryEffects(
        ICategoryService categoryService,
        NotificationService notificationService,
        NavigationManager navigationManager)
    {
        this.categoryService = categoryService;
        this.notificationService = notificationService;
        this.navigationManager = navigationManager;
    }

    [EffectMethod]
    public async Task HandleFetchCategories(
        FetchCategoriesAction action,
        IDispatcher dispatcher)
    {
        try
        {
            var categories = await categoryService.GetCategoriesAsync();
            dispatcher.Dispatch(new FetchCategoriesSuccessAction(categories));
        }
        catch (Exception)
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Lỗi",
                Detail = "Có lỗi xảy ra khi tải danh sách danh mục."
            });
        }
    }

    [EffectMethod]
    public async Task HandleCreateCategory(
        CreateCategoryAction action,
        IDispatcher dispatcher)
    {
        try
        {
            var createdCategory = await categoryService.CreateCategoryAsync(action.Category);
            dispatcher.Dispatch(new CreateCategorySuccessAction(createdCategory));

            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Thành công",
                Detail = "Danh mục đã được tạo thành công"
            });

            navigationManager.NavigateTo("/catalog/categories");
        }
        catch (Exception)
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Lỗi",
                Detail = "Có lỗi xảy ra. Vui lòng thử lại sau."
            });
        }
    }

    [EffectMethod]
    public async Task HandleUpdateCategory(
        UpdateCategoryAction action,
        IDispatcher dispatcher)
    {
        try
        {
            var updatedCategory = await categoryService.UpdateCategoryAsync(
                action.CategoryId,
                action.Category);
            dispatcher.Dispatch(new UpdateCategorySuccessAction(updatedCategory));
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Thành công",
                Detail = "Danh mục đã được cập nhật thành công"
            });

            navigationManager.NavigateTo("/catalog/categories");

        }
        catch (Exception)
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Lỗi",
                Detail = "Có lỗi xảy ra. Vui lòng thử lại sau."
            });
        }
    }

    [EffectMethod]
    public async Task HandleDeleteCategory(
        DeleteCategoryAction action,
        IDispatcher dispatcher)
    {
        try
        {
            await categoryService.DeleteCategoryAsync(action.CategoryId);
            dispatcher.Dispatch(new DeleteCategorySuccessAction(action.CategoryId));
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Thành công",
                Detail = "Danh mục đã được xóa thành công"
            });
        }
        catch (Exception)
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Lỗi",
                Detail = "Có lỗi xảy ra. Vui lòng thử lại sau."
            });
        }
    }
}