using Ordering.Application.Orders.Commands;
using Ordering.Domain.CartAggregate;
using Ordering.Domain.OrderAggregate;
using Ordering.Domain.ProductAggregate;
using Ordering.IntegrationTests.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Common.Domain;
using Ordering.Domain.OrderAggregate.Errors;

namespace Ordering.IntegrationTests.Orders;

public class PlaceOrderHandlerTests : IntegrationTestBase
{
    private readonly IOrderRepository orderRepository;
    private readonly ICartRepository cartRepository;
    private readonly IProductRepository productRepository;
    private readonly OrderingDbContext dbContext;

    public PlaceOrderHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();
        cartRepository = serviceScope.ServiceProvider.GetRequiredService<ICartRepository>();
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
        dbContext = serviceScope.ServiceProvider.GetRequiredService<OrderingDbContext>();

        // Clear database before each test to ensure a clean state
        ClearDatabase().GetAwaiter().GetResult();
    }

    private async Task ClearDatabase()
    {
        // Remove existing order items
        var existingOrderItems = await dbContext.Set<OrderItem>().ToListAsync();
        if (existingOrderItems.Any())
        {
            dbContext.Set<OrderItem>().RemoveRange(existingOrderItems);
            await dbContext.SaveChangesAsync();
        }

        // Remove existing orders
        var existingOrders = await dbContext.Set<Order>().ToListAsync();
        if (existingOrders.Any())
        {
            dbContext.Set<Order>().RemoveRange(existingOrders);
            await dbContext.SaveChangesAsync();
        }

        // Remove existing carts
        var existingCarts = await dbContext.Set<Cart>().ToListAsync();
        if (existingCarts.Any())
        {
            dbContext.Set<Cart>().RemoveRange(existingCarts);
            await dbContext.SaveChangesAsync();
        }

        // Remove existing products
        var existingProducts = await dbContext.Set<Product>().ToListAsync();
        if (existingProducts.Any())
        {
            dbContext.Set<Product>().RemoveRange(existingProducts);
            await dbContext.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task Handle_ReturnSuccess_WhenCreatingOrderWithCODPayment()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Create products with sufficient stock
        var product1Id = Guid.NewGuid();
        var variant1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        var variant2Id = Guid.NewGuid();

        var product1 = new Product(
            product1Id,
            variant1Id,
            "Test Product 1",
            10.0m,
            5, // 5 in stock
            "https://test-product-1.jpg",
            8.0m, // Sale price
            "Test product 1 description");

        var product2 = new Product(
            product2Id,
            variant2Id,
            "Test Product 2",
            15.0m,
            3, // 3 in stock
            "https://test-product-2.jpg",
            12.0m, // Sale price
            "Test product 2 description");

        await productRepository.AddProductAsync(product1);
        await productRepository.AddProductAsync(product2);

        // Create cart with items
        var cart = new Cart(customerId);
        await cart.AddItemAsync(product1Id, variant1Id, 2);
        await cart.AddItemAsync(product2Id, variant2Id, 1);
        await cartRepository.UpsertAsync(cart);

        // Create place order command
        var command = new PlaceOrder(
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "1234567890",
            "COD",
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify order was created with correct data
        var order = await orderRepository.GetByIdAsync(result.Value.Id);
        Assert.NotNull(order);

        // Verify order properties
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.Processing, order.Status);
        Assert.Equal(2, order.Items.Count);

        // Verify order items
        Assert.Contains(order.Items, item => item.ProductId == product1Id && item.Quantity == 2 && item.SalePrice == Money.FromDecimal(8.0m).Value);
        Assert.Contains(order.Items, item => item.ProductId == product2Id && item.Quantity == 1 && item.SalePrice == Money.FromDecimal(12.0m).Value);

        // Verify shipping info
        Assert.Equal("123 Test St", order.ShippingInfo.ShippingAddress.Street);
        Assert.Equal("Test Ward", order.ShippingInfo.ShippingAddress.Ward);
        Assert.Equal("1234567890", order.ShippingInfo.PhoneNumber);

        // Verify payment info
        Assert.Equal("COD", order.PaymentInfo.PaymentMethod.ToString());
    }

    [Fact]
    public async Task Handle_ReturnSuccess_WhenCreatingOrderWithOnlinePayment()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Create products with sufficient stock
        var product1Id = Guid.NewGuid();
        var variant1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        var variant2Id = Guid.NewGuid();

        var product1 = new Product(
            product1Id,
            variant1Id,
            "Test Product 1",
            10.0m,
            5, // 5 in stock
            "https://test-product-1.jpg",
            8.0m, // Sale price
            "Test product 1 description");

        var product2 = new Product(
            product2Id,
            variant2Id,
            "Test Product 2",
            15.0m,
            3, // 3 in stock
            "https://test-product-2.jpg",
            12.0m, // Sale price
            "Test product 2 description");

        await productRepository.AddProductAsync(product1);
        await productRepository.AddProductAsync(product2);

        // Create cart with items
        var cart = new Cart(customerId);
        await cart.AddItemAsync(product1Id, variant1Id, 2);
        await cart.AddItemAsync(product2Id, variant2Id, 1);
        await cartRepository.UpsertAsync(cart);

        // Create place order command
        var command = new PlaceOrder(
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "1234567890",
            "BankTransfer",
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify order was created with correct data
        var order = await orderRepository.GetByIdAsync(result.Value.Id);
        Assert.NotNull(order);

        // Verify order properties
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.PendingPayment, order.Status);
        Assert.Equal(2, order.Items.Count);

        // Verify order items
        Assert.Contains(order.Items, item => item.ProductId == product1Id && item.Quantity == 2 && item.SalePrice == Money.FromDecimal(8.0m).Value);
        Assert.Contains(order.Items, item => item.ProductId == product2Id && item.Quantity == 1 && item.SalePrice == Money.FromDecimal(12.0m).Value);

        // Verify shipping info
        Assert.Equal("123 Test St", order.ShippingInfo.ShippingAddress.Street);
        Assert.Equal("Test Ward", order.ShippingInfo.ShippingAddress.Ward);
        Assert.Equal("1234567890", order.ShippingInfo.PhoneNumber);

        // Verify payment info
        Assert.Equal("BankTransfer", order.PaymentInfo.PaymentMethod.ToString());
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenProductDoesNotExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var nonExistentProductId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Create cart with a non-existent product
        var cart = new Cart(customerId);
        await cart.AddItemAsync(nonExistentProductId, variantId, 2);
        await cartRepository.UpsertAsync(cart);

        // Create place order command
        var command = new PlaceOrder(
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "1234567890",
            "COD",
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is NotFoundError);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenInsufficientStock()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Create product with limited stock
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        var product = new Product(
            productId,
            variantId,
            "Limited Stock Product",
            10.0m,
            2, // Only 2 in stock
            "https://limited-stock.jpg",
            null, // No sale price
            "Limited stock product description");

        await productRepository.AddProductAsync(product);

        // Create cart with item requesting more than available stock
        var cart = new Cart(customerId);
        await cart.AddItemAsync(productId, variantId, 5); // Request 5, but only 2 in stock
        await cartRepository.UpsertAsync(cart);

        // Create place order command
        var command = new PlaceOrder(
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "1234567890",
            "COD",
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is OutOfStockError);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenAddressIsInvalid()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Create product with sufficient stock
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var product = new Product(
            productId,
            variantId,
            "Test Product",
            10.0m,
            5,
            "https://test.jpg",
            null,
            "Test description");
        await productRepository.AddProductAsync(product);

        // Create cart with items
        var cart = new Cart(customerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        // Create place order command with invalid address (null Ward and District)
        var command = new PlaceOrder(
            customerId,
            "123 Test St",
            string.Empty, // Missing Ward
            string.Empty, // Missing District
            "Test Province",
            "Test Country",
            "1234567890",
            "COD",
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenPaymentMethodIsInvalid()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Create product with sufficient stock
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var product = new Product(
            productId,
            variantId,
            "Test Product",
            10.0m,
            5,
            "https://test.jpg",
            null,
            "Test description");
        await productRepository.AddProductAsync(product);

        // Create cart with items
        var cart = new Cart(customerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        // Create place order command with invalid payment method
        var command = new PlaceOrder(
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "1234567890",
            "InvalidPaymentMethod",
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenCartIsEmpty()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Create an empty cart
        var cart = new Cart(customerId);
        await cartRepository.UpsertAsync(cart);

        // Create place order command
        var command = new PlaceOrder(
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "1234567890",
            "COD",
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is CartEmptyError);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenCartDoesNotExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // No cart is created for the customer

        // Create place order command
        var command = new PlaceOrder(
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "1234567890",
            "COD",
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is CartEmptyError);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenPhoneNumberIsInvalid()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Create product with sufficient stock
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var product = new Product(
            productId,
            variantId,
            "Test Product",
            10.0m,
            5,
            "https://test.jpg",
            null,
            "Test description");
        await productRepository.AddProductAsync(product);

        // Create cart with items
        var cart = new Cart(customerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        // Create place order command with invalid phone number
        var command = new PlaceOrder(
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "invalid-phone",
            "COD",
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenShippingMethodIsInvalid()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Create product with sufficient stock
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var product = new Product(
            productId,
            variantId,
            "Test Product",
            10.0m,
            5,
            "https://test.jpg",
            null,
            "Test description");
        await productRepository.AddProductAsync(product);

        // Create cart with items
        var cart = new Cart(customerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        // Create place order command with invalid shipping method
        var command = new PlaceOrder(
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "1234567890",
            "COD",
            "InvalidShippingMethod");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }
}