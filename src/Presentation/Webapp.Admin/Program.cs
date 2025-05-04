using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using Blazorise.Tailwind;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Webapp.Admin;
using Webapp.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
            .AddBlazorise(options => { options.Immediate = true; })
            .AddTailwindProviders()
            .AddFontAwesomeIcons();

builder.Services.AddBlazoriseRichTextEdit(options =>
{

});

builder.Services.AddSharedServices();

await builder.Build().RunAsync();


