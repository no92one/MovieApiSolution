namespace MovieApi.DTOs;

public record CreateUserDto(
    string? Username,
    string? Password
);