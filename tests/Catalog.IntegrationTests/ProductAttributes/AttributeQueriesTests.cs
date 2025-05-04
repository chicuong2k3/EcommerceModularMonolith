namespace Catalog.IntegrationTests.ProductAttributes;

public class AttributeQueriesTests : IntegrationTestBase
{
    private readonly IProductAttributeRepository attributeRepository;

    public AttributeQueriesTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        attributeRepository = serviceScope.ServiceProvider.GetRequiredService<IProductAttributeRepository>();
    }

    [Fact]
    public async Task GetAttributes_ReturnsAllAttributes()
    {
        // Arrange
        var attributeNames = new[] { "Color", "Size", "Material", "Weight" };
        foreach (var name in attributeNames)
        {
            await mediator.Send(new CreateAttribute(Guid.NewGuid(), name));
        }

        // Act
        var query = new GetAttributes();
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        // Should have at least the attributes we created
        Assert.True(result.Value.Count >= attributeNames.Length);

        // Verify all attributes exist in result
        foreach (var name in attributeNames)
        {
            Assert.Contains(result.Value, a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public async Task GetAttributes_ReturnsEmptyList_WhenNoAttributesExist()
    {
        // Arrange
        var existingAttributes = await mediator.Send(new GetAttributes());
        foreach (var attr in existingAttributes.Value)
        {
            await mediator.Send(new DeleteAttribute(attr.Id));
        }

        // Act
        var query = new GetAttributes();
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAttributeByName_ReturnsAttribute_WhenExists()
    {
        // Arrange
        var attributeName = "TestAttribute";
        await mediator.Send(new CreateAttribute(Guid.NewGuid(), attributeName));

        // Act
        var query = new GetAttributeByName(attributeName);
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(attributeName, result.Value.Name, ignoreCase: true);
    }

    [Fact]
    public async Task GetAttributeByName_ReturnsNotFound_WhenAttributeDoesNotExist()
    {
        // Arrange
        var nonExistentName = "NonExistentAttribute";

        // Act
        var query = new GetAttributeByName(nonExistentName);
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task GetAttributeByName_IsCaseInsensitive()
    {
        // Arrange
        var attributeName = "case-test";
        await mediator.Send(new CreateAttribute(Guid.NewGuid(), attributeName));

        // Act
        var query = new GetAttributeByName(attributeName.ToUpper());
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(attributeName, result.Value.Name, ignoreCase: true);
    }
}