using Catalog.Application.ProductAttributes.Commands;
using Catalog.Domain.ProductAttributeAggregate;
using Catalog.IntegrationTests.Abstractions;
using Common.Domain;
using Microsoft.Extensions.DependencyInjection;

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
        var attributeName = "features";
        var createResult = await mediator.Send(new CreateAttribute(attributeName));
        Assert.True(createResult.IsSuccess);

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
    public async Task DeleteAttribute_Success_CaseInsensitive()
    {
        // Arrange
        var attributeName = "features";
        var createResult = await mediator.Send(new CreateAttribute(attributeName));
        Assert.True(createResult.IsSuccess);

        // Delete using uppercase name
        var command = new DeleteAttribute(attributeName.ToUpper());

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
        var nonExistentName = "nonexistent";
        var command = new DeleteAttribute(nonExistentName);

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