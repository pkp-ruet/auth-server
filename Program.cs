using AuthServer.Models;
using AuthServer.Repositories;
using AuthServer.Services;
var builder = WebApplication.CreateBuilder(args);
var authenticationConfiguration = builder.Configuration.GetSection("Authentication").Get<AuthenticationConfiguration>();
builder.Services.AddSingleton(authenticationConfiguration);
builder.Services.AddControllers();
builder.Services.AddSingleton<IHashService, HashService>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IRefreshTokenRepository, InMemoryRefreshTokenRepository>();
builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddSingleton<RefreshTokenValidator>();



var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();
