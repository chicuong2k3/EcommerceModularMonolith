using Catalog.Core.ReadModels;
using Shared.Abstractions.Core;

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
        var categoryId = Guid.NewGuid();
        var categoryName = faker.Commerce.Categories(1)[0];
        var command = new CreateCategory(categoryId, categoryName, null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify persistence
        var savedCategory = await categoryRepository.GetByIdAsync(categoryId);
        Assert.NotNull(savedCategory);
        Assert.Equal(categoryName.ToLower(), savedCategory.Name);
        Assert.Null(savedCategory.ParentCategoryId);
        Assert.Empty(savedCategory.SubCategories);
    }

    [Fact]
    public async Task CreateCategory_Success_WithParent()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parentName = faker.Commerce.Categories(1)[0];
        var parentResult = await mediator.Send(new CreateCategory(parentId, parentName, null));
        Assert.True(parentResult.IsSuccess);

        var subCategoryId = Guid.NewGuid();
        var subCategoryName = faker.Commerce.Categories(1)[0];
        var command = new CreateCategory(subCategoryId, subCategoryName, parentId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify subcategory was created
        var savedSubCategory = await categoryRepository.GetByIdAsync(subCategoryId);
        Assert.NotNull(savedSubCategory);
        Assert.Equal(subCategoryName.ToLower(), savedSubCategory.Name);
        Assert.Equal(parentId, savedSubCategory.ParentCategoryId);
        Assert.Empty(savedSubCategory.SubCategories);

        // Verify parent category has subcategory
        var updatedParent = await categoryRepository.GetByIdAsync(parentId);
        Assert.NotNull(updatedParent);
        Assert.Contains(updatedParent.SubCategories, c => c.Id == subCategoryId);
    }

    [Fact]
    public async Task CreateCategory_Failure_EmptyName()
    {
        // Arrange
        var command = new CreateCategory(Guid.NewGuid(), "", null);

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
        var command = new CreateCategory(Guid.NewGuid(), longName, null);

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
        var nonExistentParentId = Guid.NewGuid();
        var command = new CreateCategory(Guid.NewGuid(), faker.Commerce.Categories(1)[0], nonExistentParentId);

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
        var parentId = Guid.NewGuid();
        var parentName = faker.Commerce.Categories(1)[0];
        var parentResult = await mediator.Send(new CreateCategory(parentId, parentName, null));
        Assert.True(parentResult.IsSuccess);

        // Create subcategories sequentially to avoid DbContext concurrency issues
        var subCategoryIds = new List<Guid>();
        for (int i = 0; i < 3; i++)
        {
            var subCategoryId = Guid.NewGuid();
            var result = await mediator.Send(
                new CreateCategory(subCategoryId, faker.Commerce.Categories(1)[0], parentId)
            );
            Assert.True(result.IsSuccess);
            subCategoryIds.Add(subCategoryId);
        }

        // Verify subcategories were created
        foreach (var subCategoryId in subCategoryIds)
        {
            var savedSubCategory = await categoryRepository.GetByIdAsync(subCategoryId);
            Assert.NotNull(savedSubCategory);
            Assert.Equal(parentId, savedSubCategory.ParentCategoryId);
            Assert.Empty(savedSubCategory.SubCategories);
        }

        // Verify parent has all subcategories
        var updatedParent = await categoryRepository.GetByIdAsync(parentId);
        Assert.NotNull(updatedParent);
        Assert.Equal(3, updatedParent.SubCategories.Count);
        foreach (var subCategoryId in subCategoryIds)
        {
            Assert.Contains(updatedParent.SubCategories, c => c.Id == subCategoryId);
        }
    }
}
