using Ordering.Application.Carts.Commands;
using Ordering.Domain.CartAggregate;
using Ordering.Domain.ProductAggregate;
using Ordering.IntegrationTests.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Ordering.IntegrationTests.Carts;

public class ClearCartHandlerTests : IntegrationTestBase
{
    private readonly ICartRepository cartRepository;
    private readonly IProductRepository productRepository;

    public ClearCartHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        cartRepository = serviceScope.ServiceProvider.GetRequiredService<ICartRepository>();
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
    }

    [Fact]
    public async Task Handle_ShouldClearAllItems_FromExistingCart()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create products
        var product1Id = Guid.NewGuid();
        var variant1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        var variant2Id = Guid.NewGuid();

        await productRepository.AddProductAsync(
            new Product(product1Id, variant1Id, "Product 1", 10.0m, 10, "imageUrl1", 8.0m, "description1"));

        await productRepository.AddProductAsync(
            new Product(product2Id, variant2Id, "Product 2", 15.0m, 15, "imageUrl2", 8.0m, "description2"));

        // Create cart with items
        var cart = new Cart(ownerId);
        await cart.AddItemAsync(product1Id, variant1Id, 2);
        await cart.AddItemAsync(product2Id, variant2Id, 3);
        await cartRepository.UpsertAsync(cart);

        // Verify cart has items initially
        var initialCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(initialCart);
        Assert.Equal(2, initialCart.Items.Count);

        // Create command
        var command = new ClearCart(ownerId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify cart is empty
        var clearedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(clearedCart);
        Assert.Empty(clearedCart.Items);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenCartIsAlreadyEmpty()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create empty cart
        var cart = new Cart(ownerId);
        await cartRepository.UpsertAsync(cart);

        // Verify cart is empty initially
        var initialCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(initialCart);
        Assert.Empty(initialCart.Items);

        // Create command
        var command = new ClearCart(ownerId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify cart remains empty
        var clearedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(clearedCart);
        Assert.Empty(clearedCart.Items);
    }

    [Fact]
    public async Task Handle_ShouldCreateEmptyCart_WhenCartDoesNotExist()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Verify cart doesn't exist initially
        var initialCart = await cartRepository.GetAsync(ownerId);
        Assert.Null(initialCart);

        // Create command
        var command = new ClearCart(ownerId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify empty cart was created
        var newCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(newCart);
        Assert.Empty(newCart.Items);
    }

    [Fact]
    public async Task Handle_ShouldPreserveCartId_WhenClearingExistingCart()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create cart with an item
        var cart = new Cart(ownerId);
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        await productRepository.AddProductAsync(
            new Product(productId, variantId, "Test Product", 10.0m, 10, "imageUrl", 8.0m, "description"));

        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        var originalCartId = cart.Id;

        // Create command
        var command = new ClearCart(ownerId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify cart ID was preserved
        var clearedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(clearedCart);
        Assert.Equal(originalCartId, clearedCart.Id);
    }
}