using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Webapp.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.RegisterCommonServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
