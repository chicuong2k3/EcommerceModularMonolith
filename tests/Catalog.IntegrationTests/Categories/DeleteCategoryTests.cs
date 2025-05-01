using Shared.Abstractions.Core;

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
        var categoryId = Guid.NewGuid();
        var categoryName = faker.Commerce.Categories(1)[0];
        var createResult = await mediator.Send(new CreateCategory(categoryId, categoryName, null));
        Assert.True(createResult.IsSuccess);

        var command = new DeleteCategory(categoryId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify deletion
        var deletedCategory = await categoryRepository.GetByIdAsync(categoryId);
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
        var parentId = Guid.NewGuid();
        var parentName = faker.Commerce.Categories(1)[0];
        var parentResult = await mediator.Send(new CreateCategory(parentId, parentName, null));
        Assert.True(parentResult.IsSuccess);

        var childId = Guid.NewGuid();
        var childName = faker.Commerce.Categories(1)[0];
        var childResult = await mediator.Send(new CreateCategory(childId, childName, parentId));
        Assert.True(childResult.IsSuccess);

        var command = new DeleteCategory(childId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify child was deleted
        var deletedChild = await categoryRepository.GetByIdAsync(childId);
        Assert.Null(deletedChild);

        // Verify parent no longer has the child
        var parent = await categoryRepository.GetByIdAsync(parentId);
        Assert.NotNull(parent);
        Assert.DoesNotContain(parent.SubCategories, c => c.Id == childId);
    }

    [Fact]
    public async Task DeleteCategory_Success_WithSubcategories()
    {
        // Arrange
        var parentCategory = Category.Create(Guid.NewGuid(), faker.Commerce.Categories(1)[0]).Value;

        // Add subcategories
        var subCategory1 = Category.Create(Guid.NewGuid(), faker.Commerce.Categories(1)[0]).Value;
        var subCategory2 = Category.Create(Guid.NewGuid(), faker.Commerce.Categories(1)[0]).Value;

        await parentCategory.AddSubCategoryAsync(subCategory1, categoryRepository);
        await parentCategory.AddSubCategoryAsync(subCategory2, categoryRepository);

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