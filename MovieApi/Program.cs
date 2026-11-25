using Microsoft.AspNetCore.Identity;
using MovieApi.Models;
using MovieApi.Repositories;
using MySqlConnector;
using MovieApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Repositories
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<UserRepository>();

// Lösenordshashning
builder.Services.AddSingleton<
    IPasswordHasher<User>,
    PasswordHasher<User>>();

// Sessionslagring
builder.Services.AddDistributedMemoryCache();

// Sessionskonfiguration
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "MovieApi.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/test", () => "Jag är en annan endpoint, som ligger på /test");

app.MapGet("/api/health/db", async (
    IConfiguration configuration,
    ILogger<Program> logger) =>
{
    string connectionString = "Server=localhost;Port=3306;Database=movie_api;User ID=root;Password=kurslosenord;";

    try
    {
        await using var connection =
            new MySqlConnection(connectionString);

        await connection.OpenAsync();

        await using var command =
            new MySqlCommand("SELECT 1;", connection);

        object? result = await command.ExecuteScalarAsync();

        return Results.Ok(new
        {
            message = "Database connection successful",
            result
        });
    }
    catch (MySqlException ex)
    {
        logger.LogError(ex, "Database connection failed");

        return Results.Problem(
            title: "Database unavailable",
            statusCode: 503
        );
    }
});

// Middleware
app.UseRouting();
app.UseSession();

// Endpoints
app.MapMovieEndpoints();
app.MapUserEndpoints();
app.MapAuthEndpoints();

app.Run();