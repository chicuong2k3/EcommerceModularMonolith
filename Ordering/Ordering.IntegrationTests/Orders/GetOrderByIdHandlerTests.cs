using Ordering.Application.Orders.Queries;
using Ordering.Domain.OrderAggregate;
using Ordering.IntegrationTests.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Common.Domain;
using Catalog.Contracts;
using Moq;

namespace Ordering.IntegrationTests.Orders;

public class GetOrderByIdHandlerTests : IntegrationTestBase
{
    private readonly IOrderRepository orderRepository;
    private readonly Mock<IProductService> productServiceMock;

    public GetOrderByIdHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();

        // Set up mock for IProductService
        productServiceMock = new Mock<IProductService>();

        // Configure the service provider to use the mock
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(productServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsOrder_WhenOrderExists()
    {
        // Arrange - Create a test order
        var order = await CreateTestOrderAsync();
        var query = new GetOrderById(order.Id);

        // Act
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify order details
        var retrievedOrder = result.Value;
        Assert.NotNull(retrievedOrder);
        Assert.Equal(order.Id, retrievedOrder.Id);
        Assert.Equal(order.CustomerId, retrievedOrder.CustomerId);
        Assert.Equal(order.Status, retrievedOrder.Status);

        // Verify items
        Assert.NotEmpty(retrievedOrder.Items);
        Assert.Equal(order.Items.Count, retrievedOrder.Items.Count);

        // Verify monetary values
        Assert.Equal(order.Subtotal.Amount, retrievedOrder.Subtotal.Amount);
        Assert.Equal(order.Total.Amount, retrievedOrder.Total.Amount);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var nonExistentOrderId = Guid.NewGuid();
        var query = new GetOrderById(nonExistentOrderId);

        // Act
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is NotFoundError);
    }

    [Fact]
    public async Task Handle_ReturnsCompleteOrderDetails_WithAllProperties()
    {
        // Arrange - Create a test order with multiple items
        var order = await CreateTestOrderWithMultipleItemsAsync();
        var query = new GetOrderById(order.Id);

        // Act
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);

        var retrievedOrder = result.Value;
        Assert.NotNull(retrievedOrder);

        // Verify all order properties are loaded
        Assert.Equal(order.Id, retrievedOrder.Id);
        Assert.Equal(order.CustomerId, retrievedOrder.CustomerId);
        Assert.Equal(order.Status, retrievedOrder.Status);
        Assert.Equal(order.OrderDate, retrievedOrder.OrderDate);

        // Verify shipping info
        Assert.NotNull(retrievedOrder.ShippingInfo);
        Assert.Equal(order.ShippingInfo.ShippingCosts.Amount, retrievedOrder.ShippingInfo.ShippingCosts.Amount);
        Assert.Equal(order.ShippingInfo.PhoneNumber, retrievedOrder.ShippingInfo.PhoneNumber);
        Assert.NotNull(retrievedOrder.ShippingInfo.ShippingAddress);
        Assert.Equal(order.ShippingInfo.ShippingAddress.Country, retrievedOrder.ShippingInfo.ShippingAddress.Country);

        // Verify payment info
        Assert.NotNull(retrievedOrder.PaymentInfo);
        Assert.Equal(order.PaymentInfo.PaymentMethod, retrievedOrder.PaymentInfo.PaymentMethod);

        // Verify items
        Assert.Equal(2, retrievedOrder.Items.Count);
        foreach (var item in retrievedOrder.Items)
        {
            // Verify item properties
            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.NotEqual(Guid.Empty, item.ProductId);
            Assert.NotEqual(Guid.Empty, item.ProductVariantId);
            Assert.NotEmpty(item.ProductName);
            Assert.True(item.Quantity > 0);
            Assert.NotNull(item.OriginalPrice);
            Assert.NotNull(item.SalePrice);
        }

        // Verify monetary calculations
        var expectedSubtotal = 0m;
        foreach (var item in order.Items)
        {
            expectedSubtotal += item.SalePrice.Amount * item.Quantity;
        }

        Assert.Equal(expectedSubtotal, retrievedOrder.Subtotal.Amount);
        Assert.Equal(expectedSubtotal + order.ShippingInfo.ShippingCosts.Amount, retrievedOrder.Total.Amount);
    }

    private async Task<Order> CreateTestOrderAsync()
    {
        // Create test product
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Set up product mock
        var product = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variantId,
                    OriginalPrice = 29.99m,
                    SalePrice = 24.99m,
                    Quantity = 10,
                    ImageUrl = "https://test-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Blue" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Create order
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
            variantId,
            product.Name,
            1,
            Money.FromDecimal(29.99m).Value,
            Money.FromDecimal(24.99m).Value,
            "https://test-product.jpg",
            "Color: Blue, Size: M").Value;

        var orderResult = Order.Create(
            customerId,
            paymentInfo,
            shippingInfo,
            new List<OrderItem> { orderItem });

        var order = orderResult.Value;
        await orderRepository.AddAsync(order);

        return order;
    }

    private async Task<Order> CreateTestOrderWithMultipleItemsAsync()
    {
        // Create test products
        var product1Id = Guid.NewGuid();
        var variant1Id = Guid.NewGuid();

        var product1 = new ProductDto
        {
            Id = product1Id,
            Name = "First Test Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variant1Id,
                    OriginalPrice = 29.99m,
                    SalePrice = 24.99m,
                    Quantity = 10,
                    ImageUrl = "https://first-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Size", "L" } }
                }
            }
        };

        var product2Id = Guid.NewGuid();
        var variant2Id = Guid.NewGuid();

        var product2 = new ProductDto
        {
            Id = product2Id,
            Name = "Second Test Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variant2Id,
                    OriginalPrice = 49.99m,
                    SalePrice = 44.99m,
                    Quantity = 5,
                    ImageUrl = "https://second-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Black" }, { "Size", "XL" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(product1Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product1);
        productServiceMock.Setup(s => s.GetProductByIdAsync(product2Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product2);

        // Create order
        var customerId = Guid.NewGuid();
        var location = Location.Create(
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country").Value;

        var paymentInfo = PaymentInfo.Create("BankTransfer").Value;
        var shippingInfo = ShippingInfo.Create(
            Money.FromDecimal(7.99m).Value,
            location,
            "1234567890").Value;

        var orderItem1 = OrderItem.Create(
            product1Id,
            variant1Id,
            product1.Name,
            2,
            Money.FromDecimal(29.99m).Value,
            Money.FromDecimal(24.99m).Value,
            "https://first-product.jpg",
            "Color: Red, Size: L").Value;

        var orderItem2 = OrderItem.Create(
            product2Id,
            variant2Id,
            product2.Name,
            1,
            Money.FromDecimal(49.99m).Value,
            Money.FromDecimal(44.99m).Value,
            "https://second-product.jpg",
            "Color: Black, Size: XL").Value;

        var orderResult = Order.Create(
            customerId,
            paymentInfo,
            shippingInfo,
            new List<OrderItem> { orderItem1, orderItem2 });

        var order = orderResult.Value;
        await orderRepository.AddAsync(order);

        return order;
    }
}