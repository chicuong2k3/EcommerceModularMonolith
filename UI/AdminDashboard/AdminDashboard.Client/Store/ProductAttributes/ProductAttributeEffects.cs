using Fluxor;
using Microsoft.AspNetCore.Components;
using Radzen;
using static AdminDashboard.Client.Store.ProductAttributes.ProductAttributeActions;

namespace AdminDashboard.Client.Store.ProductAttributes;

public class ProductAttributeEffects
{
    private readonly IProductAttributeService productAttributeService;
    private readonly NotificationService notificationService;
    private readonly NavigationManager navigationManager;

    public ProductAttributeEffects(
        IProductAttributeService productAttributeService,
        NotificationService notificationService,
        NavigationManager navigationManager)
    {
        this.productAttributeService = productAttributeService;
        this.notificationService = notificationService;
        this.navigationManager = navigationManager;
    }

    [EffectMethod]
    public async Task HandleFetchProductAttributes(
        FetchProductAttributesAction action,
        IDispatcher dispatcher)
    {
        try
        {
            var productAttributes = await productAttributeService.GetProductAttributesAsync();
            dispatcher.Dispatch(new FetchProductAttributesSuccessAction(productAttributes));
        }
        catch (Exception)
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Lỗi",
                Detail = "Có lỗi xảy ra khi tải danh sách thuộc tính sản phẩm."
            });

        }
    }

    [EffectMethod]
    public async Task HandleCreateProductAttribute(
        CreateProductAttributeAction action,
        IDispatcher dispatcher)
    {
        try
        {
            var createdProductAttribute = await productAttributeService.CreateProductAttributeAsync(action.ProductAttribute);
            dispatcher.Dispatch(new CreateProductAttributeSuccessAction(createdProductAttribute));

            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Thành công",
                Detail = "Thuộc tính sản phẩm đã được tạo thành công"
            });

            navigationManager.NavigateTo("/catalog/product-attributes");
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
    public async Task HandleUpdateProductAttribute(
        UpdateProductAttributeAction action,
        IDispatcher dispatcher)
    {
        try
        {
            var updatedProductAttribute = await productAttributeService.UpdateProductAttributeAsync(
                action.ProductAttributeId,
                action.ProductAttribute);
            dispatcher.Dispatch(new UpdateProductAttributeSuccessAction(updatedProductAttribute));
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Thành công",
                Detail = "Thuộc tính sản phẩm đã được cập nhật thành công"
            });

            navigationManager.NavigateTo("/catalog/product-attributes");
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
    public async Task HandleDeleteProductAttribute(
        DeleteProductAttributeAction action,
        IDispatcher dispatcher)
    {
        try
        {
            await productAttributeService.DeleteProductAttributeAsync(action.ProductAttributeId);
            dispatcher.Dispatch(new DeleteProductAttributeSuccessAction(action.ProductAttributeId));
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Thành công",
                Detail = "Thuộc tính sản phẩm đã được xóa thành công"
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

