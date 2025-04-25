using Catalog.Core.Commands;
using Catalog.Core.Repositories;
using Shared.Abstractions.Core;

namespace Catalog.IntegrationTests.Products;

public class DeleteVariantTests : IntegrationTestBase
{
    private readonly IProductRepository productRepository;

    public DeleteVariantTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        productRepository = serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
    }

    [Fact]
    public async Task DeleteVariant_Success()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Add variant
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);
        var addVariantResult = await mediator.Send(new AddVariantForProduct(
            productId,
            price,
            quantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null));
        Assert.True(addVariantResult.IsSuccess);

        // Get the variant ID
        var product = await productRepository.GetByIdWithVariantsAsync(productId);
        var variantId = product.Variants.First().Id;

        var command = new DeleteVariant(productId, variantId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify variant was deleted
        var updatedProduct = await productRepository.GetByIdWithVariantsAsync(productId);
        Assert.NotNull(updatedProduct);
        Assert.Empty(updatedProduct.Variants);
    }

    [Fact]
    public async Task DeleteVariant_Failure_ProductNotFound()
    {
        // Arrange
        var command = new DeleteVariant(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task DeleteVariant_Failure_VariantNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        var command = new DeleteVariant(productId, Guid.NewGuid());

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }

    [Fact]
    public async Task DeleteVariant_Success_LastVariant()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Add variant
        var price = faker.Random.Decimal(10, 100);
        var quantity = faker.Random.Int(1, 100);
        var addVariantResult = await mediator.Send(new AddVariantForProduct(
            productId,
            price,
            quantity,
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null));
        Assert.True(addVariantResult.IsSuccess);

        // Get the variant ID
        var product = await productRepository.GetByIdWithVariantsAsync(productId);
        var variantId = product.Variants.First().Id;

        var command = new DeleteVariant(productId, variantId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify product exists but has no variants
        var updatedProduct = await productRepository.GetByIdWithVariantsAsync(productId);
        Assert.NotNull(updatedProduct);
        Assert.Empty(updatedProduct.Variants);
    }

    [Fact]
    public async Task DeleteVariant_Success_MultipleVariants()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = faker.Commerce.ProductName();
        var createResult = await mediator.Send(new CreateProduct(productId, productName, null, null));
        Assert.True(createResult.IsSuccess);

        // Add first variant
        var addFirstVariantResult = await mediator.Send(new AddVariantForProduct(
            productId,
            faker.Random.Decimal(10, 100),
            faker.Random.Int(1, 100),
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null));
        Assert.True(addFirstVariantResult.IsSuccess);

        // Add second variant
        var addSecondVariantResult = await mediator.Send(new AddVariantForProduct(
            productId,
            faker.Random.Decimal(10, 100),
            faker.Random.Int(1, 100),
            null,
            null,
            Array.Empty<AttributeValue>(),
            null,
            null,
            null));
        Assert.True(addSecondVariantResult.IsSuccess);

        // Get the first variant ID
        var product = await productRepository.GetByIdWithVariantsAsync(productId);
        var firstVariantId = product.Variants.First().Id;

        var command = new DeleteVariant(productId, firstVariantId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify one variant remains
        var updatedProduct = await productRepository.GetByIdWithVariantsAsync(productId);
        Assert.NotNull(updatedProduct);
        Assert.Single(updatedProduct.Variants);
        Assert.DoesNotContain(updatedProduct.Variants, v => v.Id == firstVariantId);
    }
}