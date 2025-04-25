namespace Catalog.IntegrationTests.Products;

public class CreateProductTests : IntegrationTestBase
{
    private readonly IProductRepository productRepository;

    public CreateProductTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
    }

    [Fact]
    public async Task CreateProduct_Success_NoCategory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var command = new CreateProduct(productId, productName, description, null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var savedProduct = await productRepository.GetByIdAsync(productId);
        Assert.NotNull(savedProduct);
        Assert.Equal(productName, savedProduct.Name);
        Assert.Equal(description, savedProduct.Description);
        Assert.Null(savedProduct.CategoryId);
        Assert.Empty(savedProduct.Variants);
    }

    [Fact]
    public async Task CreateProduct_Success_WithCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryName = faker.Commerce.Categories(1)[0];
        await mediator.Send(new CreateCategory(categoryId, categoryName, null));

        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var command = new CreateProduct(productId, productName, description, categoryId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var savedProduct = await productRepository.GetByIdAsync(productId);
        Assert.NotNull(savedProduct);
        Assert.Equal(productName, savedProduct.Name);
        Assert.Equal(description, savedProduct.Description);
        Assert.Equal(categoryId, savedProduct.CategoryId);
    }

    [Fact]
    public async Task CreateProduct_Failure_EmptyName()
    {
        // Arrange
        var command = new CreateProduct(Guid.NewGuid(), "", faker.Commerce.ProductDescription(), null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task CreateProduct_Failure_NonExistentCategory()
    {
        // Arrange
        var nonExistentCategoryId = Guid.NewGuid();
        var command = new CreateProduct(
            Guid.NewGuid(),
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            nonExistentCategoryId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }
}