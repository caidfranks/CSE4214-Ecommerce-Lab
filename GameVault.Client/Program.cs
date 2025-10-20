using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameVault.Client;
using GameVault.Client.Services;
using GameVaultWeb.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5080";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ListingService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserState>();

await builder.Build().RunAsync();