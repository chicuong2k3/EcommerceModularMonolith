using Catalog.Application.ProductAttributes.Commands;
using Catalog.Domain.ProductAttributeAggregate;
using Catalog.IntegrationTests.Abstractions;
using Common.Domain;
using Microsoft.Extensions.DependencyInjection;

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
        var attributeName = "color";
        var command = new CreateAttribute(attributeName);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(attributeName, result.Value.Name);

        // Verify persistence
        var savedAttribute = await productAttributeRepository.GetByNameAsync(attributeName);
        Assert.NotNull(savedAttribute);
        Assert.Equal(attributeName, savedAttribute.Name);
    }

    [Fact]
    public async Task CreateAttribute_Success_DifferentCase()
    {
        // Arrange
        var attributeName = "SIZE";
        var command = new CreateAttribute(attributeName);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(attributeName.ToLower(), result.Value.Name);

        // Verify persistence
        var savedAttribute = await productAttributeRepository.GetByNameAsync(attributeName);
        Assert.NotNull(savedAttribute);
        Assert.Equal(attributeName.ToLower(), savedAttribute.Name);
    }

    [Fact]
    public async Task CreateAttribute_Failure_EmptyName()
    {
        // Arrange
        var command = new CreateAttribute("");

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }
}