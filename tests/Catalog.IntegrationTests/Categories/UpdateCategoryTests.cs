namespace Catalog.IntegrationTests.Categories;

public class UpdateCategoryTests : IntegrationTestBase
{
    private readonly IWriteCategoryRepository categoryRepository;

    public UpdateCategoryTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        categoryRepository = serviceScope.ServiceProvider.GetRequiredService<IWriteCategoryRepository>();
    }

    [Fact]
    public async Task UpdateCategory_Success_ChangeName()
    {
        // Arrange
        var initialName = faker.Commerce.Categories(1)[0];
        var createResult = await mediator.Send(new CreateCategory(initialName, null));
        Assert.True(createResult.IsSuccess);

        var newName = faker.Commerce.Categories(1)[0];
        var command = new UpdateCategory(createResult.Value.Id, newName, null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify name changed
        var updatedCategory = await categoryRepository.GetByIdAsync(createResult.Value.Id);
        Assert.NotNull(updatedCategory);
        Assert.Equal(newName.ToLower(), updatedCategory.Name);
    }

    [Fact]
    public async Task UpdateCategory_Success_ChangeParent()
    {
        // Arrange
        var categoryName = faker.Commerce.Categories(1)[0];
        var parentCategoryName = faker.Commerce.Categories(1)[0];

        var categoryResult = await mediator.Send(new CreateCategory(categoryName, null));
        var parentCategoryResult = await mediator.Send(new CreateCategory(parentCategoryName, null));

        Assert.True(categoryResult.IsSuccess);
        Assert.True(parentCategoryResult.IsSuccess);

        // Update category to have a parent
        var command = new UpdateCategory(
            categoryResult.Value.Id,
            categoryName,
            parentCategoryResult.Value.Id);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify parent changed
        var updatedParent = await categoryRepository.GetByIdAsync(parentCategoryResult.Value.Id);
        Assert.NotNull(updatedParent);
        Assert.Contains(updatedParent.SubCategories, c => c.Id == categoryResult.Value.Id);
    }

    [Fact]
    public async Task UpdateCategory_Success_ChangeNameAndParent()
    {
        // Arrange - create two categories
        var categoryName = faker.Commerce.Categories(1)[0];
        var parentCategoryName = faker.Commerce.Categories(1)[0];

        var categoryResult = await mediator.Send(new CreateCategory(categoryName, null));
        var parentCategoryResult = await mediator.Send(new CreateCategory(parentCategoryName, null));

        Assert.True(categoryResult.IsSuccess);
        Assert.True(parentCategoryResult.IsSuccess);

        // Update category name and parent
        var newName = faker.Commerce.Categories(1)[0];
        var command = new UpdateCategory(
            categoryResult.Value.Id,
            newName,
            parentCategoryResult.Value.Id);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify name and parent changed
        var updatedParent = await categoryRepository.GetByIdAsync(parentCategoryResult.Value.Id);
        Assert.NotNull(updatedParent);

        var updatedCategory = updatedParent.SubCategories.FirstOrDefault(c => c.Id == categoryResult.Value.Id);
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
        var categoryName = faker.Commerce.Categories(1)[0];
        var categoryResult = await mediator.Send(new CreateCategory(categoryName, null));
        Assert.True(categoryResult.IsSuccess);

        // Try to update with non-existent parent
        var nonExistentParentId = Guid.NewGuid();
        var command = new UpdateCategory(
            categoryResult.Value.Id,
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
        var categoryName = faker.Commerce.Categories(1)[0];
        var categoryResult = await mediator.Send(new CreateCategory(categoryName, null));
        Assert.True(categoryResult.IsSuccess);

        // Try to update with empty name
        var command = new UpdateCategory(
            categoryResult.Value.Id,
            "",
            null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }
}