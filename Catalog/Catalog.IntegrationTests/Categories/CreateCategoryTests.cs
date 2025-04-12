using Catalog.Application.Categories.Commands;
using Catalog.Application.Categories.ReadModels;
using Catalog.Domain.CategoryAggregate;
using Catalog.IntegrationTests.Abstractions;
using Common.Domain;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.IntegrationTests.Categories;

public class CreateCategoryTests : IntegrationTestBase
{
    private readonly ICategoryRepository categoryRepository;

    public CreateCategoryTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        categoryRepository = serviceScope.ServiceProvider.GetRequiredService<ICategoryRepository>();
    }

    [Fact]
    public async Task CreateCategory_Success_NoParent()
    {
        // Arrange
        var categoryName = faker.Commerce.Categories(1)[0];
        var command = new CreateCategory(categoryName, null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(categoryName.ToLower(), result.Value.Name);
        Assert.Null(result.Value.ParentCategoryId);
        Assert.Empty(result.Value.SubCategories);

        // Verify persistence
        var savedCategory = await categoryRepository.GetByIdAsync(result.Value.Id);
        Assert.NotNull(savedCategory);
        Assert.Equal(categoryName.ToLower(), savedCategory.Name);
    }

    [Fact]
    public async Task CreateCategory_Success_WithParent()
    {
        // Arrange
        var parentCategory = Category.Create(faker.Commerce.Categories(1)[0]).Value;
        await categoryRepository.AddAsync(parentCategory);

        var subCategoryName = faker.Commerce.Categories(1)[0];
        var command = new CreateCategory(subCategoryName, parentCategory.Id);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(subCategoryName.ToLower(), result.Value.Name);
        Assert.Equal(parentCategory.Id, result.Value.ParentCategoryId);
        Assert.Empty(result.Value.SubCategories);

        // Verify parent category has subcategory
        var updatedParent = await categoryRepository.GetByIdAsync(parentCategory.Id);
        Assert.NotNull(updatedParent);
        Assert.Contains(updatedParent.SubCategories, c => c.Id == result.Value.Id);
    }

    [Fact]
    public async Task CreateCategory_Failure_EmptyName()
    {
        // Arrange
        var command = new CreateCategory("", null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task CreateCategory_Failure_NameTooLong()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new CreateCategory(longName, null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task CreateCategory_Failure_ParentNotFound()
    {
        // Arrange
        var command = new CreateCategory(
            faker.Commerce.Categories(1)[0],
            Guid.NewGuid() // Non-existent parent ID
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task CreateCategory_Success_MultipleSubcategories()
    {
        // Arrange
        var parentCategory = Category.Create(faker.Commerce.Categories(1)[0]).Value;
        await categoryRepository.AddAsync(parentCategory);

        // Create subcategories sequentially to avoid DbContext concurrency issues
        var results = new List<Result<CategoryReadModel>>();
        for (int i = 0; i < 3; i++)
        {
            var result = await mediator.Send(
                new CreateCategory(faker.Commerce.Categories(1)[0], parentCategory.Id)
            );
            results.Add(result);
        }

        // Assert
        Assert.All(results, result =>
        {
            Assert.True(result.IsSuccess);
            Assert.Equal(parentCategory.Id, result.Value.ParentCategoryId);
            Assert.Empty(result.Value.SubCategories);
        });

        var updatedParent = await categoryRepository.GetByIdAsync(parentCategory.Id);
        Assert.NotNull(updatedParent);
        Assert.Equal(3, updatedParent.SubCategories.Count);
    }
}
