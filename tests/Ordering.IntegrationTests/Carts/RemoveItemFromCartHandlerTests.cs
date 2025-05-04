using Ordering.Core.Persistence;
using Shared.Abstractions.Core;

namespace Ordering.IntegrationTests.Carts;

public class RemoveItemFromCartHandlerTests : IntegrationTestBase
{
    private readonly ICartRepository cartRepository;
    private readonly OrderingDbContext dbContext;

    public RemoveItemFromCartHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        cartRepository = serviceScope.ServiceProvider.GetRequiredService<ICartRepository>();
        dbContext = serviceScope.ServiceProvider.GetRequiredService<OrderingDbContext>();

        // Clear any existing carts to prevent conflicts
        ClearDatabase().GetAwaiter().GetResult();
    }

    private async Task ClearDatabase()
    {
        // Remove any existing carts
        var existingCarts = await dbContext.Set<Cart>().ToListAsync();
        if (existingCarts.Any())
        {
            dbContext.Set<Cart>().RemoveRange(existingCarts);
            await dbContext.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task Handle_Success_RemovesItem_WhenQuantityMatchesExactly()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Create cart with one item
        var cart = new Cart(Guid.NewGuid(), ownerId);
        cart.AddItem(productId, variantId, 1);
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

        var updatedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(updatedCart);
        Assert.Empty(updatedCart.Items);
    }

    [Fact]
    public async Task Handle_Success_ReducesQuantity_WhenRemovingPartialAmount()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Create cart with item quantity of 3
        var cart = new Cart(Guid.NewGuid(), ownerId);
        cart.AddItem(productId, variantId, 3);
        await cartRepository.UpsertAsync(cart);

        // Verify initial quantity
        var initialCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(initialCart);
        Assert.Single(initialCart.Items);
        Assert.Equal(3, initialCart.Items.First().Quantity);

        // Create command to remove 2 items
        var command = new RemoveItemFromCart(ownerId, variantId, 2);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var updatedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(updatedCart);
        Assert.Single(updatedCart.Items);
        Assert.Equal(1, updatedCart.Items.First().Quantity);
    }

    [Fact]
    public async Task Handle_Failure_CartDoesNotExist()
    {
        // Arrange
        var nonExistentOwnerId = Guid.NewGuid();
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
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task Handle_Failure_ItemNotInCart()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var nonExistentVariantId = Guid.NewGuid();

        // Create cart with one item
        var cart = new Cart(Guid.NewGuid(), ownerId);
        cart.AddItem(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        // Create command with non-existent variant ID
        var command = new RemoveItemFromCart(ownerId, nonExistentVariantId, 1);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);

        // Verify cart still has the original item
        var unchangedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(unchangedCart);
        Assert.Single(unchangedCart.Items);
        Assert.Equal(variantId, unchangedCart.Items.First().ProductVariantId);
    }

    [Fact]
    public async Task Handle_Failure_RemovingMoreThanAvailable()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Create cart with item quantity of 2
        var cart = new Cart(Guid.NewGuid(), ownerId);
        cart.AddItem(productId, variantId, 2);
        await cartRepository.UpsertAsync(cart);

        // Create command to remove 3 items
        var command = new RemoveItemFromCart(ownerId, variantId, 3);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);

        // Verify cart item quantity remains unchanged
        var unchangedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(unchangedCart);
        Assert.Single(unchangedCart.Items);
        Assert.Equal(2, unchangedCart.Items.First().Quantity);
    }
}