using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameVault.Client;
using GameVault.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ListingService>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<CheckoutService>();
builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<BreadcrumbService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LogService>();

var app = builder.Build();

var authService = app.Services.GetRequiredService<AuthService>();
await authService.InitializeAsync();

await app.RunAsync();