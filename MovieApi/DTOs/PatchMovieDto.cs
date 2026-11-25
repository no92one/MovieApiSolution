namespace MovieApi.DTOs;

public record PatchMovieDto(
    string? Title,
    string? Director,
    int? ReleaseYear
);