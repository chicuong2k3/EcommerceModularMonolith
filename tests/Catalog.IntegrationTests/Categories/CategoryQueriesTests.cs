namespace Catalog.IntegrationTests.Categories;

public class CategoryQueriesTests : IntegrationTestBase
{
    private readonly ICategoryRepository categoryRepository;

    public CategoryQueriesTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        categoryRepository = serviceScope.ServiceProvider.GetRequiredService<ICategoryRepository>();
    }

    [Fact]
    public async Task GetCategories_ReturnsAllCategories()
    {
        // Arrange
        var categoryNames = new[] { "Electronics", "Clothing", "Books", "Home" };
        foreach (var name in categoryNames)
        {
            await mediator.Send(new CreateCategory(Guid.NewGuid(), name, null));
        }

        // Act
        var query = new GetCategories();
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        var categories = await categoryRepository.GetAllAsync();
        Assert.NotNull(categories);

        // Should have at least the categories we created
        Assert.True(categories.Count() >= categoryNames.Length);

        // Verify all our categories exist in the result
        foreach (var name in categoryNames)
        {
            Assert.Contains(categories, c => c.Name == name.ToLower());
        }
    }

    [Fact]
    public async Task GetCategoryById_ReturnsCategory_WhenExists()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parentResult = await mediator.Send(new CreateCategory(parentId, "Parent Category", null));
        Assert.True(parentResult.IsSuccess);

        var childNames = new[] { "Child One", "Child Two" };
        foreach (var name in childNames)
        {
            await mediator.Send(new CreateCategory(Guid.NewGuid(), name, parentId));
        }

        // Act
        var query = new GetCategoryById(parentId);
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        var category = await categoryRepository.GetByIdAsync(parentId);
        Assert.NotNull(category);
        Assert.Equal(parentId, category.Id);
        Assert.Equal("parent category", category.Name);

        // Verify subcategories
        Assert.Equal(childNames.Length, category.SubCategories.Count);
        foreach (var name in childNames)
        {
            Assert.Contains(category.SubCategories, c => c.Name == name.ToLower());
        }
    }

    [Fact]
    public async Task GetCategoryById_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var query = new GetCategoryById(nonExistentId);
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task GetCategoryById_ReturnsEmptySubCategories_WhenNoChildren()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var createResult = await mediator.Send(new CreateCategory(categoryId, "Solo Category", null));
        Assert.True(createResult.IsSuccess);

        // Act
        var query = new GetCategoryById(categoryId);
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        var category = await categoryRepository.GetByIdAsync(categoryId);
        Assert.NotNull(category);
        Assert.Equal(categoryId, category.Id);

        // Verify no subcategories
        Assert.Empty(category.SubCategories);
    }
}