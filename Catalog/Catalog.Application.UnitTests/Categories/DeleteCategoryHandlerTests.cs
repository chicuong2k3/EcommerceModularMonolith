using Catalog.Application.Categories.Commands;
using Catalog.Domain.CategoryAggregate;
using Common.Domain;
using NSubstitute;

namespace Catalog.Application.UnitTests.Categories;

public class DeleteCategoryHandlerTests
{
    private readonly ICategoryRepository categoryRepository;
    private readonly DeleteCategoryHandler handler;

    public DeleteCategoryHandlerTests()
    {
        categoryRepository = Substitute.For<ICategoryRepository>();
        handler = new DeleteCategoryHandler(categoryRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CategoryExists()
    {
        // Arrange
        var category = Category.Create("Electronics").Value;
        categoryRepository
            .GetByIdAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        var command = new DeleteCategory(category.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        await categoryRepository.Received(1).RemoveAsync(category, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CategoryNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        categoryRepository
            .GetByIdAsync(nonExistentId, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        var command = new DeleteCategory(nonExistentId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }
}