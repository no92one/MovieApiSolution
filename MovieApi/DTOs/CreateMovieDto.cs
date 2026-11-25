namespace MovieApi.DTOs;

public record CreateMovieDto(
    string Title,
    string Director,
    int ReleaseYear
);