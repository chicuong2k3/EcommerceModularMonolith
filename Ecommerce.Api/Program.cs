using Catalog.Infrastructure;
using Catalog.Infrastructure.Persistence;
using Common.Infrastructure;
using Ecommerce.Api;
using Ecommerce.Api.Logging;
using FluentResults.Extensions.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Promotion.Infrastructure;
using Promotion.Infrastructure.Persistence;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
var httpContext = builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();
AspNetCoreResult.Setup(config => config.DefaultProfile = new CustomAspNetCoreResultEndpointProfile(httpContext));

builder.Services.RegisterCommonServices(
    builder.Configuration,
    [
        Catalog.Infrastructure.ServicesRegistrator.ConfigureConsumers,
        Ordering.Infrastructure.ServicesRegistrator.ConfigureConsumers,
    ],
    [
        Catalog.Application.AssemblyInfo.Ref,
        Promotion.Application.AssemblyInfo.Ref,
        Ordering.Application.AssemblyInfo.Ref
    ],
    typeof(CatalogDbContext),
    typeof(OrderingDbContext),
    typeof(PromotionDbContext));

builder.Services.RegisterCatalogServices(builder.Configuration);
builder.Services.RegisterOrderingServices(builder.Configuration);
builder.Services.RegisterPromotionServices(builder.Configuration);
//builder.Services.RegisterPaymentServices(builder.Configuration);

builder.Services.AddControllers();

// Add Logging
builder.Host.UseSerilog(Logging.ConfigureLogger);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Services.MigratePromotionDatabase();
await app.Services.MigrateCatalogDatabaseAsync();
await app.Services.MigrateOrderingDatabaseAsync();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();
