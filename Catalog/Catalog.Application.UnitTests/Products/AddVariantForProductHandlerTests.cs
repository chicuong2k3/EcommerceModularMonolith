using Catalog.Application.Products.Commands;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAttributeAggregate;
using Common.Domain;
using NSubstitute;

namespace Catalog.Application.UnitTests.Products;

public class AddVariantForProductHandlerTests
{
    private readonly IProductRepository productRepository;
    private readonly IProductAttributeRepository productAttributeRepository;
    private readonly AddVariantForProductHandler handler;

    public AddVariantForProductHandlerTests()
    {
        productRepository = Substitute.For<IProductRepository>();
        productAttributeRepository = Substitute.For<IProductAttributeRepository>();
        handler = new AddVariantForProductHandler(productRepository, productAttributeRepository);
    }

    [Fact]
    public async Task Handle_Should_AddVariant_When_ValidCommandProvided()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", null).Value;
        productRepository
            .GetByIdWithVariantsAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        var attribute = ProductAttribute.Create("Color").Value;
        productAttributeRepository.GetByNameAsync("Color").Returns(attribute);

        var command = new AddVariantForProduct(
            product.Id,
            "SKU123",
            100,
            5,
            "https://example.com/image.jpg",
            "Product Image",
            [new AttributeValue("Color", "Red")],
            null,
            null,
            null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(product.Variants);
        Assert.Equal("SKU123", product.Variants.First().Sku);
        await productRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        productRepository
            .GetByIdWithVariantsAsync(productId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var command = new AddVariantForProduct(
            productId,
            "SKU123",
            100,
            5,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SkuIsInvalid()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", null).Value;
        productRepository.GetByIdWithVariantsAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var command = new AddVariantForProduct(
            product.Id,
            "", // Invalid SKU
            100,
            5,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_PriceIsInvalid()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", null).Value;
        productRepository.GetByIdWithVariantsAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var command = new AddVariantForProduct(
            product.Id,
            "SKU123",
            -100,
            5,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_QuantityIsInvalid()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", null).Value;
        productRepository.GetByIdWithVariantsAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var command = new AddVariantForProduct(
            product.Id,
            "SKU123",
            100,
            0,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AttributeNotFound()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", null).Value;
        productRepository.GetByIdWithVariantsAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        productAttributeRepository.GetByNameAsync("Color").Returns((ProductAttribute?)null);

        var command = new AddVariantForProduct(
            product.Id,
            "SKU123",
            100,
            5,
            null,
            null,
            [new AttributeValue("Color", "Red")],
            null,
            null,
            null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }
}