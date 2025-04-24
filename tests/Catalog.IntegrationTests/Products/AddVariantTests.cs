namespace Catalog.IntegrationTests.Products;

public class AddVariantTests : IntegrationTestBase
{
    private readonly IWriteProductRepository productRepository;

    public AddVariantTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IWriteProductRepository>();
    }

    [Fact]
    public async Task AddVariant_Success_BasicVariant()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Create variant
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);
        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            price,
            quantity,
            null,  // No image
            null,
            Array.Empty<AttributeValue>(),  // No attributes
            null,  // No discount
            null,
            null
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify product has variant
        var product = await productRepository.GetByIdWithVariantsAsync(createProductResult.Value.Id);
        Assert.NotNull(product);
        Assert.Single(product.Variants);
        Assert.Equal(quantity, product.Variants.First().Quantity);
        Assert.Equal(price, product.Variants.First().OriginalPrice.Amount);
    }

    [Fact]
    public async Task AddVariant_Success_WithDiscount()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Create variant with discount
        var originalPrice = faker.Random.Decimal(50, 100);
        var salePrice = faker.Random.Decimal(10, 49);
        var quantity = faker.Random.Int(1, 100);
        var discountStart = DateTime.UtcNow.AddDays(-1);
        var discountEnd = DateTime.UtcNow.AddDays(10);

        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            originalPrice,
            quantity,
            null,  // No image
            null,
            Array.Empty<AttributeValue>(),  // No attributes
            discountStart,
            discountEnd,
            salePrice
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify product has variant with discount
        var product = await productRepository.GetByIdWithVariantsAsync(createProductResult.Value.Id);
        Assert.NotNull(product);
        Assert.Single(product.Variants);

        var variant = product.Variants.First();
        Assert.Equal(originalPrice, variant.OriginalPrice.Amount);
        Assert.NotNull(variant.SalePrice);
        Assert.Equal(salePrice, variant.SalePrice.Amount);
        Assert.NotNull(variant.SalePriceEffectivePeriod);
        Assert.Equal(discountStart, variant.SalePriceEffectivePeriod.Start);
        Assert.Equal(discountEnd, variant.SalePriceEffectivePeriod.End);
    }

    [Fact]
    public async Task AddVariant_Success_WithAttributes()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Create attribute
        var attributeName = "color";
        var attributeValue = faker.Commerce.Color();

        // Create variant with attribute
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);

        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            price,
            quantity,
            null,  // No image
            null,
            [new AttributeValue(attributeName, attributeValue)],
            null,  // No discount
            null,
            null
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify product has variant with attribute
        var product = await productRepository.GetByIdWithVariantsAsync(createProductResult.Value.Id);
        Assert.NotNull(product);
        Assert.Single(product.Variants);
    }

    [Fact]
    public async Task AddVariant_Failure_ProductNotFound()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();
        var command = new AddVariantForProduct(
            nonExistentProductId,
            faker.Random.Decimal(10, 100),
            faker.Random.Int(1, 100),
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task AddVariant_Failure_InvalidPrice()
    {
        // Arrange - create a product first
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Create variant with negative price
        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            -10, // negative price should fail
            faker.Random.Int(1, 100),
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task AddVariant_Success_WithImage()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Create variant with image
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);
        var imageUrl = "https://example.com/image.jpg";
        var imageAltText = "Product image";

        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            price,
            quantity,
            imageUrl,
            imageAltText,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify product has variant with image
        var product = await productRepository.GetByIdWithVariantsAsync(createProductResult.Value.Id);
        Assert.NotNull(product);
        Assert.Single(product.Variants);
        Assert.NotNull(product.Variants.First().Image);
        Assert.Equal(imageUrl, product.Variants.First().Image.Url);
        Assert.Equal(imageAltText, product.Variants.First().Image.AltText);
    }

    [Fact]
    public async Task AddVariant_Failure_InvalidImageUrl()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Create variant with invalid image URL (empty string is invalid)
        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            faker.Random.Decimal(10, 100),
            faker.Random.Int(1, 100),
            "abc",  // Invalid URL should cause validation error
            "Alt text",
            Array.Empty<AttributeValue>(),
            null,
            null,
            null
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task AddVariant_Failure_InvalidSalePrice()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Create variant with invalid sale price (negative)
        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            faker.Random.Decimal(10, 100),
            faker.Random.Int(1, 100),
            null,
            null,
            Array.Empty<AttributeValue>(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(10),
            -5.0m  // Negative sale price should cause validation error
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task AddVariant_Failure_InvalidDateTimeRange()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Create variant with invalid date range (end before start)
        var start = DateTime.UtcNow;
        var end = start.AddDays(-5); // End date before start date

        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            faker.Random.Decimal(10, 100),
            faker.Random.Int(1, 100),
            null,
            null,
            Array.Empty<AttributeValue>(),
            start,
            end,  // End before start should cause validation error
            faker.Random.Decimal(5, 9)
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }

    [Fact]
    public async Task AddVariant_Failure_AttributeNotFound()
    {
        // Arrange
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Create variant with non-existent attribute
        var nonExistentAttributeName = "NonExistentAttribute_" + Guid.NewGuid().ToString();
        var attributes = new List<AttributeValue>
        {
            new AttributeValue(nonExistentAttributeName, "Some Value")
        };

        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            faker.Random.Decimal(10, 100),
            faker.Random.Int(1, 100),
            null,
            null,
            attributes,
            null,
            null,
            null
        );

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
        Assert.Contains(result.Errors, error => error.Message.Contains(nonExistentAttributeName));
    }
}