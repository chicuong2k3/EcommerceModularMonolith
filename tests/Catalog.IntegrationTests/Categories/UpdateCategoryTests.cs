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
}