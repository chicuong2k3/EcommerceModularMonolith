using Shared.Abstractions.Core;

namespace Catalog.IntegrationTests.ProductAttributes;

public class CreateAttributeTests : IntegrationTestBase
{
    private readonly IProductAttributeRepository productAttributeRepository;

    public CreateAttributeTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productAttributeRepository = serviceScope.ServiceProvider.GetRequiredService<IProductAttributeRepository>();
    }

    [Fact]
    public async Task CreateAttribute_Success()
    {
        // Arrange
        var attributeId = Guid.NewGuid();
        var attributeName = "color";
        var command = new CreateAttribute(attributeId, attributeName);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var savedAttribute = await productAttributeRepository.GetByNameAsync(attributeName);
        Assert.NotNull(savedAttribute);
        Assert.Equal(attributeName, savedAttribute.Name);
    }

    [Fact]
    public async Task CreateAttribute_Success_DifferentCase()
    {
        // Arrange
        var attributeId = Guid.NewGuid();
        var attributeName = "SIZE";
        var command = new CreateAttribute(attributeId, attributeName);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var savedAttribute = await productAttributeRepository.GetByNameAsync(attributeName);
        Assert.NotNull(savedAttribute);
        Assert.Equal(attributeName.ToLower(), savedAttribute.Name);
    }

    [Fact]
    public async Task CreateAttribute_Failure_EmptyName()
    {
        // Arrange
        var command = new CreateAttribute(Guid.NewGuid(), "");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }
}