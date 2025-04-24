using Catalog.Infrastructure;
using Catalog.Infrastructure.Persistence;
using Ecommerce.Api;
using Ecommerce.Api.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Serilog;
using Shared.Infrastructure;
using Reporting.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
var httpContext = builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();
AspNetCoreResult.Setup(config => config.DefaultProfile = new CustomAspNetCoreResultEndpointProfile(httpContext));

builder.Services.RegisterCommonServices(
    builder.Configuration,
    [
        Catalog.Infrastructure.CatalogModule.ConfigureConsumers,
        Ordering.Infrastructure.OrderingModule.ConfigureConsumers,
        //Billing.Infrastructure.BillingModule.ConfigureConsumers
    ],
    [
        Catalog.Core.AssemblyInfo.Ref,
        Ordering.Core.AssemblyInfo.Ref,
        //Billing.Application.AssemblyInfo.Ref
    ],
    typeof(CatalogDbContext),
    typeof(OrderingDbContext)
//typeof(BillingDbContext)
);

builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddOrderingModule(builder.Configuration);
//builder.Services.AddBillingModule(builder.Configuration);
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
//app.UseBillingModule();
app.UseReportingModule();

app.Run();


public partial class Program
{
}