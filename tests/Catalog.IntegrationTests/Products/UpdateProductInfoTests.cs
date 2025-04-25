namespace Catalog.IntegrationTests.Products;

public class UpdateProductInfoTests : IntegrationTestBase
{
    private readonly IProductRepository productRepository;

    public UpdateProductInfoTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
    }

    [Fact]
    public async Task UpdateProduct_Success_NameAndDescription()
    {
        // Arrange
        var originalName = faker.Commerce.ProductName();
        var originalDescription = faker.Commerce.ProductDescription();
        var createResult = await mediator.Send(new CreateProduct(originalName, originalDescription, null));
        Assert.True(createResult.IsSuccess);

        var productId = createResult.Value.Id;
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
    public async Task UpdateProduct_Success_ChangeCategory()
    {
        // Arrange
        // Create original category
        var originalCategoryName = faker.Commerce.Categories(1)[0];
        var originalCategoryResult = await mediator.Send(new CreateCategory(originalCategoryName, null));
        Assert.True(originalCategoryResult.IsSuccess);

        // Create product with original category
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(
            productName,
            faker.Commerce.ProductDescription(),
            originalCategoryResult.Value.Id));
        Assert.True(createResult.IsSuccess);

        // Create new category
        var newCategoryName = faker.Commerce.Categories(1)[0] + " New";
        var newCategoryResult = await mediator.Send(new CreateCategory(newCategoryName, null));
        Assert.True(newCategoryResult.IsSuccess);

        // Update product with new category
        var command = new UpdateProductInfo(
            createResult.Value.Id,
            productName,  // Same name
            null,  // No description change
            newCategoryResult.Value.Id  // New category
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var updatedProduct = await productRepository.GetByIdAsync(createResult.Value.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(newCategoryResult.Value.Id, updatedProduct.CategoryId);
    }

    [Fact]
    public async Task UpdateProduct_Success_RemoveCategory()
    {
        // Arrange
        // Create category
        var categoryName = faker.Commerce.Categories(1)[0];
        var categoryResult = await mediator.Send(new CreateCategory(categoryName, null));
        Assert.True(categoryResult.IsSuccess);

        // Create product with category
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(
            productName,
            faker.Commerce.ProductDescription(),
            categoryResult.Value.Id));
        Assert.True(createResult.IsSuccess);

        // Update product to remove category
        var command = new UpdateProductInfo(
            createResult.Value.Id,
            productName,  // Same name
            null,  // No description change
            null  // Remove category
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var updatedProduct = await productRepository.GetByIdAsync(createResult.Value.Id);
        Assert.NotNull(updatedProduct);
        Assert.Null(updatedProduct.CategoryId);
    }

    [Fact]
    public async Task UpdateProduct_Failure_ProductNotFound()
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
    public async Task UpdateProduct_Failure_CategoryNotFound()
    {
        // Arrange
        // Create product without category
        var createResult = await mediator.Send(new CreateProduct(
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            null));
        Assert.True(createResult.IsSuccess);

        // Try to update with non-existent category
        var nonExistentCategoryId = Guid.NewGuid();
        var command = new UpdateProductInfo(
            createResult.Value.Id,
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
    public async Task UpdateProduct_Failure_EmptyName()
    {
        // Arrange
        var createResult = await mediator.Send(new CreateProduct(
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            null));
        Assert.True(createResult.IsSuccess);

        var command = new UpdateProductInfo(
            createResult.Value.Id,
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