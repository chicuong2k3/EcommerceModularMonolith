namespace Catalog.IntegrationTests.Products;

public class DeleteVariantTests : IntegrationTestBase
{
    private readonly IWriteProductRepository productRepository;

    public DeleteVariantTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IWriteProductRepository>();
    }

    [Fact]
    public async Task DeleteVariant_Success()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Add a variant
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);

        var addVariantCommand = new AddVariantForProduct(
            createProductResult.Value.Id,
            price,
            quantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null
        );

        var addVariantResult = await mediator.Send(addVariantCommand);
        Assert.True(addVariantResult.IsSuccess);

        // Get the product with variant
        var product = await productRepository.GetByIdWithVariantsAsync(createProductResult.Value.Id);
        Assert.NotNull(product);
        Assert.Single(product.Variants);
        var variantId = product.Variants.First().Id;

        // Delete the variant
        var command = new DeleteVariant(product.Id, variantId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify variant was deleted
        var updatedProduct = await productRepository.GetByIdWithVariantsAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Empty(updatedProduct.Variants);
    }

    [Fact]
    public async Task DeleteVariant_Failure_ProductNotFound()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var command = new DeleteVariant(nonExistentProductId, variantId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task DeleteVariant_Failure_VariantNotFound()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Use a non-existent variant ID
        var nonExistentVariantId = Guid.NewGuid();
        var command = new DeleteVariant(createProductResult.Value.Id, nonExistentVariantId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task DeleteVariant_LastVariant_ReturnsSuccess()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Add a variant
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);

        var addVariantCommand = new AddVariantForProduct(
            createProductResult.Value.Id,
            price,
            quantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null
        );

        var addVariantResult = await mediator.Send(addVariantCommand);
        Assert.True(addVariantResult.IsSuccess);

        // Get the product with variant
        var product = await productRepository.GetByIdWithVariantsAsync(createProductResult.Value.Id);
        var variantId = product.Variants.First().Id;

        // Delete the variant
        var command = new DeleteVariant(product.Id, variantId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify product still exists but has no variants
        var updatedProduct = await productRepository.GetByIdWithVariantsAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Empty(updatedProduct.Variants);
    }
}