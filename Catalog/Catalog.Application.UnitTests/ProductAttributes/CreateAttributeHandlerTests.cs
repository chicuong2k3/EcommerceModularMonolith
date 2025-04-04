using Catalog.Application.ProductAttributes.Commands;
using Catalog.Domain.ProductAttributeAggregate;
using Common.Domain;
using NSubstitute;

namespace Catalog.Application.UnitTests.ProductAttributes;

public class CreateAttributeHandlerTests
{
    private readonly IProductAttributeRepository mockRepository;
    private readonly CreateAttributeHandler handler;

    public CreateAttributeHandlerTests()
    {
        mockRepository = Substitute.For<IProductAttributeRepository>();
        handler = new CreateAttributeHandler(mockRepository);
    }

    [Fact]
    public async Task Handle_Should_CreateAttribute_When_ValidNameProvided()
    {
        // Arrange
        var command = new CreateAttribute("Color");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("color", result.Value.Name);
        await mockRepository.Received(1).AddAsync(Arg.Any<ProductAttribute>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NameIsInvalid()
    {
        // Arrange
        var command = new CreateAttribute("");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }
}

