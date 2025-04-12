using Catalog.Application.Products.Commands;
using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.ProductAggregate;
using Common.Domain;
using NSubstitute;

namespace Catalog.Application.UnitTests.Products;

public class CreateProductHandlerTests
{
    private readonly IProductRepository productRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly CreateProductHandler handler;

    public CreateProductHandlerTests()
    {
        // Initialize dependencies without using "_" prefix
        productRepository = Substitute.For<IProductRepository>();
        categoryRepository = Substitute.For<ICategoryRepository>();
        handler = new CreateProductHandler(productRepository, categoryRepository);
    }

    [Fact]
    public async Task Handle_Should_CreateProduct_When_ValidCommandProvided()
    {
        // Arrange
        var command = new CreateProduct("Laptop", "High-performance laptop", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Laptop", result.Value.Name);
        Assert.Equal("High-performance laptop", result.Value.Description);
        await productRepository.Received(1).AddAsync(Arg.Any<Product>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_Should_CreateProductWithCategory_When_ValidCategoryIdProvided()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Electronics").Value;
        categoryRepository
            .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(category);

        var command = new CreateProduct("Smartphone", "Flagship smartphone", categoryId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Smartphone", result.Value.Name);
        Assert.Equal(categoryId, result.Value.CategoryId);
        await productRepository.Received(1).AddAsync(Arg.Any<Product>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CategoryNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        categoryRepository
            .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        var command = new CreateProduct("Tablet", "Portable tablet", categoryId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProductCreationFails()
    {
        // Arrange
        var command = new CreateProduct("", "Invalid product", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_DescriptionIsInvalid()
    {
        // Arrange
        var command = new CreateProduct("Laptop", "", null); // Empty description

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', 201); // Assuming max length is 200
        var command = new CreateProduct(longName, "Valid description", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_DescriptionExceedsMaxLength()
    {
        // Arrange
        var longDescription = new string('a', 1001); // Assuming max length is 1000
        var command = new CreateProduct("Valid name", longDescription, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }
}