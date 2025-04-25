using Catalog.Core;
using Catalog.Core.Persistence;
using Ecommerce.Api;
using Ecommerce.Api.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ordering.Core;
using Ordering.Core.Persistence;
using Pay.Core;
using Pay.Core.Persistence;
using Reporting.Core;
using Serilog;
using Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
var httpContext = builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();
AspNetCoreResult.Setup(config => config.DefaultProfile = new CustomAspNetCoreResultEndpointProfile(httpContext));

builder.Services.RegisterCommonServices(
    builder.Configuration,
    [
        CatalogModule.ConfigureConsumers,
        OrderingModule.ConfigureConsumers,
        PayModule.ConfigureConsumers
    ],
    [
        Catalog.Core.AssemblyInfo.Ref,
        Ordering.Core.AssemblyInfo.Ref,
        Pay.Core.AssemblyInfo.Ref
    ],
    typeof(CatalogDbContext),
    typeof(OrderingDbContext),
    typeof(PayDbContext)
);

builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddOrderingModule(builder.Configuration);
builder.Services.AdPayModule(builder.Configuration);
builder.Services.AddReportingModule(builder.Configuration);

builder.Services.AddControllers();

// Add Logging
builder.Host.UseSerilog(Logging.ConfigureLogger);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseLogContextTraceLogging();

app.MapControllers();

app.UseCatalogModule();
app.UseOrderingModule();
app.UsePayModule();
app.UseReportingModule();

app.Run();


public partial class Program
{
}