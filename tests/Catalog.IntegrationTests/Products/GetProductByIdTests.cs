namespace Catalog.IntegrationTests.Products;

public class GetProductByIdTests : IntegrationTestBase
{
    private readonly IProductRepository productRepository;

    public GetProductByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
    }

    [Fact]
    public async Task GetProductById_Success()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        await mediator.Send(new CreateProduct(productId, productName, description, null));

        // Act
        var result = await mediator.Send(new GetProductById(productId));

        // Assert
        Assert.True(result.IsSuccess);
        var product = result.Value;
        Assert.Equal(productId, product.Id);
        Assert.Equal(productName, product.Name);
        Assert.Equal(description, product.Description);
    }

    [Fact]
    public async Task GetProductById_Failure_ProductNotFound()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();

        // Act
        var result = await mediator.Send(new GetProductById(nonExistentProductId));

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task GetProductById_IncludesAllVariantsAndAttributes()
    {
        // Arrange
        var product = await CreateProductWithMultipleVariants();

        // Act
        var query = new GetProductById(product.Id);
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify all variants are returned
        Assert.Equal(3, result.Value.Variants.Count);

        // Check that variants have different prices
        var prices = result.Value.Variants.Select(v => v.OriginalPrice).Distinct().ToList();
        Assert.Equal(3, prices.Count);

        // Verify attributes for each variant
        foreach (var variant in result.Value.Variants)
        {
            Assert.NotEmpty(variant.Attributes);
        }
    }

    private async Task<Product> CreateProductWithMultipleVariants()
    {
        // Create attributes first
        var createSizeAttrCmd = new CreateAttribute(Guid.NewGuid(), "Size");
        var sizeResult = await mediator.Send(createSizeAttrCmd);
        Assert.True(sizeResult.IsSuccess);

        var createColorAttrCmd = new CreateAttribute(Guid.NewGuid(), "Color");
        var colorResult = await mediator.Send(createColorAttrCmd);
        Assert.True(colorResult.IsSuccess);

        // Create product
        var productId = Guid.NewGuid();
        var createProductCmd = new CreateProduct(
            productId,
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            null);

        var productResult = await mediator.Send(createProductCmd);
        Assert.True(productResult.IsSuccess);

        // Add multiple variants with different attributes
        var sizes = new[] { "Small", "Medium", "Large" };
        var colors = new[] { "Red", "Blue", "Green" };
        var prices = new[] { 19.99m, 24.99m, 29.99m };

        for (int i = 0; i < 3; i++)
        {
            var attributes = new List<AttributeValue>
            {
                new AttributeValue("Size", sizes[i]),
                new AttributeValue("Color", colors[i])
            };

            var addVariantCmd = new AddVariantForProduct(
                productId,
                prices[i],
                faker.Random.Int(1, 100),
                null,
                null,
                attributes,
                null,
                null,
                null);

            var variantResult = await mediator.Send(addVariantCmd);
            Assert.True(variantResult.IsSuccess);
        }

        var product = await productRepository.GetByIdWithVariantsAsync(productId);
        return product!;
    }
}