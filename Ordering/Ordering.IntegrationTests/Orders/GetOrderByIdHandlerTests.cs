//using Ordering.Application.Orders.Queries;
//using Ordering.Domain.OrderAggregate;
//using Ordering.Domain.ProductAggregate;
//using Ordering.IntegrationTests.Abstractions;
//using Ordering.IntegrationTests.Helpers;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Xunit;

//namespace Ordering.IntegrationTests.Orders;

//public class GetOrderByIdHandlerTests : IntegrationTestBase
//{
//    private readonly IOrderRepository orderRepository;
//    private readonly IProductRepository productRepository;

//    public GetOrderByIdHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
//    {
//        orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();
//        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
//    }

//    [Fact]
//    public async Task Handle_ReturnsOrder_WhenOrderExists()
//    {
//        // Arrange - Create a test order
//        var order = await CreateTestOrderAsync();
//        var query = new GetOrderById(order.Id);

//        // Act
//        var result = await mediator.Send(query);

//        // Assert
//        Assert.True(result.IsSuccess);

//        // Verify order details
//        var retrievedOrder = result.Value;
//        Assert.NotNull(retrievedOrder);
//        Assert.Equal(order.Id, retrievedOrder.Id);
//        Assert.Equal(order.CustomerId, retrievedOrder.CustomerId);
//        Assert.Equal(order.Status, retrievedOrder.Status);

//        // Verify items
//        Assert.NotEmpty(retrievedOrder.Items);
//        Assert.Equal(order.Items.Count, retrievedOrder.Items.Count);

//        // Verify monetary values
//        Assert.Equal(order.Subtotal.Amount, retrievedOrder.Subtotal.Amount);
//        Assert.Equal(order.Total.Amount, retrievedOrder.Total.Amount);
//    }

//    [Fact]
//    public async Task Handle_ReturnsNotFound_WhenOrderDoesNotExist()
//    {
//        // Arrange
//        var nonExistentOrderId = Guid.NewGuid();
//        var query = new GetOrderById(nonExistentOrderId);

//        // Act
//        var result = await mediator.Send(query);

//        // Assert
//        Assert.True(result.IsFailed);
//        Assert.Contains(result.Errors, e => e.Message.Contains("not found"));
//    }

//    [Fact]
//    public async Task Handle_ReturnsCompleteOrderDetails_WithAllProperties()
//    {
//        // Arrange - Create a test order with multiple items
//        var order = await CreateTestOrderWithMultipleItemsAsync();
//        var query = new GetOrderById(order.Id);

//        // Act
//        var result = await mediator.Send(query);

//        // Assert
//        Assert.True(result.IsSuccess);

//        var retrievedOrder = result.Value;
//        Assert.NotNull(retrievedOrder);

//        // Verify all order properties are loaded
//        Assert.Equal(order.Id, retrievedOrder.Id);
//        Assert.Equal(order.CustomerId, retrievedOrder.CustomerId);
//        Assert.Equal(order.Status, retrievedOrder.Status);
//        Assert.Equal(order.OrderDate, retrievedOrder.OrderDate);

//        // Verify shipping info
//        Assert.NotNull(retrievedOrder.ShippingInfo);
//        Assert.Equal(order.ShippingInfo.ShippingCosts.Amount, retrievedOrder.ShippingInfo.ShippingCosts.Amount);
//        Assert.Equal(order.ShippingInfo.PhoneNumber, retrievedOrder.ShippingInfo.PhoneNumber);
//        Assert.NotNull(retrievedOrder.ShippingInfo.ShippingAddress);
//        Assert.Equal(order.ShippingInfo.ShippingAddress.Country, retrievedOrder.ShippingInfo.ShippingAddress.Country);

//        // Verify payment info
//        Assert.NotNull(retrievedOrder.PaymentInfo);
//        Assert.Equal(order.PaymentInfo.PaymentMethod, retrievedOrder.PaymentInfo.PaymentMethod);

