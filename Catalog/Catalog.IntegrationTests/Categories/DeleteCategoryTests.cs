using Catalog.Application.Categories.Commands;
using Catalog.Domain.CategoryAggregate;
using Catalog.IntegrationTests.Abstractions;
using Common.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.IntegrationTests.Categories;

public class DeleteCategoryTests : IntegrationTestBase
{
    private readonly ICategoryRepository categoryRepository;

    public DeleteCategoryTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        categoryRepository = serviceScope.ServiceProvider.GetRequiredService<ICategoryRepository>();
    }

    [Fact]
    public async Task DeleteCategory_Success()
    {
        // Arrange
        var category = Category.Create(faker.Commerce.Categories(1)[0]).Value;
        await categoryRepository.AddAsync(category);
        var command = new DeleteCategory(category.Id);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify deletion
        var deletedCategory = await categoryRepository.GetByIdAsync(category.Id);
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task DeleteCategory_Failure_CategoryNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new DeleteCategory(nonExistentId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task DeleteCategory_Success_RemovesFromParent()
    {
        // Arrange
        var parentCategory = Category.Create(faker.Commerce.Categories(1)[0]).Value;
        var childCategory = Category.Create(faker.Commerce.Categories(1)[0]).Value;

        parentCategory.AddSubCategory(childCategory);
        await categoryRepository.AddAsync(parentCategory);

        var command = new DeleteCategory(childCategory.Id);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify child deleted
        var deletedChild = await categoryRepository.GetByIdAsync(childCategory.Id);
        Assert.Null(deletedChild);

        // Verify parent no longer contains child
        var updatedParent = await categoryRepository.GetByIdAsync(parentCategory.Id);
        Assert.NotNull(updatedParent);
        Assert.DoesNotContain(updatedParent.SubCategories, c => c.Id == childCategory.Id);
    }

    [Fact]
    public async Task DeleteCategory_Success_WithSubcategories()
    {
        // Arrange
        var parentCategory = Category.Create(faker.Commerce.Categories(1)[0]).Value;

        // Add subcategories
        var subCategory1 = Category.Create(faker.Commerce.Categories(1)[0]).Value;
        var subCategory2 = Category.Create(faker.Commerce.Categories(1)[0]).Value;

        parentCategory.AddSubCategory(subCategory1);
        parentCategory.AddSubCategory(subCategory2);

        await categoryRepository.AddAsync(parentCategory);

        var command = new DeleteCategory(parentCategory.Id);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify parent deleted
        var deletedParent = await categoryRepository.GetByIdAsync(parentCategory.Id);
        Assert.Null(deletedParent);

        var deletedSub1 = await categoryRepository.GetByIdAsync(subCategory1.Id);
        var deletedSub2 = await categoryRepository.GetByIdAsync(subCategory2.Id);

        Assert.Null(deletedSub1);
        Assert.Null(deletedSub2);
    }
}