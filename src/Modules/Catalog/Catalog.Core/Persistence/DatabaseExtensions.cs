using Catalog.Core.Commands;
using Catalog.Core.Entities;
using Catalog.Core.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Core.Persistence;

public static class DatabaseExtensions
{
    public static async Task MigrateCatalogDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<CatalogDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }

        var mediator = services.GetRequiredService<IMediator>();

        var productAttributes = new List<ProductAttribute>
            {
                ProductAttribute.Create("color").ValueOrDefault,
                ProductAttribute.Create("size").ValueOrDefault,
                ProductAttribute.Create("material").ValueOrDefault
            };

        if (!context.ProductAttributes.Any())
        {
            foreach (var pa in productAttributes)
            {
                await mediator.Send(new CreateAttribute(pa.Name));
            }
        }

        var productCategories = new List<Category>
            {
                Category.Create("t-shirt").Value,
                Category.Create("jacket").Value,
                Category.Create("shirt").Value,
                Category.Create("jeans").Value
            };

        var categoryIds = new List<Guid>();

        if (!context.Categories.Any())
        {
            foreach (var category in productCategories)
            {
                var createCategoryResult = await mediator.Send(new CreateCategory(category.Name, null));
                categoryIds.Add(createCategoryResult.Value.Id);
            }

            var products = new List<Product>
            {
                Product.Create("Classic T-Shirt", "A comfortable cotton t-shirt.", categoryIds[0]).Value,
                Product.Create("Leather Jacket", "A stylish leather jacket.", categoryIds[1]).Value,
                Product.Create("Formal Shirt", "A premium formal shirt.", categoryIds[2]).Value,
                Product.Create("Slim Fit Jeans", "Modern slim fit jeans.", categoryIds[3]).Value
            };

            var random = new Random();
            var colors = new List<string>() { "white", "black", "pink" };
            var sizes = new List<string>() { "s", "m", "l" };

            if (!context.Products.Any())
            {
                foreach (var product in products)
                {
                    var productCreateResult = await mediator.Send(new CreateProduct(product.Name, product.Description, product.CategoryId));
                    var price = Money.FromDecimal(random.Next(100000, 300000)).Value;
                    var quantity = random.Next(10, 100);
                    var startDiscountAt = DateTime.UtcNow.AddDays(random.Next(1, 5));
                    var endDiscountAt = startDiscountAt.AddDays(random.Next(5, 10));
                    var discountAmount = Money.FromDecimal(random.Next(10000, 50000)).Value;

                    foreach (var color in colors)
                    {
                        foreach (var size in sizes)
                        {
                            await mediator.Send(new AddVariantForProduct(
                                productCreateResult.Value.Id,
                                price,
                                quantity,
                                "",
                                "",
                                [new AttributeValue("color", color), new AttributeValue("size", size)],
                                startDiscountAt,
                                endDiscountAt,
                                discountAmount));

                        }
                    }


                }
            }
        }



    }
}
