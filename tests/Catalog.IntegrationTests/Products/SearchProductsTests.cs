//using Catalog.Core.Commands;
//using Catalog.Core.Entities;
//using Catalog.Core.Queries;

//namespace Catalog.IntegrationTests.Products;

//public class SearchProductsTests : IntegrationTestBase
//{
//    private readonly IProductRepository productRepository;

//    public SearchProductsTests(IntegrationTestWebAppFactory factory) : base(factory)
//    {
//        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
//    }

//    [Fact]
//    public async Task SearchProducts_ReturnsMatchingProducts_WhenSearchTextProvided()
//    {
//        // Arrange - Create some test products
//        var uniqueProduct = await CreateProductWithName("Unique Test Product");
//        await CreateProductWithName("Regular Product One");
//        await CreateProductWithName("Regular Product Two");

//        // Act - Search for the unique product
//        var query = new SearchProducts(
//            PageSize: 10,
//            PageNumber: 1,
//            CategoryId: null,
//            SearchText: "Unique Test",
//            SortBy: null,
//            MinPrice: null,
//            MaxPrice: null,
//            Attributes: null);

//        var result = await mediator.Send(query);

//        // Assert
//        Assert.True(result.IsSuccess);
//        Assert.Single(result.Value.Items);
//        Assert.Equal(uniqueProduct.Id, result.Value.Items[0].Id);
//        Assert.Equal("Unique Test Product", result.Value.Items[0].Name);
//    }

//    [Fact]
//    public async Task SearchProducts_PaginatesCorrectly()
//    {
//        // Arrange - Create multiple products
//        for (int i = 1; i <= 15; i++)
//        {
//            await CreateProductWithName($"Pagination Test Product {i}");
//        }

//        // Act - Get first page
//        var firstPageQuery = new SearchProducts(
//            PageSize: 10,
//            PageNumber: 1,
//            CategoryId: null,
//            SearchText: "Pagination Test",
//            SortBy: null,
//            MinPrice: null,
//            MaxPrice: null,
//            Attributes: null);

//        var firstPageResult = await mediator.Send(firstPageQuery);

//        // Get second page
//        var secondPageQuery = new SearchProducts(
//            PageSize: 10,
//            PageNumber: 2,
//            CategoryId: null,
//            SearchText: "Pagination Test",
//            SortBy: null,
//            MinPrice: null,
//            MaxPrice: null,
//            Attributes: null);

//        var secondPageResult = await mediator.Send(secondPageQuery);

//        // Assert
//        Assert.True(firstPageResult.IsSuccess);
//        Assert.True(secondPageResult.IsSuccess);

//        Assert.Equal(10, firstPageResult.Value.Items.Count);
//        Assert.Equal(5, secondPageResult.Value.Items.Count);
//        Assert.Equal(15, firstPageResult.Value.TotalCount);
//        Assert.Equal(15, secondPageResult.Value.TotalCount);

//        // Verify different products on each page
//        var firstPageIds = firstPageResult.Value.Items.Select(p => p.Id).ToList();
//        var secondPageIds = secondPageResult.Value.Items.Select(p => p.Id).ToList();
//        Assert.Empty(firstPageIds.Intersect(secondPageIds));
//    }

//    [Fact]
//    public async Task SearchProducts_SortsByPrice()
//    {
//        // Arrange - Create products with different prices
//        await CreateProductWithVariant("Expensive Product", 100m);
//        await CreateProductWithVariant("Mid-Price Product", 50m);
//        await CreateProductWithVariant("Budget Product", 10m);

//        // Act - Sort by price ascending
//        var ascendingQuery = new SearchProducts(
//            PageSize: 10,
//            PageNumber: 1,
//            CategoryId: null,
//            SearchText: "Product",
//            SortBy: "price asc",
//            MinPrice: null,
//            MaxPrice: null,
//            Attributes: null);

//        var ascendingResult = await mediator.Send(ascendingQuery);

