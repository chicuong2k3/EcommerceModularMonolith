namespace Catalog.IntegrationTests.Products;

public class UpdateQuantityTests : IntegrationTestBase
{
    private readonly IProductRepository productRepository;

    public UpdateQuantityTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
    }

    [Fact]
    public async Task UpdateQuantity_Success()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Add variant
        var price = faker.Random.Decimal(10, 100);
        var initialQuantity = faker.Random.Int(1, 100);
        var addVariantResult = await mediator.Send(new AddVariantForProduct(
            productId,
            price,
            initialQuantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null));
        Assert.True(addVariantResult.IsSuccess);

        // Get the variant ID
        var product = await productRepository.GetByIdWithVariantsAsync(productId);
        var variantId = product.Variants.First().Id;

        var newQuantity = faker.Random.Int(1, 100);
        var command = new UpdateQuantity(productId, variantId, newQuantity);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify quantity was updated
        var updatedProduct = await productRepository.GetByIdWithVariantsAsync(productId);
        Assert.NotNull(updatedProduct);
        Assert.Single(updatedProduct.Variants);
        Assert.Equal(newQuantity, updatedProduct.Variants.First().Quantity);
    }

    [Fact]
    public async Task UpdateQuantity_Failure_ProductNotFound()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();
        var command = new UpdateQuantity(nonExistentProductId, Guid.NewGuid(), faker.Random.Int(1, 100));

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task UpdateQuantity_Failure_VariantNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        var command = new UpdateQuantity(productId, Guid.NewGuid(), faker.Random.Int(1, 100));

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task UpdateQuantity_Failure_NegativeQuantity()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Add variant
        var price = faker.Random.Decimal(10, 100);
        var initialQuantity = faker.Random.Int(1, 100);
        var addVariantResult = await mediator.Send(new AddVariantForProduct(
            productId,
            price,
            initialQuantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null));
        Assert.True(addVariantResult.IsSuccess);

        // Get the variant ID
        var product = await productRepository.GetByIdWithVariantsAsync(productId);
        var variantId = product.Variants.First().Id;

        var command = new UpdateQuantity(productId, variantId, -1);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }
}