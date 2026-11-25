namespace MovieApi.Models;

public record User(
    int Id,
    string Username,
    string PasswordHash
);