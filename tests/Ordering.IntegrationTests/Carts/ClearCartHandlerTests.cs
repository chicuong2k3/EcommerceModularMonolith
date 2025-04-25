using Ordering.Core.Persistence;
using Shared.Abstractions.Core;

namespace Ordering.IntegrationTests.Carts;

public class ClearCartHandlerTests : IntegrationTestBase
{
    private readonly ICartRepository cartRepository;
    private readonly OrderingDbContext dbContext;

    public ClearCartHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
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
    public async Task Handle_Success_ClearsExistingCart()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        // Create cart with items
        var cart = new Cart(Guid.NewGuid(), ownerId);
        await cart.AddItemAsync(productId, variantId, 1);
        await cartRepository.UpsertAsync(cart);

        // Verify cart has items initially
        var initialCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(initialCart);
        Assert.Single(initialCart.Items);

        // Create command
        var command = new ClearCart(ownerId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        var clearedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(clearedCart);
        Assert.Empty(clearedCart.Items);
    }

    [Fact]
    public async Task Handle_Success_ClearsEmptyCart()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Create empty cart
        var cart = new Cart(Guid.NewGuid(), ownerId);
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
        var clearedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(clearedCart);
        Assert.Empty(clearedCart.Items);
    }

    [Fact]
    public async Task Handle_Success_CreatesEmptyCart_WhenCartDoesNotExist()
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
        var newCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(newCart);
        Assert.Empty(newCart.Items);
    }

    [Fact]
    public async Task Handle_Success_PreservesCartId_WhenClearingExistingCart()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var cartId = Guid.NewGuid();

        // Create cart with known ID
        var cart = new Cart(cartId, ownerId);
        await cartRepository.UpsertAsync(cart);

        // Create command
        var command = new ClearCart(ownerId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        var clearedCart = await cartRepository.GetAsync(ownerId);
        Assert.NotNull(clearedCart);
        Assert.Equal(cartId, clearedCart.Id);
        Assert.Empty(clearedCart.Items);
    }
}