using Microsoft.AspNetCore.Identity;
using MovieApi.DTOs;
using MovieApi.Models;
using MovieApi.Repositories;

namespace MovieApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/api/auth");

        auth.MapPost("/login", async (
            LoginDto data,
            UserRepository repository,
            IPasswordHasher<User> passwordHasher,
            HttpContext context) =>
        {
            if (string.IsNullOrWhiteSpace(data.Username) ||
                string.IsNullOrWhiteSpace(data.Password))
            {
                return Results.BadRequest(new
                {
                    message = "Username and password are required."
                });
            }

            User? user = await repository.GetUserByUsernameAsync(
                data.Username
            );

            if (user is null)
            {
                return Results.Unauthorized();
            }

            PasswordVerificationResult result =
                passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    data.Password
                );

            if (result == PasswordVerificationResult.Failed)
            {
                return Results.Unauthorized();
            }

            // Skapa inloggad session
            await context.Session.LoadAsync();

            context.Session.SetInt32("UserId", user.Id);

            await context.Session.CommitAsync();

            return Results.Ok(new
            {
                message = "Login successful.",
                user = new UserResponseDto(
                    user.Id,
                    user.Username
                )
            });
        });
        
        auth.MapGet("/login", async (
            HttpContext context,
            UserRepository repository) =>
        {
            await context.Session.LoadAsync();

            int? userId = context.Session.GetInt32("UserId");

            if (userId is null)
            {
                return Results.Unauthorized();
            }

            User? user = await repository.GetUserByIdAsync(
                userId.Value
            );

            if (user is null)
            {
                context.Session.Clear();
                await context.Session.CommitAsync();

                return Results.Unauthorized();
            }

            return Results.Ok(new
            {
                authenticated = true,
                user = new UserResponseDto(
                    user.Id,
                    user.Username
                )
            });
        });
        
        auth.MapDelete("/login", async (HttpContext context) =>
        {
            await context.Session.LoadAsync();

            context.Session.Clear();

            await context.Session.CommitAsync();

            return Results.NoContent();
        });
    }
    
}