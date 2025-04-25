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
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Add variant
        var sku = faker.Commerce.Ean13();
        var price = faker.Random.Decimal(10, 100);
        var initialQuantity = faker.Random.Int(1, 50);

        var addVariantCommand = new AddVariantForProduct(
            createProductResult.Value.Id,
            price,
            initialQuantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null
        );

        var addVariantResult = await mediator.Send(addVariantCommand);
        Assert.True(addVariantResult.IsSuccess);

        // Get product to access variant ID
        var product = await productRepository.GetByIdWithVariantsAsync(createProductResult.Value.Id);
        Assert.NotNull(product);
        Assert.Single(product.Variants);
        var variantId = product.Variants.First().Id;

        // Update quantity
        var newQuantity = initialQuantity + 50;
        var command = new UpdateQuantity(product.Id, variantId, newQuantity);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify quantity updated
        var updatedProduct = await productRepository.GetByIdWithVariantsAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Single(updatedProduct.Variants);
        Assert.Equal(newQuantity, updatedProduct.Variants.First().Quantity);
    }

    [Fact]
    public async Task UpdateQuantity_Failure_ProductNotFound()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();
        var command = new UpdateQuantity(nonExistentProductId, Guid.NewGuid(), 10);

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
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Try to update a non-existent variant
        var nonExistentVariantId = Guid.NewGuid();
        var command = new UpdateQuantity(createProductResult.Value.Id, nonExistentVariantId, 10);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task UpdateQuantity_Failure_InvalidQuantity()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Add variant
        var price = faker.Random.Decimal(10, 100);
        var initialQuantity = faker.Random.Int(1, 50);

        var addVariantCommand = new AddVariantForProduct(
            createProductResult.Value.Id,
            price,
            initialQuantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null
        );

        var addVariantResult = await mediator.Send(addVariantCommand);
        Assert.True(addVariantResult.IsSuccess);

        // Get product to access variant ID
        var product = await productRepository.GetByIdWithVariantsAsync(createProductResult.Value.Id);
        var variantId = product!.Variants.First().Id;

        // Update with invalid quantity
        var command = new UpdateQuantity(product.Id, variantId, -10);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task UpdateQuantity_Success_ZeroQuantity()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Add variant
        var sku = faker.Commerce.Ean13();
        var price = faker.Random.Decimal(10, 100);
        var initialQuantity = faker.Random.Int(1, 50);

        var addVariantCommand = new AddVariantForProduct(
            createProductResult.Value.Id,
            price,
            initialQuantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null
        );

        var addVariantResult = await mediator.Send(addVariantCommand);
        Assert.True(addVariantResult.IsSuccess);

        // Get product to access variant ID
        var product = await productRepository.GetByIdWithVariantsAsync(createProductResult.Value.Id);
        Assert.NotNull(product);
        Assert.Single(product.Variants);
        var variantId = product.Variants.First().Id;

        // Update quantity to zero (out of stock)
        var command = new UpdateQuantity(product.Id, variantId, 0);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify quantity updated
        var updatedProduct = await productRepository.GetByIdWithVariantsAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Single(updatedProduct.Variants);
        Assert.Equal(0, updatedProduct.Variants.First().Quantity);

        // Verify variant is marked as out of stock
        Assert.False(updatedProduct.Variants.First().InStock());
    }
}