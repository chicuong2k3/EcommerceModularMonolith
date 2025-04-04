using Catalog.Application.Products.Commands;
using Catalog.Domain.ProductAggregate;
using Common.Domain;
using NSubstitute;

namespace Catalog.Application.UnitTests.Products;

public class DeleteProductHandlerTests
{
    private readonly IProductRepository productRepository;
    private readonly DeleteProductHandler handler;

    public DeleteProductHandlerTests()
    {
        productRepository = Substitute.For<IProductRepository>();
        handler = new DeleteProductHandler(productRepository);
    }

    [Fact]
    public async Task Handle_Should_DeleteProduct_When_ProductExists()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", null).Value;
        productRepository
            .GetByIdAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        var command = new DeleteProduct(product.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        await productRepository.Received(1).RemoveAsync(product, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        productRepository
            .GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var command = new DeleteProduct(productId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }
}