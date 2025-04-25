using Ordering.Core.Persistence;

namespace Ordering.IntegrationTests.Orders;

public class PlaceOrderHandlerTests : IntegrationTestBase
{
    private readonly IOrderRepository orderRepository;
    private readonly ICartRepository cartRepository;
    private readonly Mock<IProductService> productServiceMock;
    private readonly OrderingDbContext dbContext;

    public PlaceOrderHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        // Create the mock first
        productServiceMock = new Mock<IProductService>();

        // Create a new scope with the mock service
        CreateNewServiceScope(factory, services =>
        {
            // Remove any existing IProductService registrations
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IProductService));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add our mock
            services.AddScoped<IProductService>(_ => productServiceMock.Object);
        });

        // Get services from the updated scope
        orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();
        cartRepository = serviceScope.ServiceProvider.GetRequiredService<ICartRepository>();
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
    }

    [Fact]
    public async Task Handle_ReturnSuccess_WhenCreatingOrderWithCODPayment()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Setup product data via mock
        var product1Id = Guid.NewGuid();
        var variant1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        var variant2Id = Guid.NewGuid();

        var product1 = new ProductDto
        {
            Id = product1Id,
            Name = "Test Product 1",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variant1Id,
                    OriginalPrice = 10.0m,
                    SalePrice = 8.0m,
                    Quantity = 5,
                    ImageUrl = "https://test-product-1.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Size", "M" } }
                }
            }
        };

        var product2 = new ProductDto
        {
            Id = product2Id,
            Name = "Test Product 2",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variant2Id,
                    OriginalPrice = 15.0m,
                    SalePrice = 12.0m,
                    Quantity = 3,
                    ImageUrl = "https://test-product-2.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Blue" }, { "Size", "L" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(product1Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product1);
        productServiceMock.Setup(s => s.GetProductByIdAsync(product2Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product2);

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
        var order = await orderRepository.GetByIdAsync(result.Value);
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

        // Setup product data via mock
        var product1Id = Guid.NewGuid();
        var variant1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        var variant2Id = Guid.NewGuid();

        var product1 = new ProductDto
        {
            Id = product1Id,
            Name = "Test Product 1",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variant1Id,
                    OriginalPrice = 10.0m,
                    SalePrice = 8.0m,
                    Quantity = 5,
                    ImageUrl = "https://test-product-1.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Size", "M" } }
                }
            }
        };

        var product2 = new ProductDto
        {
            Id = product2Id,
            Name = "Test Product 2",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variant2Id,
                    OriginalPrice = 15.0m,
                    SalePrice = 12.0m,
                    Quantity = 3,
                    ImageUrl = "https://test-product-2.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Blue" }, { "Size", "L" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(product1Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product1);
        productServiceMock.Setup(s => s.GetProductByIdAsync(product2Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product2);

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
        var order = await orderRepository.GetByIdAsync(result.Value);
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

        // Setup product service mock to return null for nonExistentProductId
        productServiceMock.Setup(s => s.GetProductByIdAsync(nonExistentProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Create cart with non-existent product
        var cart = new Cart(customerId);
        await cart.AddItemAsync(nonExistentProductId, variantId, 1);
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
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Setup product with limited stock
        var product = new ProductDto
        {
            Id = productId,
            Name = "Limited Stock Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variantId,
                    OriginalPrice = 10.0m,
                    SalePrice = null,
                    Quantity = 2, // Only 2 in stock
                    ImageUrl = "https://limited-stock.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Black" }, { "Size", "S" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

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
        Assert.Contains(result.Errors, e => e is Error);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenAddressIsInvalid()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Setup product
        var product = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variantId,
                    OriginalPrice = 10.0m,
                    SalePrice = null,
                    Quantity = 5,
                    ImageUrl = "https://test.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Green" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

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
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Setup product
        var product = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variantId,
                    OriginalPrice = 10.0m,
                    SalePrice = null,
                    Quantity = 5,
                    ImageUrl = "https://test.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Green" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

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
        Assert.Contains(result.Errors, e => e is Error);
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
        Assert.Contains(result.Errors, e => e is Error);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenPhoneNumberIsInvalid()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Setup product
        var product = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variantId,
                    OriginalPrice = 10.0m,
                    SalePrice = null,
                    Quantity = 5,
                    ImageUrl = "https://test.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Green" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

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
            "123", // Invalid phone number (too short)
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
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Setup product
        var product = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = variantId,
                    OriginalPrice = 10.0m,
                    SalePrice = null,
                    Quantity = 5,
                    ImageUrl = "https://test.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Green" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

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
        Assert.Contains(result.Errors, e => e is Error);
    }
}