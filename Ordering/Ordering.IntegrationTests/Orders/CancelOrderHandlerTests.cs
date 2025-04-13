using Ordering.Application.Orders.Commands;
using Ordering.Domain.OrderAggregate;
using Catalog.Contracts;
using Ordering.IntegrationTests.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Persistence;
using Common.Domain;
using Ordering.Domain.OrderAggregate.Errors;
using Moq;

namespace Ordering.IntegrationTests.Orders;

public class CancelOrderHandlerTests : IntegrationTestBase
{
    private readonly IOrderRepository orderRepository;
    private readonly OrderingDbContext dbContext;
    private readonly Mock<IProductService> productServiceMock;

    public CancelOrderHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();
        dbContext = serviceScope.ServiceProvider.GetRequiredService<OrderingDbContext>();

        // Set up mock for IProductService
        productServiceMock = new Mock<IProductService>();

        // Configure the service provider to use the mock
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(productServiceMock.Object);

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
    public async Task Handle_CancelsOrder_InPaymentPendingStatus()
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
        var order = await CreateOrderAsync(OrderStatus.Refunded);
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

        // Set up product mock
        var product = new ProductDto
        {
            Id = productId,
            Name = $"Test Product for {status}",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variantId,
                    OriginalPrice = 29.99m,
                    SalePrice = null,
                    Quantity = 10,
                    ImageUrl = "https://test-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Blue" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

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
            "https://test-product.jpg",
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

        // Create the order
        var orderResult = Order.Create(
            customerId,
            paymentInfo,
            shippingInfo,
            new List<OrderItem> { orderItem });

        var order = orderResult.Value;

        // Set the status according to the parameter
        // Note: This is for testing purposes, normally status changes would follow business logic
        var setStatusField = typeof(Order).GetProperty("Status");
        setStatusField?.SetValue(order, status);

        // Save to repository
        await orderRepository.AddAsync(order);

        return order;
    }
}