namespace Catalog.IntegrationTests.Products;

public class DeleteProductTests : IntegrationTestBase
{
    private readonly IProductRepository productRepository;

    public DeleteProductTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
    }

    [Fact]
    public async Task DeleteProduct_Success()
    {
        // Arrange
        var productId = Guid.NewGuid();
        await mediator.Send(new CreateProduct(
            productId,
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            null));

        // Act
        var result = await mediator.Send(new DeleteProduct(productId));

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var deletedProduct = await productRepository.GetByIdAsync(productId);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task DeleteProduct_Failure_ProductNotFound()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();

        // Act
        var result = await mediator.Send(new DeleteProduct(nonExistentProductId));

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }
}