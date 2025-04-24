

namespace Ordering.IntegrationTests.Carts;

public class AddItemToCartHandlerTests : IntegrationTestBase
{
    private readonly ICartRepository cartRepository;
    private readonly Mock<IProductService> productServiceMock;

    public AddItemToCartHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
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
        cartRepository = serviceScope.ServiceProvider.GetRequiredService<ICartRepository>();
    }

    [Fact]
    public async Task Handle_ShouldCreateNewCart_WhenCartDoesNotExist()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var quantity = 2;

        // Set up mock product
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
                    ImageUrl = "https://test-image.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Blue" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(productId, variantId, quantity)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify cart persistence
        var persistedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(persistedCart);
        Assert.Single(persistedCart.Items);

        var persistedItem = persistedCart.Items.First();
        Assert.Equal(productId, persistedItem.ProductId);
        Assert.Equal(variantId, persistedItem.ProductVariantId);
        Assert.Equal(quantity, persistedItem.Quantity);
    }

    [Fact]
    public async Task Handle_ShouldAddNewItemToExistingCart_WhenCartAlreadyExists()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Set up initial product
        var initialProductId = Guid.NewGuid();
        var initialVariantId = Guid.NewGuid();
        var initialQuantity = 1;

        var initialProduct = new ProductDto
        {
            Id = initialProductId,
            Name = "Initial Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = initialVariantId,
                    OriginalPrice = 10.0m,
                    SalePrice = 8.0m,
                    Quantity = 5,
                    ImageUrl = "https://initial-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Size", "S" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(initialProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(initialProduct);

        // Create initial cart with one item
        var initialCart = new Cart(ownerId);
        await initialCart.AddItemAsync(initialProductId, initialVariantId, initialQuantity);
        await cartRepository.UpsertAsync(initialCart);

        // Set up new product
        var newProductId = Guid.NewGuid();
        var newVariantId = Guid.NewGuid();
        var newQuantity = 3;

        var newProduct = new ProductDto
        {
            Id = newProductId,
            Name = "New Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = newVariantId,
                    OriginalPrice = 15.0m,
                    SalePrice = 12.0m,
                    Quantity = 10,
                    ImageUrl = "https://new-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Green" }, { "Size", "L" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(newProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newProduct);

        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(newProductId, newVariantId, newQuantity)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var persistedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(persistedCart);

        // Verify cart has both items
        Assert.Equal(ownerId, persistedCart.OwnerId);
        Assert.Equal(2, persistedCart.Items.Count);

        // First item should still be there
        Assert.Contains(persistedCart.Items, item =>
            item.ProductId == initialProductId &&
            item.ProductVariantId == initialVariantId &&
            item.Quantity == initialQuantity);

        // New item should be added
        Assert.Contains(persistedCart.Items, item =>
            item.ProductId == newProductId &&
            item.ProductVariantId == newVariantId &&
            item.Quantity == newQuantity);
    }

    [Fact]
    public async Task Handle_ShouldIncreaseQuantity_WhenAddingExistingItemToCart()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var initialQuantity = 2;
        var additionalQuantity = 3;
        var expectedFinalQuantity = initialQuantity + additionalQuantity;

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
                    OriginalPrice = 10.0m,
                    SalePrice = 8.0m,
                    Quantity = 10,
                    ImageUrl = "https://test-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Blue" }, { "Size", "XL" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Add initial item
        var initialCart = new Cart(ownerId);
        await initialCart.AddItemAsync(productId, variantId, initialQuantity);
        await cartRepository.UpsertAsync(initialCart);

        // Add the same item again with additional quantity
        var command = new AddItemToCart(ownerId,
            new List<AddItemDto> { new AddItemDto(productId, variantId, additionalQuantity) });

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var persistedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(persistedCart);
        Assert.Single(persistedCart.Items);
        Assert.Equal(expectedFinalQuantity, persistedCart.Items.First().Quantity);
        Assert.Equal(ownerId, persistedCart.OwnerId);
    }

    [Fact]
    public async Task Handle_ShouldAddMultipleItems_InSingleRequest()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        var firstProductId = Guid.NewGuid();
        var firstVariantId = Guid.NewGuid();
        var firstQuantity = 1;

        var secondProductId = Guid.NewGuid();
        var secondVariantId = Guid.NewGuid();
        var secondQuantity = 2;

        var thirdProductId = Guid.NewGuid();
        var thirdVariantId = Guid.NewGuid();
        var thirdQuantity = 3;

        // Set up product mocks
        var firstProduct = new ProductDto
        {
            Id = firstProductId,
            Name = "First Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = firstVariantId,
                    OriginalPrice = 10.0m,
                    SalePrice = 5.0m,
                    Quantity = 5,
                    ImageUrl = "https://first-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Red" }, { "Size", "S" } }
                }
            }
        };

        var secondProduct = new ProductDto
        {
            Id = secondProductId,
            Name = "Second Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = secondVariantId,
                    OriginalPrice = 15.0m,
                    SalePrice = 7.0m,
                    Quantity = 8,
                    ImageUrl = "https://second-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Blue" }, { "Size", "M" } }
                }
            }
        };

        var thirdProduct = new ProductDto
        {
            Id = thirdProductId,
            Name = "Third Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = thirdVariantId,
                    OriginalPrice = 20.0m,
                    SalePrice = 10.0m,
                    Quantity = 12,
                    ImageUrl = "https://third-product.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Green" }, { "Size", "L" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(firstProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(firstProduct);
        productServiceMock.Setup(s => s.GetProductByIdAsync(secondProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(secondProduct);
        productServiceMock.Setup(s => s.GetProductByIdAsync(thirdProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(thirdProduct);

        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(firstProductId, firstVariantId, firstQuantity),
            new AddItemDto(secondProductId, secondVariantId, secondQuantity),
            new AddItemDto(thirdProductId, thirdVariantId, thirdQuantity)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var persistedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(persistedCart);
        Assert.Equal(3, persistedCart.Items.Count);

        // Verify each item was added correctly
        Assert.Contains(persistedCart.Items, item =>
            item.ProductId == firstProductId &&
            item.ProductVariantId == firstVariantId &&
            item.Quantity == firstQuantity);

        Assert.Contains(persistedCart.Items, item =>
            item.ProductId == secondProductId &&
            item.ProductVariantId == secondVariantId &&
            item.Quantity == secondQuantity);

        Assert.Contains(persistedCart.Items, item =>
            item.ProductId == thirdProductId &&
            item.ProductVariantId == thirdVariantId &&
            item.Quantity == thirdQuantity);
    }

    [Fact]
    public async Task Handle_ShouldFailValidation_WhenProductDoesNotExist()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var nonExistentProductId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var quantity = 2;

        // Setup product service mock to return null for nonExistentProductId
        productServiceMock.Setup(s => s.GetProductByIdAsync(nonExistentProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(nonExistentProductId, variantId, quantity)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailed);
        Assert.IsType<NotFoundError>(result.Errors.First());
        Assert.Contains($"Product with id {nonExistentProductId} not found", result.Errors.First().Message);

        // Verify no cart was created
        var persistedCart = await cartRepository.GetAsync(ownerId);
        Assert.Null(persistedCart);
    }

    [Fact]
    public async Task Handle_ShouldFailValidation_WhenProductVariantDoesNotExist()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var existingVariantId = Guid.NewGuid();
        var nonExistentVariantId = Guid.NewGuid();
        var quantity = 2;

        // Set up product with a specific variant
        var product = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Variants = new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    VariantId = existingVariantId, // Different variant ID
                    OriginalPrice = 10.0m,
                    SalePrice = 8.0m,
                    Quantity = 5,
                    ImageUrl = "https://test-image.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Blue" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Try to add a different variant that doesn't exist
        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(productId, nonExistentVariantId, quantity)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailed);
        Assert.IsType<NotFoundError>(result.Errors.First());
        Assert.Contains($"Product variant with id {nonExistentVariantId} not found", result.Errors.First().Message);

        // Verify no cart was created
        var persistedCart = await cartRepository.GetAsync(ownerId);
        Assert.Null(persistedCart);
    }

    [Fact]
    public async Task Handle_ShouldFailValidation_WhenQuantityExceedsAvailableStock()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var availableQuantity = 5;
        var requestedQuantity = 10; // More than available

        // Set up product with limited quantity
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
                    Quantity = availableQuantity, // Only 5 items in stock
                    ImageUrl = "https://test-image.jpg",
                    Attributes = new Dictionary<string, string> { { "Color", "Blue" }, { "Size", "M" } }
                }
            }
        };

        productServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(productId, variantId, requestedQuantity)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailed);
        Assert.IsType<Error>(result.Errors.First());

        // Verify no cart was created
        var persistedCart = await cartRepository.GetAsync(ownerId);
        Assert.Null(persistedCart);
    }
}