//        // Sort by price descending
//        var descendingQuery = new SearchProducts(
//            PageSize: 10,
//            PageNumber: 1,
//            CategoryId: null,
//            SearchText: "Product",
//            SortBy: "price desc",
//            MinPrice: null,
//            MaxPrice: null,
//            Attributes: null);

//        var descendingResult = await mediator.Send(descendingQuery);

//        // Assert
//        Assert.True(ascendingResult.IsSuccess);
//        Assert.True(descendingResult.IsSuccess);

//        Assert.Equal(3, ascendingResult.Value.Items.Count);
//        Assert.Equal(3, descendingResult.Value.Items.Count);

//        // Verify ascending order
//        Assert.Contains("Budget", ascendingResult.Value.Items[0].Name);
//        Assert.Contains("Mid-Price", ascendingResult.Value.Items[1].Name);
//        Assert.Contains("Expensive", ascendingResult.Value.Items[2].Name);

//        // Verify descending order
//        Assert.Contains("Expensive", descendingResult.Value.Items[0].Name);
//        Assert.Contains("Mid-Price", descendingResult.Value.Items[1].Name);
//        Assert.Contains("Budget", descendingResult.Value.Items[2].Name);
//    }

//    [Fact]
//    public async Task SearchProducts_FiltersByPriceRange()
//    {
//        // Arrange - Create products with different prices
//        await CreateProductWithVariant("Expensive Product", 100m);
//        await CreateProductWithVariant("Mid-Price Product", 50m);
//        await CreateProductWithVariant("Budget Product", 10m);

//        // Act - Filter to show only mid-range products
//        var query = new SearchProducts(
//            PageSize: 10,
//            PageNumber: 1,
//            CategoryId: null,
//            SearchText: "Product",
//            SortBy: null,
//            MinPrice: 20m,
//            MaxPrice: 80m,
//            Attributes: null);

//        var result = await mediator.Send(query);

//        // Assert
//        Assert.True(result.IsSuccess);
//        Assert.Single(result.Value.Items);
//        Assert.Contains("Mid-Price", result.Value.Items[0].Name);
//    }

//    [Fact]
//    public async Task SearchProducts_FiltersByCategory()
//    {
//        // Arrange - Create a category and products
//        var createCategoryCommand = new CreateCategory("Test Category", null);
//        var categoryResult = await mediator.Send(createCategoryCommand);
//        Assert.True(categoryResult.IsSuccess);

//        var categoryId = categoryResult.Value.Id;

//        // Create products with and without category
//        await CreateProductWithName("Product With Category", categoryId);
//        await CreateProductWithName("Product Without Category");

//        // Act - Filter by category
//        var query = new SearchProducts(
//            PageSize: 10,
//            PageNumber: 1,
//            CategoryId: categoryId,
//            SearchText: null,
//            SortBy: null,
//            MinPrice: null,
//            MaxPrice: null,
//            Attributes: null);

//        var result = await mediator.Send(query);

//        // Assert
//        Assert.True(result.IsSuccess);
//        Assert.Single(result.Value.Items);
//        Assert.Equal("Product With Category", result.Value.Items[0].Name);
//    }

//    // Helper methods
//    private async Task<Product> CreateProductWithName(string name, Guid? categoryId = null)
//    {
//        var createCommand = new CreateProduct(name, "Test description", categoryId);
//        var result = await mediator.Send(createCommand);
//        Assert.True(result.IsSuccess);

//        return await productRepository.GetByIdAsync(result.Value.Id);
//    }

//    private async Task<Product> CreateProductWithVariant(string productName, decimal price)
//    {
//        var product = await CreateProductWithName(productName);

//        var addVariantCommand = new AddVariantForProduct(
//            product.Id,
//            faker.Commerce.Ean13(),
//            price,
//            10, // quantity
//            null, // imageUrl
//            null, // imageAltText
//            Array.Empty<AttributeValue>(),
//            null, // salePrice
//            null, // discountStart
//            null  // discountEnd
//        );

//        var result = await mediator.Send(addVariantCommand);
//        Assert.True(result.IsSuccess);

//        return await productRepository.GetByIdWithVariantsAsync(product.Id);
//    }
//}