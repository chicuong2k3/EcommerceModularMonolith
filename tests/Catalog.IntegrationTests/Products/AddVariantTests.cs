using System.Text;

namespace Catalog.IntegrationTests.Products;

public class AddVariantTests : IntegrationTestBase
{
    private readonly IProductRepository productRepository;

    public AddVariantTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
    }

    [Fact]
    public async Task AddVariant_Success_BasicVariant()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Create variant
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);
        var command = new AddVariantForProduct(
            productId,
            price,
            quantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify product has variant
        var product = await productRepository.GetByIdWithVariantsAsync(productId);
        Assert.NotNull(product);
        Assert.Single(product.Variants);
        Assert.Equal(quantity, product.Variants.First().Quantity);
        Assert.Equal(price, product.Variants.First().OriginalPrice.Amount);
    }

    [Fact]
    public async Task AddVariant_Success_WithDiscount()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Create variant with discount
        var originalPrice = faker.Random.Decimal(50, 100);
        var salePrice = faker.Random.Decimal(10, 49);
        var quantity = faker.Random.Int(1, 100);
        var discountStart = DateTime.UtcNow.AddDays(-1);
        var discountEnd = DateTime.UtcNow.AddDays(10);

        var command = new AddVariantForProduct(
            productId,
            originalPrice,
            quantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            discountStart,
            discountEnd,
            salePrice);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify product has variant with discount
        var product = await productRepository.GetByIdWithVariantsAsync(productId);
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
        // Create attributes first
        var createSizeAttrCmd = new CreateAttribute(Guid.NewGuid(), "Size");
        var sizeResult = await mediator.Send(createSizeAttrCmd);
        Assert.True(sizeResult.IsSuccess);

        var createColorAttrCmd = new CreateAttribute(Guid.NewGuid(), "Color");
        var colorResult = await mediator.Send(createColorAttrCmd);
        Assert.True(colorResult.IsSuccess);

        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Create attribute
        var attributeName = "color";
        var attributeValue = faker.Commerce.Color();

        // Create variant with attribute
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);

        var command = new AddVariantForProduct(
            productId,
            price,
            quantity,
            null,
            null,
            new[] { new AttributeValue(attributeName, attributeValue) },
            null,
            null,
            null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify product has variant with attribute
        var product = await productRepository.GetByIdWithVariantsAsync(productId);
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
            null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task AddVariant_Failure_InvalidPrice()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Create variant with negative price
        var command = new AddVariantForProduct(
            productId,
            -10, // negative price should fail
            faker.Random.Int(1, 100),
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null);

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
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Create variant with image
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);
        var fakeImageData = Convert.ToBase64String(Encoding.UTF8.GetBytes("fake image content"));
        var imageAltText = "Product image";

        var command = new AddVariantForProduct(
            productId,
            price,
            quantity,
            fakeImageData,
            imageAltText,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify product has variant with image
        var product = await productRepository.GetByIdWithVariantsAsync(productId);
        Assert.NotNull(product);
        Assert.Single(product.Variants);
        Assert.NotNull(product.Variants.First().Image);
        Assert.Equal(fakeImageData, product.Variants.First().Image.Base64Data);
        Assert.Equal(imageAltText, product.Variants.First().Image.AltText);
    }

    [Fact]
    public async Task AddVariant_Failure_InvalidBase64Image()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        var invalidBase64 = "abc";

        var command = new AddVariantForProduct(
            productId,
            faker.Random.Decimal(10, 100),
            faker.Random.Int(1, 100),
            invalidBase64,  // image stored as base64 string
            "Alt text",
            Array.Empty<AttributeValue>(),
            null,
            null,
            null);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error =>
            error is ValidationError ve && ve.Message.Contains("image", StringComparison.OrdinalIgnoreCase));
    }


    [Fact]
    public async Task AddVariant_Failure_InvalidSalePrice()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Create variant with invalid sale price (higher than original price)
        var originalPrice = faker.Random.Decimal(10, 50);
        var invalidSalePrice = originalPrice + 10; // Sale price higher than original

        var command = new AddVariantForProduct(
            productId,
            originalPrice,
            faker.Random.Int(1, 100),
            null,
            null,
            Array.Empty<AttributeValue>(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            invalidSalePrice);

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
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Create variant with invalid date range (end before start)
        var command = new AddVariantForProduct(
            productId,
            faker.Random.Decimal(10, 100),
            faker.Random.Int(1, 100),
            null,
            null,
            Array.Empty<AttributeValue>(),
            DateTime.UtcNow.AddDays(1), // Start date after end date
            DateTime.UtcNow,
            faker.Random.Decimal(5, 9));

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is ValidationError);
    }
}