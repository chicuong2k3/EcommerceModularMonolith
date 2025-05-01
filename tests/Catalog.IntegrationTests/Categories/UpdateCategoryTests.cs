using FluentResults;

namespace Catalog.IntegrationTests.Categories;

public class UpdateCategoryTests : IntegrationTestBase
{
    private readonly ICategoryRepository categoryRepository;

    public UpdateCategoryTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        categoryRepository = serviceScope.ServiceProvider.GetRequiredService<ICategoryRepository>();
    }

    [Fact]
    public async Task UpdateCategory_Success_ChangeName()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var initialName = faker.Commerce.Categories(1)[0];
        await mediator.Send(new CreateCategory(categoryId, initialName, null));

        var newName = faker.Commerce.Categories(1)[0];
        var command = new UpdateCategory(categoryId, newName, null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify name changed
        var updatedCategory = await categoryRepository.GetByIdAsync(categoryId);
        Assert.NotNull(updatedCategory);
        Assert.Equal(newName.ToLower(), updatedCategory.Name);
    }

    [Fact]
    public async Task UpdateCategory_Success_ChangeParent()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var categoryName = faker.Commerce.Categories(1)[0];
        var parentCategoryName = faker.Commerce.Categories(1)[0];

        await mediator.Send(new CreateCategory(categoryId, categoryName, null));
        await mediator.Send(new CreateCategory(parentId, parentCategoryName, null));

        // Update category to have a parent
        var command = new UpdateCategory(
            categoryId,
            categoryName,
            parentId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify parent changed
        var updatedParent = await categoryRepository.GetByIdAsync(parentId);
        Assert.NotNull(updatedParent);
        Assert.Contains(updatedParent.SubCategories, c => c.Id == categoryId);
    }

    [Fact]
    public async Task UpdateCategory_Success_ChangeNameAndParent()
    {
        // Arrange - create two categories
        var categoryId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var categoryName = faker.Commerce.Categories(1)[0];
        var parentCategoryName = faker.Commerce.Categories(1)[0];

        await mediator.Send(new CreateCategory(categoryId, categoryName, null));
        await mediator.Send(new CreateCategory(parentId, parentCategoryName, null));

        // Update category name and parent
        var newName = faker.Commerce.Categories(1)[0];
        var command = new UpdateCategory(
            categoryId,
            newName,
            parentId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify name and parent changed
        var updatedParent = await categoryRepository.GetByIdAsync(parentId);
        Assert.NotNull(updatedParent);

        var updatedCategory = updatedParent.SubCategories.FirstOrDefault(c => c.Id == categoryId);
        Assert.NotNull(updatedCategory);
        Assert.Equal(newName.ToLower(), updatedCategory.Name);
    }

    [Fact]
    public async Task UpdateCategory_Failure_CategoryNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateCategory(nonExistentId, "newname", null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task UpdateCategory_Failure_ParentCategoryNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryName = faker.Commerce.Categories(1)[0];
        await mediator.Send(new CreateCategory(categoryId, categoryName, null));

        // Try to update with non-existent parent
        var nonExistentParentId = Guid.NewGuid();
        var command = new UpdateCategory(
            categoryId,
            categoryName,
            nonExistentParentId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task UpdateCategory_Failure_EmptyName()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryName = faker.Commerce.Categories(1)[0];
        await mediator.Send(new CreateCategory(categoryId, categoryName, null));

        // Try to update with empty name
        var command = new UpdateCategory(
            categoryId,
            "",
            null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task UpdateCategory_Failure_DuplicateName()
    {
        // Arrange
        var name1 = faker.Commerce.Categories(1)[0];
        var name2 = faker.Commerce.Categories(1)[0];
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var result1 = await mediator.Send(new CreateCategory(id1, name1, null));
        var result2 = await mediator.Send(new CreateCategory(id2, name2, null));

        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);

        // Act - attempt to update the second category to have the same name as the first
        var updateCommand = new UpdateCategory(id2, name1, null);
        var updateResult = await mediator.Send(updateCommand);

        // Assert
        Assert.True(updateResult.IsFailed);
        Assert.Contains(updateResult.Errors, e => e is ConflictError);
    }


    [Fact]
    public async Task UpdateCategory_Failure_CircularReference()
    {
        // Arrange
        var categoryAId = Guid.NewGuid();
        var categoryBId = Guid.NewGuid();
        var categoryCId = Guid.NewGuid();

        var nameA = faker.Commerce.Categories(1)[0];
        var nameB = faker.Commerce.Categories(1)[0];
        var nameC = faker.Commerce.Categories(1)[0];

        // Create A (no parent), B (parent = A), C (parent = B)
        var aResult = await mediator.Send(new CreateCategory(categoryAId, nameA, null));
        Assert.True(aResult.IsSuccess);
        var bResult = await mediator.Send(new CreateCategory(categoryBId, nameB, categoryAId));
        Assert.True(bResult.IsSuccess);
        var cResult = await mediator.Send(new CreateCategory(categoryCId, nameC, categoryBId));
        Assert.True(cResult.IsSuccess);

        // Try to make A's parent = C (should cause circular ref)
        var command = new UpdateCategory(
            categoryAId,
            nameA,
            categoryCId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }
}