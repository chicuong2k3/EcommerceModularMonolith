using Catalog.Application.Products.Commands;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAttributeAggregate;
using Common.Domain;
using NSubstitute;

namespace Catalog.Application.UnitTests.Products;

public class UpdateQuantityHandlerTests
{
    private readonly IProductRepository productRepository;
    private readonly IProductAttributeRepository productAttributeRepository;
    private readonly UpdateQuantityHandler handler;

    public UpdateQuantityHandlerTests()
    {
        productRepository = Substitute.For<IProductRepository>();
        productAttributeRepository = Substitute.For<IProductAttributeRepository>();
        handler = new UpdateQuantityHandler(productRepository);
    }

    [Fact]
    public async Task Handle_Should_UpdateQuantity_When_ValidCommandProvided()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", null).Value;
        var salePeriod = DateTimeRange.Create(DateTime.UtcNow, DateTime.UtcNow.AddDays(1)).Value;
        var variant = ProductVariant.Create("SKU123", Money.FromDecimal(100).Value, 5, null, Money.FromDecimal(80).Value, salePeriod).Value;
        product.AddVariant(variant);

        productRepository.GetByIdWithVariantsAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var command = new UpdateQuantity(product.Id, variant.Id, 10);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(10, variant.Quantity);
        await productRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        productRepository.GetByIdWithVariantsAsync(productId, Arg.Any<CancellationToken>()).Returns((Product)null);

        var command = new UpdateQuantity(productId, Guid.NewGuid(), 10);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_VariantNotFound()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", null).Value;
        productRepository
            .GetByIdWithVariantsAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        var command = new UpdateQuantity(product.Id, Guid.NewGuid(), 10);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NewQuantityIsNegative()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", null).Value;
        var salePeriod = DateTimeRange.Create(DateTime.UtcNow, DateTime.UtcNow.AddDays(1)).Value;
        var variant = ProductVariant.Create("SKU123", Money.FromDecimal(100).Value, 5, null, Money.FromDecimal(80).Value, salePeriod).Value;
        product.AddVariant(variant);

        productRepository
            .GetByIdWithVariantsAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        var command = new UpdateQuantity(product.Id, variant.Id, -5);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }
}