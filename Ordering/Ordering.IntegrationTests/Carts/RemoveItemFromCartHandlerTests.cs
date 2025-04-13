using Ordering.Application.Carts.Commands;
using Ordering.Domain.CartAggregate;
using Ordering.IntegrationTests.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Common.Domain;

namespace Ordering.IntegrationTests.Carts;

public class RemoveItemFromCartHandlerTests : IntegrationTestBase
{
    private readonly ICartRepository cartRepository;

    public RemoveItemFromCartHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        cartRepository = serviceScope.ServiceProvider.GetRequiredService<ICartRepository>();
    }

    [Fact]
    public async Task Handle_ShouldRemoveItem_FromExistingCart()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create product identifiers
        var product1Id = Guid.NewGuid();
        var variant1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        var variant2Id = Guid.NewGuid();

        // Create cart with items
        var cart = new Cart(ownerId);
        await cart.AddItemAsync(product1Id, variant1Id, 2);
        await cart.AddItemAsync(product2Id, variant2Id, 3);
        await cartRepository.UpsertAsync(cart);

        // Verify cart has both items initially
        var initialCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(initialCart);
        Assert.Equal(2, initialCart.Items.Count);

        // Create command to remove the first item
        var command = new RemoveItemFromCart(ownerId, variant1Id, 2);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify item was removed
        var updatedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(updatedCart);
        Assert.Single(updatedCart.Items);

        // Check that the remaining item is the second one
        var remainingItem = updatedCart.Items.Single();
        Assert.Equal(product2Id, remainingItem.ProductId);
        Assert.Equal(variant2Id, remainingItem.ProductVariantId);
        Assert.Equal(3, remainingItem.Quantity);
    }

    [Fact]
    public async Task Handle_ShouldRemoveLastItem_AndLeaveCartEmpty()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create product
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Create cart with one item
        var cart = new Cart(ownerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        // Verify cart has the item initially
        var initialCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(initialCart);
        Assert.Single(initialCart.Items);

        // Create command
        var command = new RemoveItemFromCart(ownerId, variantId, 1);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify cart is empty but still exists
        var updatedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(updatedCart);
        Assert.Empty(updatedCart.Items);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCartDoesNotExist()
    {
        // Arrange
        var nonExistentOwnerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Verify cart doesn't exist
        var cart = await cartRepository.GetAsync(nonExistentOwnerId);
        Assert.Null(cart);

        // Create command
        var command = new RemoveItemFromCart(nonExistentOwnerId, variantId, 1);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is NotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenItemNotInCart()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create product identifiers
        var existingProductId = Guid.NewGuid();
        var existingVariantId = Guid.NewGuid();

        // Create cart with the product
        var cart = new Cart(ownerId);
        await cart.AddItemAsync(existingProductId, existingVariantId, 1);
        await cartRepository.UpsertAsync(cart);

        // Create non-existent product/variant IDs
        var nonExistentVariantId = Guid.NewGuid();

        // Create command to remove non-existent item
        var command = new RemoveItemFromCart(ownerId, nonExistentVariantId, 1);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e is NotFoundError);

        // Verify cart was not modified
        var updatedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(updatedCart);
        Assert.Single(updatedCart.Items);
        Assert.Equal(existingProductId, updatedCart.Items.First().ProductId);
    }

    [Fact]
    public async Task Handle_ShouldPreserveCartId_WhenRemovingItems()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create product
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Create cart with item
        var cart = new Cart(ownerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        var originalCartId = cart.Id;

        // Create command
        var command = new RemoveItemFromCart(ownerId, variantId, 1);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify cart ID was preserved
        var updatedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(updatedCart);
        Assert.Equal(originalCartId, updatedCart.Id);
    }
}