//        // Verify items
//        Assert.Equal(2, retrievedOrder.Items.Count);
//        foreach (var item in retrievedOrder.Items)
//        {
//            // Verify item properties
//            Assert.NotEqual(Guid.Empty, item.Id);
//            Assert.NotEqual(Guid.Empty, item.ProductId);
//            Assert.NotEqual(Guid.Empty, item.ProductVariantId);
//            Assert.NotEmpty(item.ProductName);
//            Assert.True(item.Quantity > 0);
//            Assert.NotNull(item.OriginalPrice);
//            Assert.NotNull(item.SalePrice);
//        }

//        // Verify monetary calculations
//        var expectedSubtotal = 0m;
//        foreach (var item in order.Items)
//        {
//            expectedSubtotal += item.SalePrice.Amount * item.Quantity;
//        }

//        Assert.Equal(expectedSubtotal, retrievedOrder.Subtotal.Amount);
//        Assert.Equal(expectedSubtotal + order.ShippingInfo.ShippingCosts.Amount, retrievedOrder.Total.Amount);
//    }

//    private async Task<Order> CreateTestOrderAsync()
//    {
//        // Create test product
//        var product = TestProductHelper.CreateTestProduct(
//            Guid.NewGuid(), Guid.NewGuid(), faker.Commerce.ProductName(), 29.99m, 10);
//        await productRepository.AddProductAsync(product);

//        // Create order
//        var customerId = Guid.NewGuid();
//        var location = Location.Create(
//            "123 Test St",
//            "Test Ward",
//            "Test District",
//            "Test Province",
//            "Test Country").Value;

//        var paymentInfo = PaymentInfo.Create("COD").Value;
//        var shippingInfo = ShippingInfo.Create(
//            Money.FromDecimal(5.99m).Value,
//            location,
//            "1234567890").Value;

//        var orderItem = OrderItem.Create(
//            product.Id,
//            product.VariantId,
//            product.Name,
//            1,
//            Money.FromDecimal(product.OriginalPrice).Value,
//            Money.FromDecimal(product.OriginalPrice).Value,
//            product.ImageUrl,
//            product.AttributesDescription).Value;

//        var orderResult = Order.Create(
//            customerId,
//            paymentInfo,
//            shippingInfo,
//            new List<OrderItem> { orderItem });

//        var order = orderResult.Value;
//        await orderRepository.AddAsync(order);

//        return order;
//    }

//    private async Task<Order> CreateTestOrderWithMultipleItemsAsync()
//    {
//        // Create test products
//        var product1 = TestProductHelper.CreateTestProduct(
//            Guid.NewGuid(), Guid.NewGuid(), faker.Commerce.ProductName(), 29.99m, 10);
//        var product2 = TestProductHelper.CreateTestProduct(
//            Guid.NewGuid(), Guid.NewGuid(), faker.Commerce.ProductName(), 49.99m, 5);

//        await productRepository.AddProductAsync(product1);
//        await productRepository.AddProductAsync(product2);

//        // Create order
//        var customerId = Guid.NewGuid();
//        var location = Location.Create(
//            "123 Test St",
//            "Test Ward",
//            "Test District",
//            "Test Province",
//            "Test Country").Value;

//        var paymentInfo = PaymentInfo.Create("CreditCard").Value;
//        var shippingInfo = ShippingInfo.Create(
//            Money.FromDecimal(7.99m).Value,
//            location,
//            "1234567890").Value;

//        var orderItem1 = OrderItem.Create(
//            product1.Id,
//            product1.VariantId,
//            product1.Name,
//            2,
//            Money.FromDecimal(product1.OriginalPrice).Value,
//            Money.FromDecimal(product1.OriginalPrice).Value,
//            product1.ImageUrl,
//            product1.AttributesDescription).Value;

//        var orderItem2 = OrderItem.Create(
//            product2.Id,
//            product2.VariantId,
//            product2.Name,
//            1,
//            Money.FromDecimal(product2.OriginalPrice).Value,
//            Money.FromDecimal(product2.OriginalPrice).Value,
//            product2.ImageUrl,
//            product2.AttributesDescription).Value;

//        var orderResult = Order.Create(
//            customerId,
//            paymentInfo,
//            shippingInfo,
//            new List<OrderItem> { orderItem1, orderItem2 });

//        var order = orderResult.Value;
//        await orderRepository.AddAsync(order);

//        return order;
//    }
//}