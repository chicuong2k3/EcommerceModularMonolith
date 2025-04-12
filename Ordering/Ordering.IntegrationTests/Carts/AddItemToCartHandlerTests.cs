using Ordering.Application.Carts.Commands;
using Ordering.Domain.CartAggregate;
using Ordering.Domain.ProductAggregate;
using Ordering.IntegrationTests.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Common.Domain;
using Ordering.Domain.OrderAggregate.Errors;

namespace Ordering.IntegrationTests.Carts;

public class AddItemToCartHandlerTests : IntegrationTestBase
{
    private readonly ICartRepository cartRepository;
    private readonly IProductRepository productRepository;

    public AddItemToCartHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        cartRepository = serviceScope.ServiceProvider.GetRequiredService<ICartRepository>();
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
    }

    [Fact]
    public async Task Handle_ShouldCreateNewCart_WhenCartDoesNotExist()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var quantity = 2;

        // Create a product in the repository so it exists
        var product = new Product(
            productId,
            variantId,
            "Test Product",
            10.0m,
            5,
            "https://test-image.jpg",
            8.0m,
            "Test product description");
        await productRepository.AddProductAsync(product);

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

        // Create initial cart with one item
        var initialProductId = Guid.NewGuid();
        var initialVariantId = Guid.NewGuid();
        var initialQuantity = 1;

        // Create the product in repository
        var initialProduct = new Product(
            initialProductId,
            initialVariantId,
            "Initial Product",
            10.0m,
            5,
            "https://initial-product.jpg",
            8.0m,
            "Initial product description");
        await productRepository.AddProductAsync(initialProduct);

        var initialCart = new Cart(ownerId);
        await initialCart.AddItemAsync(initialProductId, initialVariantId, initialQuantity);
        await cartRepository.UpsertAsync(initialCart);

        // New item to add
        var newProductId = Guid.NewGuid();
        var newVariantId = Guid.NewGuid();
        var newQuantity = 3;

        // Create the new product in repository
        var newProduct = new Product(
            newProductId,
            newVariantId,
            "New Product",
            15.0m,
            10,
            "https://new-product.jpg",
            12.0m,
            "New product description");
        await productRepository.AddProductAsync(newProduct);

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

        // Create the product in repository
        var product = new Product(
            productId,
            variantId,
            "Test Product",
            10.0m,
            10,
            "https://test-product.jpg",
            8.0m,
            "Test product description");
        await productRepository.AddProductAsync(product);

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

        // Create products in repository
        await productRepository.AddProductAsync(new Product(
            firstProductId,
            firstVariantId,
            "First Product",
            10.0m,
            5,
            "https://first-product.jpg",
            5.0m,
            "First product description"));

        await productRepository.AddProductAsync(new Product(
            secondProductId,
            secondVariantId,
            "Second Product",
            15.0m,
            8,
            "https://second-product.jpg",
            7.0m,
            "Second product description"));

        await productRepository.AddProductAsync(new Product(
            thirdProductId,
            thirdVariantId,
            "Third Product",
            20.0m,
            12,
            "https://third-product.jpg",
            10.0m,
            "Third product description"));

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

        // Verify all items are in the cart
        Assert.Equal(3, persistedCart.Items.Count);

        // Check each item is present with correct quantity
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
    public async Task Handle_ShouldPreserveCartId_WhenAddingToExistingCart()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create initial cart
        var cart = new Cart(ownerId);
        await cartRepository.UpsertAsync(cart);
        var originalCartId = cart.Id;

        // Create product
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        await productRepository.AddProductAsync(new Product(
            productId,
            variantId,
            "Test Product",
            10.0m,
            5,
            "https://test-product.jpg",
            8.0m,
            "Test product description"));

        // Add item to cart
        var command = new AddItemToCart(ownerId,
            new List<AddItemDto> { new AddItemDto(productId, variantId, 1) });

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify cart ID stayed the same
        var persistedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(persistedCart);
        Assert.Equal(originalCartId, persistedCart.Id);
    }

    [Fact]
    public async Task Handle_ShouldAddAllItemsTogether_WhenSomeItemsExistAndSomeDont()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create initial cart with one item
        var existingProductId = Guid.NewGuid();
        var existingVariantId = Guid.NewGuid();
        var initialQuantity = 2;

        // Create products in repository
        await productRepository.AddProductAsync(new Product(
            existingProductId,
            existingVariantId,
            "Existing Product",
            10.0m,
            10,
            "https://existing-product.jpg",
            8.0m,
            "Existing product description"));

        var cart = new Cart(ownerId);
        await cart.AddItemAsync(existingProductId, existingVariantId, initialQuantity);
        await cartRepository.UpsertAsync(cart);

        // Add additional quantity to existing item and add a new item
        var additionalQuantity = 3;
        var newProductId = Guid.NewGuid();
        var newVariantId = Guid.NewGuid();
        var newQuantity = 1;

        // Create new product in repository
        await productRepository.AddProductAsync(new Product(
            newProductId,
            newVariantId,
            "New Product",
            15.0m,
            5,
            "https://new-product.jpg",
            12.0m,
            "New product description"));

        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(existingProductId, existingVariantId, additionalQuantity),
            new AddItemDto(newProductId, newVariantId, newQuantity)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var persistedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(persistedCart);

        // Should have two items total
        Assert.Equal(2, persistedCart.Items.Count);

        // Check existing item has increased quantity
        var existingItem = persistedCart.Items.FirstOrDefault(i =>
            i.ProductId == existingProductId && i.ProductVariantId == existingVariantId);
        Assert.NotNull(existingItem);
        Assert.Equal(initialQuantity + additionalQuantity, existingItem.Quantity);

        // Check new item exists
        var newItem = persistedCart.Items.FirstOrDefault(i =>
            i.ProductId == newProductId && i.ProductVariantId == newVariantId);
        Assert.NotNull(newItem);
        Assert.Equal(newQuantity, newItem.Quantity);
    }

    [Fact]
    public async Task Handle_ShouldFailValidation_WhenProductDoesNotExist()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var nonExistentProductId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var quantity = 2;

        // Make sure the product doesn't exist in repository
        var product = await productRepository.GetProductAsync(nonExistentProductId, variantId);
        Assert.Null(product); // Verify product doesn't exist

        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(nonExistentProductId, variantId, quantity)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is NotFoundError);

        // Verify cart was not created or was empty
        var cart = await cartRepository.GetAsync(ownerId);
        if (cart != null)
        {
            Assert.Empty(cart.Items);
        }
    }

    [Fact]
    public async Task Handle_ShouldFailValidation_WhenProductVariantDoesNotExist()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create a product but use a non-existent variant ID
        var productId = Guid.NewGuid();
        var existingVariantId = Guid.NewGuid();
        var nonExistentVariantId = Guid.NewGuid();

        // Create product in repository with specific variant
        var product = new Product(
            productId,
            existingVariantId,
            "Test Product",
            10.0m,
            5,
            "https://test-product.jpg",
            8.0m,
            "Test product description");
        await productRepository.AddProductAsync(product);

        // Verify product exists but with different variant
        var existingProduct = await productRepository.GetProductAsync(productId, existingVariantId);
        Assert.NotNull(existingProduct);

        // Verify the variant we want to add doesn't exist
        var nonExistentVariant = await productRepository.GetProductAsync(productId, nonExistentVariantId);
        Assert.Null(nonExistentVariant);

        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(productId, nonExistentVariantId, 1)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is NotFoundError);

        // Verify cart was not created or was empty
        var cart = await cartRepository.GetAsync(ownerId);
        if (cart != null)
        {
            Assert.Empty(cart.Items);
        }
    }

    [Fact]
    public async Task Handle_ShouldFailValidation_WhenMultipleProductsIncludeNonExistentOne()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create one valid product
        var existingProductId = Guid.NewGuid();
        var existingVariantId = Guid.NewGuid();

        await productRepository.AddProductAsync(new Product(
            existingProductId,
            existingVariantId,
            "Existing Product",
            10.0m,
            5,
            "https://existing-product.jpg",
            8.0m,
            "Existing product description"));

        // Create a non-existent product ID
        var nonExistentProductId = Guid.NewGuid();
        var nonExistentVariantId = Guid.NewGuid();

        // Make sure the product doesn't exist
        var nonExistentProduct = await productRepository.GetProductAsync(nonExistentProductId, nonExistentVariantId);
        Assert.Null(nonExistentProduct);

        // Add both products to cart
        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(existingProductId, existingVariantId, 1),
            new AddItemDto(nonExistentProductId, nonExistentVariantId, 2)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is NotFoundError);

        // Verify cart does not contain any items (should fail atomically)
        var cart = await cartRepository.GetAsync(ownerId);
        if (cart != null)
        {
            Assert.Empty(cart.Items);
        }
    }
    [Fact]
    public async Task Handle_ShouldFailValidation_WhenRequestedQuantityExceedsStock()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var availableStock = 5;
        var requestedQuantity = availableStock + 3; // Exceeds available stock

        // Create product with limited stock
        await productRepository.AddProductAsync(new Product(
            productId,
            variantId,
            "Low Stock Product",
            10.0m,
            availableStock,
            "https://low-stock-product.jpg",
            8.0m,
            "Low stock product description"));

        // Verify product exists with correct stock
        var existingProduct = await productRepository.GetProductAsync(productId, variantId);
        Assert.NotNull(existingProduct);
        Assert.Equal(availableStock, existingProduct.Quantity);

        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(productId, variantId, requestedQuantity)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is OutOfStockError);

        // Verify cart was not created or was empty
        var cart = await cartRepository.GetAsync(ownerId);
        if (cart != null)
        {
            Assert.Empty(cart.Items);
        }
    }
    [Fact]
    public async Task Handle_ShouldFailValidation_WhenAnyItemInMultipleItemsRequestExceedsStock()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create first product with plenty of stock
        var product1Id = Guid.NewGuid();
        var variant1Id = Guid.NewGuid();
        var quantity1 = 3;
        var availableStock1 = 10;

        // Create second product with limited stock
        var product2Id = Guid.NewGuid();
        var variant2Id = Guid.NewGuid();
        var quantity2 = 8; // Exceeds available stock
        var availableStock2 = 5;

        await productRepository.AddProductAsync(new Product(
            product1Id,
            variant1Id,
            "Well Stocked Product",
            10.0m,
            availableStock1,
            "https://well-stocked-product.jpg",
            8.0m,
            "Well stocked product description"));

        await productRepository.AddProductAsync(new Product(
            product2Id,
            variant2Id,
            "Limited Stock Product",
            15.0m,
            availableStock2,
            "https://limited-stock-product.jpg",
            8.0m,
            "Limited stock product description"));

        // Add both items
        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(product1Id, variant1Id, quantity1),
            new AddItemDto(product2Id, variant2Id, quantity2)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is OutOfStockError);

        // Verify cart does not contain any items (should fail atomically)
        var cart = await cartRepository.GetAsync(ownerId);
        if (cart != null)
        {
            Assert.Empty(cart.Items);
        }
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenRequestedQuantityEqualsAvailableStock()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var availableStock = 5;
        var requestedQuantity = availableStock; // Exactly the available stock

        // Create product with exact stock
        await productRepository.AddProductAsync(new Product(
            productId,
            variantId,
            "Exact Stock Product",
            10.0m,
            availableStock,
            "https://exact-stock-product.jpg",
            8.0m,
            "Exact stock product description"));

        var itemsToAdd = new List<AddItemDto>
        {
            new AddItemDto(productId, variantId, requestedQuantity)
        };

        var command = new AddItemToCart(ownerId, itemsToAdd);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify item was added with the requested quantity
        var persistedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(persistedCart);
        Assert.Single(persistedCart.Items);
        Assert.Equal(requestedQuantity, persistedCart.Items.First().Quantity);
    }
}