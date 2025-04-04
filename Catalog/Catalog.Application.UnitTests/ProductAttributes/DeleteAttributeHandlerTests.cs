using Catalog.Application.ProductAttributes.Commands;
using Catalog.Domain.ProductAttributeAggregate;
using Common.Domain;
using NSubstitute;

namespace Catalog.Application.UnitTests.ProductAttributes;

public class DeleteAttributeHandlerTests
{
    private readonly IProductAttributeRepository mockRepository;
    private readonly DeleteAttributeHandler handler;

    public DeleteAttributeHandlerTests()
    {
        mockRepository = Substitute.For<IProductAttributeRepository>();
        handler = new DeleteAttributeHandler(mockRepository);
    }

    [Fact]
    public async Task Handle_Should_DeleteAttribute_When_AttributeExists()
    {
        // Arrange
        var attribute = ProductAttribute.Create("Size").Value;
        mockRepository
            .GetByNameAsync("size", Arg.Any<CancellationToken>())
            .Returns(attribute);

        var command = new DeleteAttribute("size");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        await mockRepository.Received(1).RemoveAsync(attribute, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AttributeNotFound()
    {
        // Arrange
        mockRepository
            .GetByNameAsync("NonExistent", Arg.Any<CancellationToken>())
            .Returns((ProductAttribute?)null);

        var command = new DeleteAttribute("NonExistent");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }
}