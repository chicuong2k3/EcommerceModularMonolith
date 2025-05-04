using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Webapp.Admin;
using Webapp.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddShared();

builder.Services.AddBlazoriseRichTextEdit(options =>
{

});

await builder.Build().RunAsync();


