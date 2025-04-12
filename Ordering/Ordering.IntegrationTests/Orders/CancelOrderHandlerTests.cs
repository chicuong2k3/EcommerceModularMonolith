using Ordering.Application.Orders.Commands;
using Ordering.Domain.OrderAggregate;
using Ordering.Domain.ProductAggregate;
using Ordering.IntegrationTests.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Persistence;
using Ordering.Domain.OrderAggregate.Events;
using Common.Infrastructure.Outbox;
using Common.Domain;
using Ordering.Domain.OrderAggregate.Errors;

namespace Ordering.IntegrationTests.Orders;

public class CancelOrderHandlerTests : IntegrationTestBase
{
    private readonly IOrderRepository orderRepository;
    private readonly IProductRepository productRepository;
    private readonly OrderingDbContext dbContext;

    public CancelOrderHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
        dbContext = serviceScope.ServiceProvider.GetRequiredService<OrderingDbContext>();

        // Clear any existing order items to prevent conflicts
        ClearDatabase().GetAwaiter().GetResult();
    }

    private async Task ClearDatabase()
    {
        // Remove any existing order items to avoid conflicts
        var existingOrderItems = await dbContext.Set<OrderItem>().ToListAsync();
        if (existingOrderItems.Any())
        {
            dbContext.Set<OrderItem>().RemoveRange(existingOrderItems);
            await dbContext.SaveChangesAsync();
        }

        // Remove any existing orders
        var existingOrders = await dbContext.Set<Order>().ToListAsync();
        if (existingOrders.Any())
        {
            dbContext.Set<Order>().RemoveRange(existingOrders);
            await dbContext.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task Handle_CancelsOrder_InPendingPaymentStatus()
    {
        // Arrange
        var order = await CreateOrderAsync(OrderStatus.PendingPayment);
        var cancelCommand = new CancelOrder(order.Id);

        // Act
        var result = await mediator.Send(cancelCommand);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify order status changed
        var canceledOrder = await orderRepository.GetByIdAsync(order.Id);
        Assert.NotNull(canceledOrder);
        Assert.Equal(OrderStatus.Canceled, canceledOrder.Status);

    }

    [Fact]
    public async Task Handle_CancelsOrder_InProcessingStatus()
    {
        // Arrange
        var order = await CreateOrderAsync(OrderStatus.Processing);
        var cancelCommand = new CancelOrder(order.Id);

        // Act
        var result = await mediator.Send(cancelCommand);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify order status changed
        var canceledOrder = await orderRepository.GetByIdAsync(order.Id);
        Assert.NotNull(canceledOrder);
        Assert.Equal(OrderStatus.Canceled, canceledOrder.Status);
    }

    [Fact]
    public async Task Handle_ReturnsFail_ForNonExistentOrder()
    {
        // Arrange
        var nonExistentOrderId = Guid.NewGuid();
        var cancelCommand = new CancelOrder(nonExistentOrderId);

        // Act
        var result = await mediator.Send(cancelCommand);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is NotFoundError);
    }

    [Fact]
    public async Task Handle_ReturnsFail_ForAlreadyShippedOrder()
    {
        // Arrange
        var order = await CreateOrderAsync(OrderStatus.Shipped);
        var cancelCommand = new CancelOrder(order.Id);

        // Act
        var result = await mediator.Send(cancelCommand);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is InvalidOrderStatus);
    }

    [Fact]
    public async Task Handle_ReturnsFail_ForDeliveredOrder()
    {
        // Arrange
        var order = await CreateOrderAsync(OrderStatus.Delivered);
        var cancelCommand = new CancelOrder(order.Id);

        // Act
        var result = await mediator.Send(cancelCommand);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is InvalidOrderStatus);
    }

    [Fact]
    public async Task Handle_ReturnsFail_ForAlreadyCancelledOrder()
    {
        // Arrange
        var order = await CreateOrderAsync(OrderStatus.Canceled);
        var cancelCommand = new CancelOrder(order.Id);

        // Act
        var result = await mediator.Send(cancelCommand);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is InvalidOrderStatus);
    }

    [Fact]
    public async Task Handle_ReturnsFail_ForRefundedOrder()
    {
        // Arrange
        var order = await CreateTestOrderAsync(OrderStatus.Refunded);
        var cancelCommand = new CancelOrder(order.Id);

        // Act
        var result = await mediator.Send(cancelCommand);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is InvalidOrderStatus);
    }


    private async Task<Order> CreateOrderAsync(OrderStatus status)
    {
        // Create a product with a unique ID for each test
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        var product = new Product(
            productId,
            variantId,
            $"Test Product for {status}",
            29.99m,
            10,
            "https://test-product.jpg",
            null,
            "Test product description");

        await productRepository.AddProductAsync(product);

        // Create an order with a unique customer ID for each test
        var customerId = Guid.NewGuid();

        // Create a unique order item with the product
        var orderItem = OrderItem.Create(
            productId,
            variantId,
            product.Name,
            1,
            Money.FromDecimal(29.99m).Value,
            Money.FromDecimal(29.99m).Value,
            null,
            null).Value;

        var shippingInfo = ShippingInfo.Create(
            Money.FromDecimal(5.99m).Value,
            Location.Create(
                "123 Test St",
                "Test Ward",
                "Test District",
                "Test Province",
                "Test Country").Value,
            "1234567890").Value;

        var paymentInfo = PaymentInfo.Create("COD").Value;

        var order = Order.Create(customerId, paymentInfo, shippingInfo, [orderItem]).Value;

        order.Status = status;

        await orderRepository.AddAsync(order);
        return order;
    }

    private async Task<Order> CreateTestOrderAsync(OrderStatus initialStatus)
    {
        // Create test products
        var product = TestProductHelper.CreateTestProduct(
            Guid.NewGuid(), Guid.NewGuid(), faker.Commerce.ProductName(), 29.99m, 10);
        await productRepository.AddProductAsync(product);

        // Create a new order
        var customerId = Guid.NewGuid();
        var location = Location.Create(
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country").Value;

        var paymentInfo = PaymentInfo.Create("COD").Value;
        var shippingInfo = ShippingInfo.Create(
            Money.FromDecimal(5.99m).Value,
            location,
            "1234567890").Value;

        var orderItem = OrderItem.Create(
            product.Id,
            product.VariantId,
            product.Name,
            1,
            Money.FromDecimal(product.OriginalPrice).Value,
            Money.FromDecimal(product.OriginalPrice).Value,
            product.ImageUrl,
            product.AttributesDescription).Value;

        var orderResult = Order.Create(
            customerId,
            paymentInfo,
            shippingInfo,
            new List<OrderItem> { orderItem });

        var order = orderResult.Value;

        typeof(Order).GetProperty(nameof(Order.Status))
            ?.SetValue(order, initialStatus);

        await orderRepository.AddAsync(order);

        return order;
    }
}