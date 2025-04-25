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
    public async Task Handle_CreatesOrder_WithCODPayment()
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
        var cart = new Cart(Guid.NewGuid(), customerId);
        await cart.AddItemAsync(product1Id, variant1Id, 2);
        await cart.AddItemAsync(product2Id, variant2Id, 1);
        await cartRepository.UpsertAsync(cart);

        // Create place order command
        var command = new PlaceOrder(
            Guid.NewGuid(),
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

        var orders = await dbContext.Set<Order>().Include(o => o.Items).ToListAsync();
        Assert.Single(orders);
        var order = orders[0];

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

        // Verify cart is cleared after successful order
        var updatedCart = await cartRepository.GetAsync(customerId);
        Assert.NotNull(updatedCart);
        Assert.Empty(updatedCart.Items);
    }

    [Fact]
    public async Task Handle_CreatesOrder_WithOnlinePayment()
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
        var cart = new Cart(Guid.NewGuid(), customerId);
        await cart.AddItemAsync(product1Id, variant1Id, 2);
        await cart.AddItemAsync(product2Id, variant2Id, 1);
        await cartRepository.UpsertAsync(cart);

        // Create place order command
        var command = new PlaceOrder(
            Guid.NewGuid(),
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

        var orders = await dbContext.Set<Order>().Include(o => o.Items).ToListAsync();
        Assert.Single(orders);
        var order = orders[0];

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

        // Verify cart is cleared after successful order
        var updatedCart = await cartRepository.GetAsync(customerId);
        Assert.NotNull(updatedCart);
        Assert.Empty(updatedCart.Items);
    }

    [Fact]
    public async Task Handle_Failure_ProductDoesNotExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var nonExistentProductId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        productServiceMock.Setup(s => s.GetProductByIdAsync(nonExistentProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Create cart with non-existent product
        var cart = new Cart(Guid.NewGuid(), customerId);
        await cart.AddItemAsync(nonExistentProductId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        var command = new PlaceOrder(
            Guid.NewGuid(),
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
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task Handle_Failure_InsufficientStock()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

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
                    SalePrice = 8.0m,
                    Quantity = 1, // Only 1 in stock
                    ImageUrl = "https://test-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Create cart with quantity more than stock
        var cart = new Cart(Guid.NewGuid(), customerId);
        await cart.AddItemAsync(productId, variantId, 2); // Trying to order 2
        await cartRepository.UpsertAsync(cart);

        var command = new PlaceOrder(
            Guid.NewGuid(),
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
        Assert.Contains(result.Errors, error => error is Error);
    }

    [Fact]
    public async Task Handle_Failure_InvalidAddress()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Setup product data via mock
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

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
                    SalePrice = 8.0m,
                    Quantity = 5,
                    ImageUrl = "https://test-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Create cart with items
        var cart = new Cart(Guid.NewGuid(), customerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        var command = new PlaceOrder(
            Guid.NewGuid(),
            customerId,
            "Street",
            "",
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
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task Handle_Failure_InvalidPaymentMethod()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Setup product data via mock
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

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
                    SalePrice = 8.0m,
                    Quantity = 5,
                    ImageUrl = "https://test-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Create cart with items
        var cart = new Cart(Guid.NewGuid(), customerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        var command = new PlaceOrder(
            Guid.NewGuid(),
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "1234567890",
            "InvalidMethod", // Invalid payment method
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
        Assert.Contains(result.Errors, error => error.Message.Contains("payment method"));
    }

    [Fact]
    public async Task Handle_Failure_EmptyCart()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Create empty cart
        var cart = new Cart(Guid.NewGuid(), customerId);
        await cartRepository.UpsertAsync(cart);

        var command = new PlaceOrder(
            Guid.NewGuid(),
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
        Assert.Contains(result.Errors, error => error is Error);
    }

    [Fact]
    public async Task Handle_Failure_CartDoesNotExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new PlaceOrder(
            Guid.NewGuid(),
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
        Assert.Contains(result.Errors, error => error is Error);
    }

    [Fact]
    public async Task Handle_Failure_InvalidPhoneNumber()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Setup product data via mock
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

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
                    SalePrice = 8.0m,
                    Quantity = 5,
                    ImageUrl = "https://test-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Create cart with items
        var cart = new Cart(Guid.NewGuid(), customerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        var command = new PlaceOrder(
            Guid.NewGuid(),
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "123", // Invalid phone number
            "COD",
            "Standard");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task Handle_Failure_InvalidShippingMethod()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Setup product data via mock
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

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
                    SalePrice = 8.0m,
                    Quantity = 5,
                    ImageUrl = "https://test-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Create cart with items
        var cart = new Cart(Guid.NewGuid(), customerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        var command = new PlaceOrder(
            Guid.NewGuid(),
            customerId,
            "123 Test St",
            "Test Ward",
            "Test District",
            "Test Province",
            "Test Country",
            "1234567890",
            "COD",
            "InvalidMethod"); // Invalid shipping method

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    private async Task<Product> CreateProductWithMultipleVariants()
    {
        // Create attributes first
        var createSizeAttrCmd = new CreateAttribute("Size", new[] { "Small", "Medium", "Large" });
        var sizeResult = await mediator.Send(createSizeAttrCmd);
        Assert.True(sizeResult.IsSuccess);

        var createColorAttrCmd = new CreateAttribute("Color", new[] { "Red", "Blue", "Green", "Yellow", "Black" });
        var colorResult = await mediator.Send(createColorAttrCmd);
        Assert.True(colorResult.IsSuccess);

        // Create product
        var productId = Guid.NewGuid();
        var createProductCmd = new CreateProduct(
            productId,
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            null);

        var productResult = await mediator.Send(createProductCmd);
        Assert.True(productResult.IsSuccess);

        // Add multiple variants with different attributes
        var sizes = new[] { "Small", "Medium", "Large" };
        var colors = new[] { "Red", "Blue", "Green" };
        var prices = new[] { 19.99m, 24.99m, 29.99m };

        for (int i = 0; i < 3; i++)
        {
            var attributes = new List<AttributeValue>
            {
                new AttributeValue("Size", sizes[i]),
                new AttributeValue("Color", colors[i])
            };

            var addVariantCmd = new AddVariantForProduct(
                productId,
                prices[i],
                faker.Random.Int(1, 100),
                null,
                null,
                attributes,
                null,
                null,
                null);

            var variantResult = await mediator.Send(addVariantCmd);
            Assert.True(variantResult.IsSuccess);
        }

        var product = await productRepository.GetByIdWithVariantsAsync(productId);
        return product!;
    }
}