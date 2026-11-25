using Microsoft.AspNetCore.Identity;
using MovieApi.DTOs;
using MovieApi.Models;
using MovieApi.Repositories;
using MySqlConnector;

namespace MovieApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var users = app.MapGroup("/api/users");

        users.MapPost("", async (
            CreateUserDto data,
            UserRepository repository,
            IPasswordHasher<User> passwordHasher) =>
        {
            if (string.IsNullOrWhiteSpace(data.Username) ||
                data.Username.Length < 3 ||
                data.Username.Length > 50 ||
                string.IsNullOrWhiteSpace(data.Password) ||
                data.Password.Length < 10 ||
                data.Password.Length > 128)
            {
                return Results.BadRequest(new
                {
                    message = "Invalid registration data."
                });
            }

            string username = data.Username.Trim();

            // Skapa ett tillfälligt User-objekt för hashningen.
            User newUser = new(0, username, "");

            string passwordHash = passwordHasher.HashPassword(
                newUser,
                data.Password
            );

            try
            {
                User user = await repository.CreateUserAsync(
                    username,
                    passwordHash
                );

                UserResponseDto response = new(
                    user.Id,
                    user.Username
                );

                return Results.Json(
                    response,
                    statusCode: StatusCodes.Status201Created
                );
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                return Results.Conflict(new
                {
                    message = "Username is already taken."
                });
            }
        });
    }
}