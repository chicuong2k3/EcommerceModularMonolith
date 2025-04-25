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
            await mediator.Send(new CreateCategory(name, null));
        }

        // Act
        var query = new GetCategories();
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        // Should have at least the categories we created
        Assert.True(result.Value.Count() >= categoryNames.Length);

        // Verify all our categories exist in the result
        foreach (var name in categoryNames)
        {
            Assert.Contains(result.Value, c => c.Name == name.ToLower());
        }
    }

    [Fact]
    public async Task GetCategoryById_ReturnsCategory_WhenExists()
    {
        // Arrange
        var parentCategoryResult = await mediator.Send(new CreateCategory("Parent Category", null));
        Assert.True(parentCategoryResult.IsSuccess);

        var childNames = new[] { "Child One", "Child Two" };
        foreach (var name in childNames)
        {
            await mediator.Send(new CreateCategory(name, parentCategoryResult.Value.Id));
        }

        // Act
        var query = new GetCategoryById(parentCategoryResult.Value.Id);
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(parentCategoryResult.Value.Id, result.Value.Id);
        Assert.Equal("parent category", result.Value.Name);

        // Verify subcategories
        Assert.Equal(childNames.Length, result.Value.SubCategories.Count);
        foreach (var name in childNames)
        {
            Assert.Contains(result.Value.SubCategories, c => c.Name == name.ToLower());
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
        var categoryResult = await mediator.Send(new CreateCategory("Solo Category", null));
        Assert.True(categoryResult.IsSuccess);

        // Act
        var query = new GetCategoryById(categoryResult.Value.Id);
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(categoryResult.Value.Id, result.Value.Id);

        // Verify no subcategories
        Assert.Empty(result.Value.SubCategories);
    }
}