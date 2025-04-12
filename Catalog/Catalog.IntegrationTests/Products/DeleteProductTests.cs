using Catalog.Application.Products.Commands;
using Catalog.Domain.ProductAggregate;
using Catalog.IntegrationTests.Abstractions;
using Common.Domain;
using Microsoft.Extensions.DependencyInjection;

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
        var productName = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var createResult = await mediator.Send(new CreateProduct(productName, description, null));
        Assert.True(createResult.IsSuccess);

        var command = new DeleteProduct(createResult.Value.Id);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify deletion
        var deletedProduct = await productRepository.GetByIdAsync(createResult.Value.Id);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task DeleteProduct_Failure_ProductNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new DeleteProduct(nonExistentId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }
}