using Catalog.Application.Categories.Commands;
using Catalog.Domain.CategoryAggregate;
using Common.Domain;
using NSubstitute;

namespace Catalog.Application.UnitTests.Categories;

public class CreateCategoryHandlerTests
{
    private readonly ICategoryRepository mockRepository;
    private readonly CreateCategoryHandler handler;

    public CreateCategoryHandlerTests()
    {
        mockRepository = Substitute.For<ICategoryRepository>();
        handler = new CreateCategoryHandler(mockRepository);
    }

    [Fact]
    public async Task Handle_Should_CreateCategory_When_ValidNameProvided()
    {
        // Arrange
        var command = new CreateCategory("Electronics", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("electronics", result.Value.Name);
        await mockRepository.Received(1).AddAsync(Arg.Any<Category>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NameIsInvalid()
    {
        // Arrange
        var command = new CreateCategory("", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task Handle_Should_AddSubCategory_When_ParentCategoryIdIsValid()
    {
        // Arrange
        var parentCategory = Category.Create("Parent").Value;
        mockRepository
            .GetByIdAsync(parentCategory.Id, Arg.Any<CancellationToken>())
            .Returns(parentCategory);

        var command = new CreateCategory("Child", parentCategory.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(parentCategory.SubCategories.Any());
        Assert.Contains(parentCategory.SubCategories, c => c.Name == "child");
        await mockRepository.Received(1).AddAsync(Arg.Any<Category>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ParentCategoryNotFound()
    {
        // Arrange
        var nonExistentParentId = Guid.NewGuid();
        mockRepository
            .GetByIdAsync(nonExistentParentId, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        var command = new CreateCategory("Child", nonExistentParentId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }
}
