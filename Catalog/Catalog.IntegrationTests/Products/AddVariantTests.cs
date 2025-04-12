using Catalog.Application.Products.Commands;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAttributeAggregate;
using Catalog.IntegrationTests.Abstractions;
using Common.Domain;
using Microsoft.Extensions.DependencyInjection;

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
        var productName = faker.Commerce.ProductName();
        var createProductResult = await mediator.Send(new CreateProduct(productName, null, null));
        Assert.True(createProductResult.IsSuccess);

        // Create variant
        var sku = faker.Commerce.Ean13();
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);
        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            sku,
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
        Assert.Equal(sku, product.Variants.First().Sku);
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
        var sku = faker.Commerce.Ean13();
        var originalPrice = faker.Random.Decimal(50, 100);
        var salePrice = faker.Random.Decimal(10, 49);
        var quantity = faker.Random.Int(1, 100);
        var discountStart = DateTime.UtcNow.AddDays(-1);
        var discountEnd = DateTime.UtcNow.AddDays(10);

        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            sku,
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
        Assert.Equal(sku, variant.Sku);
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
        var sku = faker.Commerce.Ean13();
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);

        var command = new AddVariantForProduct(
            createProductResult.Value.Id,
            sku,
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
            faker.Commerce.Ean13(),
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
            faker.Commerce.Ean13(),
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
}