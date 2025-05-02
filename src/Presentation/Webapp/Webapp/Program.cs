using Blazorise.Icons.FontAwesome;
using Blazorise.Tailwind;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Webapp;
using Webapp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<ResponseHandler>();
builder.Services.AddHttpClient<CategoryService>((sp, client) =>
{
    client.BaseAddress = new Uri("https://localhost:7210");
})
.AddHttpMessageHandler<ResponseHandler>();
builder.Services.AddHttpClient<ProductAttributeService>((sp, client) =>
{
    client.BaseAddress = new Uri("https://localhost:7210");
})
.AddHttpMessageHandler<ResponseHandler>();

AddBlazorise(builder.Services);

await builder.Build().RunAsync();


void AddBlazorise(IServiceCollection services)
{
    services
        .AddBlazorise();
    services
        .AddTailwindProviders()
        .AddFontAwesomeIcons();

}
