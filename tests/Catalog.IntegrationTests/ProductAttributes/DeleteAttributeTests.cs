using Shared.Abstractions.Core;

namespace Catalog.IntegrationTests.ProductAttributes;

public class DeleteAttributeTests : IntegrationTestBase
{
    private readonly IProductAttributeRepository productAttributeRepository;

    public DeleteAttributeTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productAttributeRepository = serviceScope.ServiceProvider.GetRequiredService<IProductAttributeRepository>();
    }

    [Fact]
    public async Task DeleteAttribute_Success()
    {
        // Arrange
        var attributeId = Guid.NewGuid();
        var attributeName = "test-attribute";
        await mediator.Send(new CreateAttribute(attributeId, attributeName));

        var command = new DeleteAttribute(attributeName);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify deletion
        var deletedAttribute = await productAttributeRepository.GetByNameAsync(attributeName);
        Assert.Null(deletedAttribute);
    }

    [Fact]
    public async Task DeleteAttribute_Failure_AttributeNotFound()
    {
        // Arrange
        var command = new DeleteAttribute("non-existent-attribute");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task DeleteAttribute_Failure_EmptyName()
    {
        // Arrange
        var command = new DeleteAttribute("");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }
}