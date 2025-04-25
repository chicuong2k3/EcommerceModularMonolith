namespace Ordering.IntegrationTests.Orders;

public class OrderQueryHandlerTests : IntegrationTestBase
{
    private readonly IOrderRepository writeOrderRepository;
    private readonly Mock<IProductService> productServiceMock;

    public OrderQueryHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        writeOrderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();

        // Set up mock for IProductService
        productServiceMock = new Mock<IProductService>();

        // Configure the service provider to use the mock
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(productServiceMock.Object);
    }

    [Fact]
    public async Task GetOrderById_ReturnsOrder_WhenOrderExists()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var query = new GetOrderById(order.Id);

        // Act
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        var orderModel = result.Value;
        Assert.NotNull(orderModel);
        Assert.Equal(order.Id, orderModel.Id);
        Assert.Equal(order.CustomerId, orderModel.CustomerId);
        Assert.Equal(order.Status.ToString(), orderModel.Status);
        Assert.True((order.OrderDate - orderModel.OrderDate).Duration() < TimeSpan.FromMilliseconds(1));
        Assert.Equal(order.Total.Amount, orderModel.Total);
        Assert.Equal(order.Subtotal.Amount, orderModel.Subtotal);
        Assert.Equal(order.PaymentInfo.PaymentMethod.ToString(), orderModel.PaymentMethod);
        Assert.Equal(order.ShippingInfo.ShippingMethod.ToString(), orderModel.ShippingMethod);
        Assert.Equal(order.ShippingInfo.ShippingCosts.Amount, orderModel.ShippingCosts);
        Assert.Equal(order.ShippingInfo.PhoneNumber, orderModel.PhoneNumber);
        Assert.Equal(order.ShippingInfo.ShippingAddress.Street, orderModel.Street);
        Assert.Equal(order.ShippingInfo.ShippingAddress.Ward, orderModel.Ward);
        Assert.Equal(order.ShippingInfo.ShippingAddress.District, orderModel.District);
        Assert.Equal(order.ShippingInfo.ShippingAddress.Province, orderModel.Province);
        Assert.Equal(order.ShippingInfo.ShippingAddress.Country, orderModel.Country);

        // Verify items
        Assert.Single(orderModel.Items);
        var item = orderModel.Items[0];
        var orderItem = order.Items.First();
        Assert.Equal(orderItem.ProductId, item.ProductId);
        Assert.Equal(orderItem.ProductVariantId, item.ProductVariantId);
        Assert.Equal(orderItem.ProductName, item.ProductName);
        Assert.Equal(orderItem.Quantity, item.Quantity);
        Assert.Equal(orderItem.OriginalPrice.Amount, item.OriginalPrice);
        Assert.Equal(orderItem.SalePrice.Amount, item.SalePrice);
    }

    [Fact]
    public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var query = new GetOrderById(Guid.NewGuid());

        // Act
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.IsType<NotFoundError>(result.Errors.First());
    }

    [Fact]
    public async Task GetOrders_ReturnsPaginatedOrders_WithFiltering()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        await CreateMultipleTestOrdersAsync(customerId, 5);

        var query = new GetOrders(
            PageNumber: 1,
            PageSize: 3,
            CustomerId: customerId,
            OrderStatus: ((int)OrderStatus.Processing).ToString(),
            StartOrderDate: DateTime.UtcNow.AddDays(-1),
            EndOrderDate: DateTime.UtcNow.AddDays(1),
            MinTotal: 20m,
            MaxTotal: 100m,
            PaymentMethod: PaymentMethod.COD.ToString());

        // Act
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        var paginatedResult = result.Value;
        Assert.NotNull(paginatedResult);
        Assert.NotEmpty(paginatedResult.Data);
        Assert.Equal(1, paginatedResult.PageNumber);
        Assert.Equal(3, paginatedResult.PageSize);
        Assert.True(paginatedResult.TotalPages > 0);

        // Verify filtering
        foreach (var order in paginatedResult.Data)
        {
            Assert.Equal(customerId, order.CustomerId);
            Assert.Equal(OrderStatus.Processing.ToString(), order.Status);
            Assert.True(order.OrderDate >= query.StartOrderDate);
            Assert.True(order.OrderDate <= query.EndOrderDate);
            Assert.True(order.Total >= query.MinTotal);
            Assert.True(order.Total <= query.MaxTotal);
            Assert.Equal(PaymentMethod.COD.ToString(), order.PaymentMethod);
        }
    }

    [Fact]
    public async Task GetOrders_ReturnsEmptyList_WhenNoOrdersMatchCriteria()
    {
        // Arrange
        var query = new GetOrders(
            PageNumber: 1,
            PageSize: 10,
            CustomerId: Guid.NewGuid(),
            OrderStatus: null,
            StartOrderDate: null,
            EndOrderDate: null,
            MinTotal: null,
            MaxTotal: null,
            PaymentMethod: null);

        // Act
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        var paginatedResult = result.Value;
        Assert.NotNull(paginatedResult);
        Assert.Empty(paginatedResult.Data);
        Assert.Equal(0, paginatedResult.TotalRecords);
        Assert.Equal(0, paginatedResult.TotalPages);
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
            location,
            "1234567890",
            ShippingMethod.Standard).Value;

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
            Guid.NewGuid(),
            customerId,
            paymentInfo,
            shippingInfo,
            new List<OrderItem> { orderItem });

        var order = orderResult.Value;
        await writeOrderRepository.AddAsync(order);

        return order;
    }

    private async Task<List<Order>> CreateMultipleTestOrdersAsync(Guid customerId, int count)
    {
        var orders = new List<Order>();

        for (int i = 0; i < count; i++)
        {
            var productId = Guid.NewGuid();
            var variantId = Guid.NewGuid();

            var product = new ProductDto
            {
                Id = productId,
                Name = $"Test Product {i}",
                Variants = new List<ProductVariantDto>
                {
                    new ProductVariantDto
                    {
                        VariantId = variantId,
                        OriginalPrice = 29.99m + i,
                        SalePrice = 24.99m + i,
                        Quantity = 10,
                        ImageUrl = $"https://test-product-{i}.jpg",
                        Attributes = new Dictionary<string, string> { { "Color", "Blue" }, { "Size", "M" } }
                    }
                }
            };

            productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var location = Location.Create(
                "123 Test St",
                "Test Ward",
                "Test District",
                "Test Province",
                "Test Country").Value;

            var paymentInfo = PaymentInfo.Create("COD").Value;
            var shippingInfo = ShippingInfo.Create(
                location,
                "1234567890",
                ShippingMethod.Standard).Value;

            var orderItem = OrderItem.Create(
                product.Id,
                variantId,
                product.Name,
                1,
                Money.FromDecimal(29.99m + i).Value,
                Money.FromDecimal(24.99m + i).Value,
                $"https://test-product-{i}.jpg",
                "Color: Blue, Size: M").Value;

            var orderResult = Order.Create(
                Guid.NewGuid(),
                customerId,
                paymentInfo,
                shippingInfo,
                new List<OrderItem> { orderItem });

            var order = orderResult.Value;
            await writeOrderRepository.AddAsync(order);
            orders.Add(order);
        }

        return orders;
    }
}