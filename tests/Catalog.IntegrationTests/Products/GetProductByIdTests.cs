namespace Catalog.IntegrationTests.Products;

public class GetProductByIdTests : IntegrationTestBase
{
    private readonly IWriteProductRepository productRepository;

    public GetProductByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IWriteProductRepository>();
    }

    [Fact]
    public async Task GetProductById_ReturnsProduct_WhenExists()
    {
        // Arrange
        var product = await CreateProductWithVariantAndAttributes();

        // Act
        var query = new GetProductById(product.Id);
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(product.Id, result.Value.Id);
        Assert.Equal(product.Name, result.Value.Name);
        Assert.Equal(product.Description, result.Value.Description);

        // Verify variants
        Assert.NotEmpty(result.Value.Variants);
        var variant = result.Value.Variants[0];
        var originalVariant = product.Variants.First();
        Assert.Equal(originalVariant.Id, variant.VariantId);
        Assert.Equal(originalVariant.OriginalPrice, variant.OriginalPrice);

        // Verify attributes
        Assert.NotEmpty(variant.Attributes);
    }

    [Fact]
    public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var query = new GetProductById(nonExistentId);
        var result = await mediator.Send(query);

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

    // Helper methods
    private async Task<Product> CreateProductWithVariantAndAttributes()
    {
        // Create a product
        var createProductCmd = new CreateProduct(
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            null);

        var productResult = await mediator.Send(createProductCmd);
        Assert.True(productResult.IsSuccess);

        // Add a variant with attributes
        var attributes = new List<AttributeValue>
        {
            new AttributeValue("Color", "Red"),
            new AttributeValue("Size", "Large")
        };

        var addVariantCmd = new AddVariantForProduct(
            productResult.Value.Id,
            faker.Random.Decimal(10, 100),
            faker.Random.Int(1, 100),
            null,
            null,
            attributes,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1),
            faker.Random.Decimal(1, 10)
        );

        var variantResult = await mediator.Send(addVariantCmd);
        Assert.True(variantResult.IsSuccess);
        var product = await productRepository.GetByIdWithVariantsAsync(productResult.Value.Id);

        return product!;
    }

    private async Task<Product> CreateProductWithMultipleVariants()
    {
        // Create product
        var createProductCmd = new CreateProduct(
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            null);

        var productResult = await mediator.Send(createProductCmd);
        Assert.True(productResult.IsSuccess);

        // Add multiple variants with different attributes
        var sizes = new[] { "Small", "Medium", "Large" };
        var prices = new[] { 19.99m, 24.99m, 29.99m };

        for (int i = 0; i < 3; i++)
        {
            var attributes = new List<AttributeValue>
            {
                new AttributeValue("Size", sizes[i]),
                new AttributeValue("Color", faker.Commerce.Color())
            };

            var addVariantCmd = new AddVariantForProduct(
                productResult.Value.Id,
                prices[i],
                faker.Random.Int(1, 100),
                null,
                null,
                attributes,
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                faker.Random.Decimal(1, 10)
            );

            var variantResult = await mediator.Send(addVariantCmd);
            Assert.True(variantResult.IsSuccess);
        }

        var product = await productRepository.GetByIdWithVariantsAsync(productResult.Value.Id);
        return product!;
    }
}