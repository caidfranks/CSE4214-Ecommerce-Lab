using GameVault.Server.Services;
using GameVault.Server.Filters;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// builder.Services.AddAuthorization(options =>
//     {
//         // options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // Or JwtBearerDefaults.AuthenticationScheme, etc.
//         // options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Or JwtBearerDefaults.AuthenticationScheme, etc.
//         options.AddPolicy("LoggedIn", policy => policy.RequireAuthenticatedUser());
//     });

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters();
// });
// .AddCookie(options => // If using cookie authentication
// {
//     options.LoginPath = "/login"; // Set your login path
// });

builder.Services.AddSingleton<IFirebaseAuthService, FirebaseAuthService>();
builder.Services.AddSingleton<IFirestoreService, FirestoreService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<CartService>();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new[] { "https://localhost:5166", "http://localhost:5166" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthorization();
// app.UseAuthentication();
app.MapControllers();
app.Run();