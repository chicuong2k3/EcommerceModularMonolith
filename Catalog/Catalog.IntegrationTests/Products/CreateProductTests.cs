using Catalog.Application.Categories.Commands;
using Catalog.Application.Products.Commands;
using Catalog.Domain.ProductAggregate;
using Catalog.IntegrationTests.Abstractions;
using Common.Domain;
using Microsoft.Extensions.DependencyInjection;

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
        var productName = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var command = new CreateProduct(productName, description, null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(productName, result.Value.Name);
        Assert.Equal(description, result.Value.Description);
        Assert.Null(result.Value.CategoryId);
        Assert.Empty(result.Value.Variants);

        // Verify persistence
        var savedProduct = await productRepository.GetByIdAsync(result.Value.Id);
        Assert.NotNull(savedProduct);
        Assert.Equal(productName, savedProduct.Name);
        Assert.Equal(description, savedProduct.Description);
    }

    [Fact]
    public async Task CreateProduct_Success_WithCategory()
    {
        // Arrange
        var categoryName = faker.Commerce.Categories(1)[0];
        var categoryResult = await mediator.Send(new CreateCategory(categoryName, null));
        Assert.True(categoryResult.IsSuccess);

        var productName = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var command = new CreateProduct(productName, description, categoryResult.Value.Id);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(productName, result.Value.Name);
        Assert.Equal(description, result.Value.Description);
        Assert.Equal(categoryResult.Value.Id, result.Value.CategoryId);

        // Verify persistence
        var savedProduct = await productRepository.GetByIdAsync(result.Value.Id);
        Assert.NotNull(savedProduct);
        Assert.Equal(categoryResult.Value.Id, savedProduct.CategoryId);
    }

    [Fact]
    public async Task CreateProduct_Failure_EmptyName()
    {
        // Arrange
        var command = new CreateProduct("", faker.Commerce.ProductDescription(), null);

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