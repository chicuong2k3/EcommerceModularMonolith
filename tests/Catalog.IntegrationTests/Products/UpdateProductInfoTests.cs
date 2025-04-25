namespace Catalog.IntegrationTests.Products;

public class UpdateProductInfoTests : IntegrationTestBase
{
    private readonly IProductRepository productRepository;

    public UpdateProductInfoTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
    }

    [Fact]
    public async Task UpdateProductInfo_Success_UpdateNameAndDescription()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var originalName = faker.Commerce.ProductName();
        var originalDescription = faker.Commerce.ProductDescription();
        await mediator.Send(new CreateProduct(
            productId,
            originalName, 
            originalDescription, 
            null));

        var newName = faker.Commerce.ProductName();
        var newDescription = faker.Commerce.ProductDescription();

        var command = new UpdateProductInfo(
            productId,
            newName,
            newDescription,
            null  // No category change
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var updatedProduct = await productRepository.GetByIdAsync(productId);
        Assert.NotNull(updatedProduct);
        Assert.Equal(newName, updatedProduct.Name);
        Assert.Equal(newDescription, updatedProduct.Description);
    }

    [Fact]
    public async Task UpdateProductInfo_Success_ChangeCategory()
    {
        // Arrange
        // Create original category
        var originalCategoryId = Guid.NewGuid();
        var originalCategoryName = faker.Commerce.Categories(1)[0];
        await mediator.Send(new CreateCategory(originalCategoryId, originalCategoryName, null));

        // Create product with original category
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        await mediator.Send(new CreateProduct(
            productId,
            productName,
            faker.Commerce.ProductDescription(),
            originalCategoryId));

        // Create new category
        var newCategoryId = Guid.NewGuid();
        var newCategoryName = faker.Commerce.Categories(1)[0] + " New";
        await mediator.Send(new CreateCategory(newCategoryId, newCategoryName, null));

        // Update product with new category
        var command = new UpdateProductInfo(
            productId,
            productName,  // Same name
            null,  // No description change
            newCategoryId  // New category
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var updatedProduct = await productRepository.GetByIdAsync(productId);
        Assert.NotNull(updatedProduct);
        Assert.Equal(newCategoryId, updatedProduct.CategoryId);
    }

    [Fact]
    public async Task UpdateProductInfo_Success_RemoveCategory()
    {
        // Arrange
        // Create category
        var categoryId = Guid.NewGuid();
        var categoryName = faker.Commerce.Categories(1)[0];
        await mediator.Send(new CreateCategory(categoryId, categoryName, null));

        // Create product with category
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        await mediator.Send(new CreateProduct(
            productId,
            productName,
            faker.Commerce.ProductDescription(),
            categoryId));

        // Update product to remove category
        var command = new UpdateProductInfo(
            productId,
            productName,  // Same name
            null,  // No description change
            null  // Remove category
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var updatedProduct = await productRepository.GetByIdAsync(productId);
        Assert.NotNull(updatedProduct);
        Assert.Null(updatedProduct.CategoryId);
    }

    [Fact]
    public async Task UpdateProductInfo_Failure_ProductNotFound()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();
        var command = new UpdateProductInfo(
            nonExistentProductId,
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task UpdateProductInfo_Failure_CategoryNotFound()
    {
        // Arrange
        // Create product without category
        var productId = Guid.NewGuid();
        await mediator.Send(new CreateProduct(
            productId,
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            null));

        // Try to update with non-existent category
        var nonExistentCategoryId = Guid.NewGuid();
        var command = new UpdateProductInfo(
            productId,
            faker.Commerce.ProductName(),
            null,
            nonExistentCategoryId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task UpdateProductInfo_Failure_EmptyName()
    {
        // Arrange
        var productId = Guid.NewGuid();
        await mediator.Send(new CreateProduct(
            productId,
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            null));

        var command = new UpdateProductInfo(
            productId,
            "", // Empty name should fail
            faker.Commerce.ProductDescription(),
            null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }
}