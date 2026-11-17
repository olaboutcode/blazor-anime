using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Examples;
using BlazorAnime;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazorAnime();
builder.Services.AddBlazorAnimeAnimatable();
builder.Services.AddBlazorAnimeDraggable();
builder.Services.AddBlazorAnimeLayout();
builder.Services.AddBlazorAnimeScope();
builder.Services.AddBlazorAnimeEvents();

await builder.Build().RunAsync();