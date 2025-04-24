namespace Ordering.IntegrationTests.Carts;

public class ClearCartHandlerTests : IntegrationTestBase
{
    private readonly ICartRepository cartRepository;

    public ClearCartHandlerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        cartRepository = serviceScope.ServiceProvider.GetRequiredService<ICartRepository>();
    }

    [Fact]
    public async Task Handle_ShouldClearAllItems_FromExistingCart()
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